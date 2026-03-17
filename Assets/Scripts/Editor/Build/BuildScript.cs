#if UNITY_EDITOR
using System;
using System.IO;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class BuildScript
{
    private const string HotfixDllOutputDir = "Assets/ResBundle/HotfixDlls";
    private const string AOTDllOutputDir    = "Assets/ResBundle/AOTDlls";

    public static void BuildAndroid()
    {
        GenerateAndPrepare();
        Build();

    }

    // ─────────────────────────────────────────────────────────────
    // Step 1: Jenkins 第一次调用
    //   -executeMethod BuildScript.GenerateAndPrepare
    //   完成 HybridCLR Generate + 拷贝 dll + 构建 Addressables
    //   StripAOT 内部会跑一次 BuildPlayer，进程结束后内存干净
    // ─────────────────────────────────────────────────────────────
    public static void GenerateAndPrepare()
    {
        try
        {
            Debug.Log("===== GenerateAndPrepare Start =====");

            BuildTools.ApplyAppVersionToPlayerSettings();
            GenerateHybridCLR();
            CopyHotfixDllsToAddressables();
            AssetDatabase.Refresh();
            BuildAddressables();

            Debug.Log("===== GenerateAndPrepare Success =====");
            EditorApplication.Exit(0);
        }
        catch (Exception e)
        {
            Debug.LogError($"GenerateAndPrepare Failed: {e}");
            EditorApplication.Exit(1);
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Step 2: Jenkins 第二次调用（新进程，内存干净）
    //   -executeMethod BuildScript.BuildAndroid
    //   只做 APK 打包，不再触发任何 HybridCLR 生成
    // ─────────────────────────────────────────────────────────────
    public static void Build()
    {
        try
        {
            Debug.Log("===== BuildAndroid Start =====");

            BuildTools.ApplyAppVersionToPlayerSettings();
            BuildAPK();

            Debug.Log("===== BuildAndroid Success =====");
            EditorApplication.Exit(0);
        }
        catch (Exception e)
        {
            Debug.LogError($"BuildAndroid Failed: {e}");
            EditorApplication.Exit(1);
        }
    }

    // ─────────────────────────────────────────────────────────────

    private static void GenerateHybridCLR()
    {
        Debug.Log("[Build] HybridCLR GenerateAll...");

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        var installer = new HybridCLR.Editor.Installer.InstallerController();
        if (!installer.HasInstalledHybridCLR())
        {
            string libil2cppSrcDir = Path.GetFullPath("HybridCLRData/il2cpp_plus_repo/libil2cpp");
            if (!Directory.Exists(libil2cppSrcDir))
                throw new Exception($"HybridCLR install source not found: {libil2cppSrcDir}");

            Debug.Log($"[Build] Installing HybridCLR from local: {libil2cppSrcDir}");
            installer.InstallFromLocal(libil2cppSrcDir);
        }

        PrebuildCommand.GenerateAll();
        Debug.Log("[Build] HybridCLR GenerateAll done.");
    }

    private static void CopyHotfixDllsToAddressables()
    {
        Debug.Log("[Build] Copying dlls to Addressables...");
        string hotfixSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(BuildTarget.Android);
        string aotSrcDir    = SettingsUtil.GetAssembliesPostIl2CppStripDir(BuildTarget.Android);
        CopyDllsAsBytes(hotfixSrcDir, HotfixDllOutputDir, SettingsUtil.HotUpdateAssemblyFilesExcludePreserved);
        CopyAOTDllsAsBytes(aotSrcDir, AOTDllOutputDir);
        Debug.Log("[Build] Dll copy done.");
    }

    private static void CopyDllsAsBytes(string srcDir, string dstDir, System.Collections.Generic.List<string> dllNames)
    {
        Directory.CreateDirectory(dstDir);
        foreach (string dllName in dllNames)
        {
            string src = Path.Combine(srcDir, dllName);
            if (!File.Exists(src)) { Debug.LogWarning($"[Build] Not found, skip: {src}"); continue; }
            File.Copy(src, Path.Combine(dstDir, dllName + ".bytes"), overwrite: true);
        }
    }

    private static void CopyAOTDllsAsBytes(string srcDir, string dstDir)
    {
        Directory.CreateDirectory(dstDir);
        foreach (string dllName in AOTGenericReferences.PatchedAOTAssemblyList)
        {
            string src = Path.Combine(srcDir, dllName);
            if (!File.Exists(src)) { Debug.LogWarning($"[Build] AOT not found, skip: {src}"); continue; }
            File.Copy(src, Path.Combine(dstDir, dllName + ".bytes"), overwrite: true);
        }
    }

    private static void BuildAddressables()
    {
        Debug.Log("[Build] Building Addressables...");
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) throw new Exception("AddressableAssetSettings not found.");
        AddressableAssetSettings.CleanPlayerContent(settings.ActivePlayerDataBuilder);
        AddressableAssetSettings.BuildPlayerContent(out var result);
        if (!string.IsNullOrEmpty(result.Error)) throw new Exception($"Addressables build failed: {result.Error}");
        Debug.Log($"[Build] Addressables built. Duration: {result.Duration:F2}s");
    }

    private static void BuildAPK()
    {
        var version = BuildTools.LoadBuildVersion();
        if (version == null) throw new Exception("BuildVersion.asset not found.");

        string outputDir = Path.GetFullPath("Build/Android");
        Directory.CreateDirectory(outputDir);
        string outputPath = Path.Combine(outputDir, $"game_{version.AppVersion}_{version.AndroidVersionCode}.apk");

        Debug.Log($"[Build] Building APK => {outputPath}");

        var opts = new BuildPlayerOptions
        {
            scenes             = GetEnabledScenes(),
            locationPathName   = outputPath,
            target             = BuildTarget.Android,
            options            = BuildOptions.None,
        };

        if (Array.IndexOf(Environment.GetCommandLineArgs(), "-development") >= 0)
            opts.options |= BuildOptions.Development;

        var summary = BuildPipeline.BuildPlayer(opts).summary;
        if (summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new Exception($"BuildPlayer failed: {summary.result}, errors={summary.totalErrors}");

        Debug.Log($"[Build] APK done: {outputPath} ({summary.totalSize / 1024 / 1024} MB)");
    }

    private static string[] GetEnabledScenes()
    {
        var list = new System.Collections.Generic.List<string>();
        foreach (var s in EditorBuildSettings.scenes)
            if (s.enabled) list.Add(s.path);
        if (list.Count == 0) throw new Exception("Build Settings 中没有启用的场景。");
        return list.ToArray();
    }
}
#endif
