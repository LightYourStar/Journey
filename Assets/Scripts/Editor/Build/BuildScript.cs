#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace UIManager.Build
{
    public class BuildScript
    {
        // Jenkins 调用入口：-executeMethod UIManager.Build.BuildScript.BuildAndroid
        [MenuItem("打包工具/build")]
        public static void BuildAndroid()
        {
            try
            {
                Debug.Log("===== BuildAndroid Start =====");

                // 1. 读取版本配置
                BuildTools.ApplyAppVersionToPlayerSettings();

                // 2. 构建 Addressables（先清理旧缓存再全量构建）
                BuildAddressables();

                // 3. 构建 APK
                BuildAPK();

                Debug.Log("===== BuildAndroid Success =====");
            }
            catch (Exception e)
            {
                Debug.LogError($"BuildAndroid Failed: {e}");
                // Jenkins 通过非零退出码判断失败
                EditorApplication.Exit(1);
            }
        }

        private static void BuildAddressables()
        {
            Debug.Log("[Build] Building Addressables...");

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
                throw new Exception("AddressableAssetSettings not found. 请确认 Addressables 已初始化。");

            // 清理旧构建缓存，保证干净构建
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

            // 输出目录：项目根/Build/Android/
            string outputDir = Path.GetFullPath("Build/Android");
            Directory.CreateDirectory(outputDir);

            string apkName = $"game_{version.AppVersion}_{version.AndroidVersionCode}.apk";
            string outputPath = Path.Combine(outputDir, apkName);

            Debug.Log($"[Build] Building APK => {outputPath}");

            // 从 EditorBuildSettings 中收集所有启用的场景
            var scenes = GetEnabledScenes();

            var buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None,
            };

            // Jenkins 可通过命令行参数 -development 开启 Development Build
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-development") >= 0)
            {
                buildOptions.options |= BuildOptions.Development;
                Debug.Log("[Build] Development build enabled.");
            }

            var report = BuildPipeline.BuildPlayer(buildOptions);
            var summary = report.summary;

            if (summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new Exception($"BuildPlayer failed: result={summary.result}, errors={summary.totalErrors}");

            Debug.Log($"[Build] APK built successfully: {outputPath} ({summary.totalSize / 1024 / 1024} MB)");
        }

        private static string[] GetEnabledScenes()
        {
            var scenes = EditorBuildSettings.scenes;
            var list = new System.Collections.Generic.List<string>();
            foreach (var s in scenes)
            {
                if (s.enabled)
                    list.Add(s.path);
            }
            if (list.Count == 0)
                throw new Exception("没有找到任何启用的场景，请在 Build Settings 中添加场景。");
            return list.ToArray();
        }
    }
}
#endif