using System;

namespace JO.Patch
{
    [Serializable]
    public class PatchManifest
    {
        public int minAppVersionCode;
        public long resVersion;
        public long codeVersion;

        public string cdnBaseUrl;
        public string addressablesRemotePath;
        public string hotfixPath;

        public string[] preDownloadLabels;
        public PatchManifestFile[] files;
    }

    [Serializable]
    public class PatchManifestFile
    {
        public string name;
        public string url;
        public long size;
        public string sha256;
        public string type;
    }
}