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
    // Addressables 中存放热更 dll(.bytes) 的目录，需加入 Addressables 组
    private const string HotfixDllOutputDir = "Assets/ResBundle/HotfixDlls";
    // AOT 补充元数据 dll(.bytes) 目录
    private const string AOTDllOutputDir = "Assets/ResBundle/AOTDlls";

    // Jenkins 调用入口：-executeMethod UIManager.Build.BuildScript.BuildAndroid
    public static void BuildAndroid()
    {
        try
        {
            Debug.Log("===== BuildAndroid Start =====");

            // 1. 应用版本号到 PlayerSettings
            BuildTools.ApplyAppVersionToPlayerSettings();

            // 2. HybridCLR：生成所有必要文件（桥接函数、裁剪配置、AOT泛型等）
            GenerateHybridCLR();

            // 3. 拷贝热更 dll 和 AOT 补充元数据 dll 到 Addressables 资源目录
            CopyHotfixDllsToAddressables();

            // 4. 刷新 AssetDatabase，确保新拷贝的 .bytes 文件被识别
            AssetDatabase.Refresh();

            // 5. 构建 Addressables
            BuildAddressables();

            // 6. 构建 APK
            BuildAPK();

            Debug.Log("===== BuildAndroid Success =====");
        }
        catch (Exception e)
        {
            Debug.LogError($"BuildAndroid Failed: {e}");
            EditorApplication.Exit(1);
        }
    }

    private static void GenerateHybridCLR()
    {
        Debug.Log("[Build] HybridCLR GenerateAll...");

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        var installer = new HybridCLR.Editor.Installer.InstallerController();
        if (!installer.HasInstalledHybridCLR())
        {
            // Jenkins 清空工作区后 LocalIl2CppData 目录丢失，但 il2cpp_plus_repo 在仓库中已包含
            // hybridclr 子目录，直接用本地 repo 重新 install，避免 clone 网络依赖
            string libil2cppSrcDir = Path.GetFullPath("HybridCLRData/il2cpp_plus_repo/libil2cpp");
            if (!Directory.Exists(libil2cppSrcDir))
                throw new Exception($"HybridCLR install source not found: {libil2cppSrcDir}\n请确保 HybridCLRData/il2cpp_plus_repo 已提交到仓库。");

            Debug.Log($"[Build] HybridCLR not installed, installing from local: {libil2cppSrcDir}");
            installer.InstallFromLocal(libil2cppSrcDir);
            Debug.Log("[Build] HybridCLR install done.");
        }

        PrebuildCommand.GenerateAll();
        Debug.Log("[Build] HybridCLR GenerateAll done.");
    }

    private static void CopyHotfixDllsToAddressables()
    {
        Debug.Log("[Build] Copying HybridCLR dlls to Addressables...");

        // 热更 dll 源目录
        string hotfixSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(BuildTarget.Android);
        // AOT 补充元数据 dll 源目录
        string aotSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(BuildTarget.Android);

        CopyDllsAsBytes(hotfixSrcDir, HotfixDllOutputDir, SettingsUtil.HotUpdateAssemblyFilesExcludePreserved);
        CopyAOTDllsAsBytes(aotSrcDir, AOTDllOutputDir);

        Debug.Log("[Build] Dll copy done.");
    }

    /// <summary>
    /// 只拷贝 HybridCLR 配置中声明的热更程序集
    /// </summary>
    private static void CopyDllsAsBytes(string srcDir, string dstDir, System.Collections.Generic.List<string> dllNames)
    {
        Directory.CreateDirectory(dstDir);
        foreach (string dllName in dllNames)
        {
            string src = Path.Combine(srcDir, dllName);
            if (!File.Exists(src))
            {
                Debug.LogWarning($"[Build] Hotfix dll not found, skip: {src}");
                continue;
            }
            string dst = Path.Combine(dstDir, dllName + ".bytes");
            File.Copy(src, dst, overwrite: true);
            Debug.Log($"[Build] Copied hotfix dll: {dllName} -> {dst}");
        }
    }

    /// <summary>
    /// 拷贝 AOTGenericReferences 中声明的 AOT 补充元数据 dll
    /// </summary>
    private static void CopyAOTDllsAsBytes(string srcDir, string dstDir)
    {
        Directory.CreateDirectory(dstDir);
        foreach (string dllName in AOTGenericReferences.PatchedAOTAssemblyList)
        {
            string src = Path.Combine(srcDir, dllName);
            if (!File.Exists(src))
            {
                Debug.LogWarning($"[Build] AOT dll not found, skip: {src}");
                continue;
            }
            string dst = Path.Combine(dstDir, dllName + ".bytes");
            File.Copy(src, dst, overwrite: true);
            Debug.Log($"[Build] Copied AOT dll: {dllName} -> {dst}");
        }
    }

    private static void BuildAddressables()
    {
        Debug.Log("[Build] Building Addressables...");

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
            throw new Exception("AddressableAssetSettings not found.");

        AddressableAssetSettings.CleanPlayerContent(settings.ActivePlayerDataBuilder);

        AddressablesPlayerBuildResult result;
        AddressableAssetSettings.BuildPlayerContent(out result);

        if (!string.IsNullOrEmpty(result.Error))
            throw new Exception($"Addressables build failed: {result.Error}");

        Debug.Log($"[Build] Addressables built. Duration: {result.Duration:F2}s");
    }

    private static void BuildAPK()
    {
        var version = BuildTools.LoadBuildVersion();
        if (version == null)
            throw new Exception("BuildVersion.asset not found.");

        string outputDir = Path.GetFullPath("Build/Android");
        Directory.CreateDirectory(outputDir);

        string apkName = $"game_{version.AppVersion}_{version.AndroidVersionCode}.apk";
        string outputPath = Path.Combine(outputDir, apkName);

        Debug.Log($"[Build] Building APK => {outputPath}");

        var buildOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = outputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None,
        };

        bool isDev = Array.IndexOf(Environment.GetCommandLineArgs(), "-development") >= 0;
        if (isDev)
        {
            buildOptions.options |= BuildOptions.Development;
            Debug.Log("[Build] Development build enabled.");
        }

        var report = BuildPipeline.BuildPlayer(buildOptions);
        var summary = report.summary;

        if (summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new Exception($"BuildPlayer failed: result={summary.result}, errors={summary.totalErrors}");

        Debug.Log($"[Build] APK done: {outputPath} ({summary.totalSize / 1024 / 1024} MB)");
    }

    private static string[] GetEnabledScenes()
    {
        var list = new System.Collections.Generic.List<string>();
        foreach (var s in EditorBuildSettings.scenes)
        {
            if (s.enabled) list.Add(s.path);
        }
        if (list.Count == 0)
            throw new Exception("Build Settings 中没有启用的场景。");
        return list.ToArray();
    }
}
#endif