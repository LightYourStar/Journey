#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using JO.Patch;
public static class BuildTools
{
    private const string BuildVersionAssetPath = "Assets/Build/BuildVersion.asset";

    [MenuItem("Build/01 Load BuildVersion Asset")]
    public static BuildVersion LoadBuildVersion()
    {
        var v = AssetDatabase.LoadAssetAtPath<BuildVersion>(BuildVersionAssetPath);
        if (v == null)
        {
            Debug.LogError($"找不到 BuildVersion.asset：{BuildVersionAssetPath}");
        }
        else
        {
            Debug.Log($"Loaded BuildVersion: App={v.AppVersion} VC={v.AndroidVersionCode} Res={v.ResVersion} Code={v.CodeVersion}");
        }
        return v;
    }

    [MenuItem("Build/02 Apply AppVersion To PlayerSettings")]
    public static void ApplyAppVersionToPlayerSettings()
    {
        var v = LoadBuildVersion();
        if (v == null) return;

        PlayerSettings.bundleVersion = v.AppVersion;
        PlayerSettings.Android.bundleVersionCode = v.AndroidVersionCode;

        Debug.Log($"Applied PlayerSettings: bundleVersion={v.AppVersion}, androidVersionCode={v.AndroidVersionCode}");
    }

    [MenuItem("Build/03 Bump Res/Code Version (yyyyMMddNN)")]
    public static void BumpResAndCodeVersion()
    {
        var v = LoadBuildVersion();
        if (v == null) return;

        long next = NextDateSerialVersion(v.ResVersion);
        v.ResVersion = next;
        v.CodeVersion = next;

        EditorUtility.SetDirty(v);
        AssetDatabase.SaveAssets();
        Debug.Log($"Bumped Res/Code Version => {next}");
    }

    /// <summary>
    /// 版本格式：yyyyMMddNN（例如 2025122201）
    /// 若当天已是 01，则递增到 02；跨天就从 01 开始
    /// </summary>
    private static long NextDateSerialVersion(long current)
    {
        var today = DateTime.Now.ToString("yyyyMMdd");
        long baseToday = long.Parse(today) * 100;

        if (current >= baseToday && current < baseToday + 99)
        {
            return current + 1;
        }
        return baseToday + 1;
    }

    [MenuItem("Build/04 Generate manifest.json (Hotfix only)")]
    public static void GenerateManifest()
    {
        var v = LoadBuildVersion();
        if (v == null) return;

        // 你本地导出/部署的“CDN根目录”，先从 CdnBaseUrl 里推断一个本地目录（你也可以改成手填路径）
        // 这里给个简单做法：把 manifest 输出到项目根下的 BuildCDN/game/android/
        string cdnRootLocal = Path.GetFullPath("BuildCDN/game/android");
        Directory.CreateDirectory(cdnRootLocal);

        // 假设热更文件已经被你拷贝到了 BuildCDN/game/android/hotfix/ 下
        string hotfixDir = Path.Combine(cdnRootLocal, v.HotfixPath.TrimEnd('/', '\\'));
        Directory.CreateDirectory(hotfixDir);

        var files = new List<PatchManifestFile>();
        foreach (var rel in v.HotfixFiles)
        {
            string localPath = Path.Combine(hotfixDir, rel.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(localPath))
            {
                Debug.LogWarning($"Hotfix file not found (先跳过): {localPath}");
                continue;
            }

            files.Add(new PatchManifestFile
            {
                name = $"{v.HotfixPath}{rel}".Replace("\\", "/"),
                url = $"{v.HotfixPath}{rel}".Replace("\\", "/"),
                size = new FileInfo(localPath).Length,
                sha256 = Sha256File(localPath),
                type = "hotfix"
            });
        }

        var manifest = new PatchManifest
        {
            minAppVersionCode = v.AndroidVersionCode,   // 你也可以改成更低，做到“向后兼容不强更”
            resVersion = v.ResVersion,
            codeVersion = v.CodeVersion,
            cdnBaseUrl = v.CdnBaseUrl,
            addressablesRemotePath = v.AddressablesRemotePath,
            hotfixPath = v.HotfixPath,
            preDownloadLabels = v.PreDownloadLabels,
            files = files.ToArray()
        };

        string json = JsonUtility.ToJson(manifest, true);
        string manifestPath = Path.Combine(cdnRootLocal, "manifest.json");
        var utf8NoBom = new UTF8Encoding(false);
        File.WriteAllText(manifestPath, json, utf8NoBom);

        Debug.Log($"Generated manifest.json => {manifestPath}");
        EditorUtility.RevealInFinder(manifestPath);
    }

    private static string Sha256File(string path)
    {
        using var fs = File.OpenRead(path);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(fs);
        var sb = new StringBuilder(hash.Length * 2);
        for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("x2"));
        return sb.ToString();
    }
}
#endif