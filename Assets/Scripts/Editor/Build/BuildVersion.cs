using UnityEngine;

[CreateAssetMenu(menuName = "Build/BuildVersion", fileName = "Assets/Build/BuildVersion")]
public class BuildVersion : ScriptableObject
{
    [Header("App")]
    public string AppVersion = "1.0.0";
    public int AndroidVersionCode = 10000;

    [Header("Patch")]
    public long ResVersion = 2025122201;
    public long CodeVersion = 2025122201;

    [Header("Remote")]
    public string CdnBaseUrl = "http://172.18.18.28:8000/game/android/"; // 先用本地模拟CDN
    public string AddressablesRemotePath = "aa/";
    public string HotfixPath = "hotfix/";

    [Header("PreDownload Labels")]
    public string[] PreDownloadLabels = new[] { "boot", "common_ui" };

    [Header("Hotfix Files (relative to HotfixPath)")]
    public string[] HotfixFiles = new[] { "HotUpdate.dll.bytes" }; // 你后面可加 AOTMeta.bytes 等
}