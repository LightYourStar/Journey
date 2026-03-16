#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    [MenuItem("打包工具/build")]
    public static void BuildAndroid()
    {
        try
        {
            Debug.Log("===== BuildAndroid Start =====");
            Debug.Log($"[Build] ProjectPath: {Directory.GetCurrentDirectory()}");
            Debug.Log($"[Build] ActiveBuildTarget: {EditorUserBuildSettings.activeBuildTarget}");
            Debug.Log($"[Build] CommandLineArgs: {string.Join(" ", Environment.GetCommandLineArgs())}");

            // 1. 切换到 Android 平台
            SwitchToAndroid();

            // 2. 读取版本配置并应用
            BuildTools.ApplyAppVersionToPlayerSettings();

            // 3. 构建 Addressables
            BuildAddressables();

            // 4. 构建 APK
            BuildAPK();

            Debug.Log("===== BuildAndroid Success =====");
            EditorApplication.Exit(0);
        }
        catch (Exception e)
        {
            Debug.LogError("===== BuildAndroid Failed =====");
            Debug.LogError(e.ToString());
            EditorApplication.Exit(1);
        }
    }

    private static void SwitchToAndroid()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            Debug.Log("[Build] Already on Android target.");
            return;
        }

        Debug.Log("[Build] Switching build target to Android...");

        bool success = EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Android,
            BuildTarget.Android);

        if (!success)
        {
            throw new Exception("切换到 Android 平台失败。");
        }

        Debug.Log("[Build] Switched to Android target successfully.");
    }

    private static void BuildAddressables()
    {
        Debug.Log("[Build] Building Addressables...");

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
            throw new Exception("AddressableAssetSettings not found. 请确认 Addressables 已初始化。");

        Debug.Log($"[Build] Addressables ActivePlayerDataBuilder: {settings.ActivePlayerDataBuilder?.Name}");

        AddressableAssetSettings.CleanPlayerContent(settings.ActivePlayerDataBuilder);

        AddressablesPlayerBuildResult result;
        AddressableAssetSettings.BuildPlayerContent(out result);

        if (!string.IsNullOrEmpty(result.Error))
            throw new Exception($"Addressables build failed: {result.Error}");

        Debug.Log($"[Build] Addressables built successfully. Duration: {result.Duration:F2}s");
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

        Debug.Log($"[Build] AppVersion: {version.AppVersion}");
        Debug.Log($"[Build] AndroidVersionCode: {version.AndroidVersionCode}");
        Debug.Log($"[Build] OutputDir: {outputDir}");
        Debug.Log($"[Build] OutputPath: {outputPath}");

        string[] scenes = GetEnabledScenes();
        Debug.Log($"[Build] EnabledScenes ({scenes.Length}):");
        foreach (string scene in scenes)
        {
            Debug.Log($"[Build] Scene => {scene}");
        }

        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None,
        };

        if (Array.IndexOf(Environment.GetCommandLineArgs(), "-development") >= 0)
        {
            buildOptions.options |= BuildOptions.Development;
            Debug.Log("[Build] Development build enabled.");
        }

        Debug.Log("[Build] Calling BuildPipeline.BuildPlayer...");

        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        BuildSummary summary = report.summary;

        Debug.Log($"[Build] Result: {summary.result}");
        Debug.Log($"[Build] TotalErrors: {summary.totalErrors}");
        Debug.Log($"[Build] TotalWarnings: {summary.totalWarnings}");
        Debug.Log($"[Build] TotalSize: {summary.totalSize}");
        Debug.Log($"[Build] TotalTime: {summary.totalTime}");

        if (summary.result != BuildResult.Succeeded)
        {
            throw new Exception(
                $"BuildPlayer failed: result={summary.result}, errors={summary.totalErrors}, warnings={summary.totalWarnings}");
        }

        Debug.Log($"[Build] APK built successfully: {outputPath}");
    }

    private static string[] GetEnabledScenes()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        List<string> list = new List<string>();

        foreach (EditorBuildSettingsScene s in scenes)
        {
            if (s.enabled)
                list.Add(s.path);
        }

        if (list.Count == 0)
            throw new Exception("没有找到任何启用的场景，请在 Build Settings 中添加场景。");

        return list.ToArray();
    }
}
#endif