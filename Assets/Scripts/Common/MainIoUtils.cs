using UnityEngine;

namespace JO
{
    public class MainIoUtils
    {
        public static string BundlePath;

        static MainIoUtils()
        {
            RuntimePlatform platform = Application.platform;
            if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.OSXEditor)
            {
                BundlePath = Application.dataPath + "/../StreamingAssets/bundle/";
            }
            else
            {
                BundlePath = Application.persistentDataPath + "/bundle/";
            }
        }
    }
}