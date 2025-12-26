using UnityEngine;

namespace JO
{
    public class MainIoUtils
    {
        public static string BundlePath;

        static MainIoUtils()
        {
            RuntimePlatform platform = Application.platform;

            if (platform == RuntimePlatform.WindowsEditor ||
                platform == RuntimePlatform.OSXEditor ||
                platform == RuntimePlatform.Android)
            {
                // 先统一走 HTTP，直接指向 BuildCDN 的 aa 目录
                BundlePath = "http://172.18.18.28:8000/game/android/aa";
            }
            else
            {
                // 其他平台以后再考虑本地缓存
                BundlePath = Application.persistentDataPath + "/bundle/";
            }

            Debug.Log("[MainIoUtils] BundlePath = " + BundlePath);
        }
    }

}