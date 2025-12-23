#if UNITY_EDITOR
using UnityEditor;

public static class VersionApply
{
    public static void ApplyToPlayerSettings(BuildVersion v)
    {
        PlayerSettings.bundleVersion = v.AppVersion;
        PlayerSettings.Android.bundleVersionCode = v.AndroidVersionCode; // :contentReference[oaicite:3]{index=3}
    }
}
#endif