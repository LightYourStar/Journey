using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using JO.Patch;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JO
{
    public class LDMainEntrance : LDBaseMono
    {
        [System.Serializable]
        private class LocalVersion
        {
            public long resVersion;
            public long codeVersion;
        }

        /// <summary>
        /// 启动时拉取的 manifest 地址
        /// 注意：要和你 BuildTools 生成的路径一致
        /// 目前是：BuildCDN/game/android/manifest.json
        /// 对应 HTTP： http://172.18.18.28:8000/game/android/manifest.json
        /// </summary>
        [SerializeField]
        private string m_ManifestUrl = "http://172.18.18.28:8000/game/android/manifest.json";

        private static string s_HotfixDir => Path.Combine(Application.persistentDataPath, "hotfix");

        private void Awake()
        {
            InitFocusInfo();
            AdapterCanvas();
            StartCoroutine(BootRoutine());
        }

        /// <summary>
        /// 启动总流程：
        /// 1. 拉 manifest
        /// 2. 版本检查（预留强更）
        /// 3. 设置 Addressables 远端路径
        /// 4. 初始化 Addressables
        /// 5. Catalog 更新 + 预下载
        /// 6. 下载热更 DLL（预留 HybridCLR 入口）
        /// 7. 创建常驻节点，进入游戏
        /// </summary>
        private IEnumerator BootRoutine()
        {
            Debug.Log("[BOOT] BootRoutine start");

            // 0) 拉 manifest
            PatchManifest manifest = null;
            yield return StartCoroutine(FetchManifestCoroutine(m_ManifestUrl, m => manifest = m));
            if (manifest == null)
            {
                Debug.LogError("[BOOT] FetchManifest failed, manifest == null");
                yield break;
            }

            // 1) App 版本检查（现在只是骨架，真正的 versionCode 你之后再接）
#if UNITY_ANDROID && !UNITY_EDITOR
            // TODO: 用 AndroidJavaObject 读取真正的 versionCode
            int localAppVersionCode = manifest.minAppVersionCode;
#else
            int localAppVersionCode = manifest.minAppVersionCode;
#endif

            if (localAppVersionCode < manifest.minAppVersionCode)
            {
                Debug.LogError($"[BOOT] App version too low. local={localAppVersionCode}, min={manifest.minAppVersionCode}");
                // TODO: 这里弹强更 UI，引导玩家去更新，然后 yield break
                // 暂时先直接进游戏的话，可以注释掉这段判断
                // yield break;
            }

            LocalVersion localVersion = LoadLocalVersion();
            long localResVersion  = localVersion?.resVersion  ?? 0;
            long localCodeVersion = localVersion?.codeVersion ?? 0;

            bool needResUpdate  = manifest.resVersion  > localResVersion;
            bool needCodeUpdate = manifest.codeVersion > localCodeVersion;

            Debug.Log($"[BOOT] localRes={localResVersion}, remoteRes={manifest.resVersion}, needResUpdate={needResUpdate}");
            Debug.Log($"[BOOT] localCode={localCodeVersion}, remoteCode={manifest.codeVersion}, needCodeUpdate={needCodeUpdate}");

            // 2) 根据 manifest 设置 BundlePath / Remote.LoadPath
            string aaBase = manifest.cdnBaseUrl.TrimEnd('/') + "/" +
                            manifest.addressablesRemotePath.Trim('/');

            MainIoUtils.BundlePath = aaBase;

            // 给 Addressables 的 Remote.LoadPath 里的 {MainApp.MainIoUtils.BundlePath} 赋值
            AddressablesRuntimeProperties.ClearCachedPropertyValues();
            AddressablesRuntimeProperties.SetPropertyValue(
                "MainApp.MainIoUtils.BundlePath",
                aaBase
            );

            Debug.Log("[BOOT] Addressables Base = " + aaBase);

            UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (handle, ex) =>
            {
                Debug.LogError($"[Addressables] Exception. Op={handle.DebugName}, ex={ex}");
            };

            // 3) 初始化 Addressables
            yield return InitAddressablesCoroutine();

            // 4) 【按需】更新 Catalog + 预下载
            if (needResUpdate)
            {
                Debug.Log("[BOOT] NeedResUpdate = true, run catalog update & pre-download.");

                // 清理旧版本的缓存（只清预下载 label 对应的 bunlde）
                yield return HandleResVersionChangeCoroutine(manifest);

                // 更新 Catalog
                yield return UpdateCatalogsCoroutine();

                // 预下载常用 label
                yield return PreDownloadLabelsCoroutine(manifest.preDownloadLabels);
            }
            else
            {
                Debug.Log("[BOOT] Resource up to date, skip catalog update & predownload.");
            }

            // 5) 【按需】下载热更 DLL
            if (needCodeUpdate)
            {
                Debug.Log("[BOOT] NeedCodeUpdate = true, download hotfix dll.");
                yield return DownloadHotfixFilesCoroutine(manifest);
            }
            else
            {
                Debug.Log("[HOTFIX] Code up to date, try load local dll.");
                // 不需要下新的，直接尝试加载本地上次留下的 dll
                LoadHotfixAssemblies();
            }

            // 7) 创建常驻节点
            yield return LoadKeepNodeCoroutine();

            Debug.Log("[BOOT] BootRoutine done, ready to enter game.");
            // TODO: 在这里进入登陆/主界面，例如：
            // SceneManager.LoadScene("MainScene");
        }

        #region Manifest

        private IEnumerator FetchManifestCoroutine(string url, System.Action<PatchManifest> onDone)
        {
            Debug.Log("[BOOT] FetchManifest " + url);

            using (var req = UnityWebRequest.Get(url))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("[BOOT] FetchManifest FAILED: " + req.error);
                    onDone?.Invoke(null);
                    yield break;
                }

                var json = req.downloadHandler.text;
                PatchManifest manifest = null;

                try
                {
                    Debug.Log("[BOOT] json:" + json);
                    if (!string.IsNullOrEmpty(json) && json[0] == '\uFEFF')//去掉Utf8-bom
                    {
                        json = json.Substring(1);
                    }

                    manifest = JsonUtility.FromJson<PatchManifest>(json);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("[BOOT] Parse manifest FAILED: " + e);
                }

                onDone?.Invoke(manifest);
            }
        }

        #endregion

        #region Addressables 初始化/更新/预下载

        private static IEnumerator InitAddressablesCoroutine()
        {
            Debug.Log("[BOOT] InitAddressables...");
            AsyncOperationHandle<IResourceLocator> handle = Addressables.InitializeAsync();

            while (!handle.IsDone)
            {
                yield return null;
            }

            // ★ 先判断句柄是否有效
            if (!handle.IsValid())
            {
                Debug.LogError("[BOOT] InitAddressables FAILED: handle is invalid.");
                yield break;
            }

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("[BOOT] InitAddressables FAILED: " + handle.OperationException);
                yield break;
            }

            Debug.Log("[BOOT] InitAddressables Succeeded");
        }


        /// <summary>
        /// 更新 Catalog
        /// </summary>
        /// <returns></returns>
        private static IEnumerator UpdateCatalogsCoroutine()
        {
            Debug.Log("[BOOT] CheckForCatalogUpdates...");
            AsyncOperationHandle<List<string>> checkHandle = Addressables.CheckForCatalogUpdates(false);
            yield return checkHandle;

            List<string> catalogs = checkHandle.Result;
            Addressables.Release(checkHandle);

            if (catalogs == null || catalogs.Count == 0)
            {
                Debug.Log("[BOOT] Catalog already up to date.");
                yield break;
            }

            Debug.Log($"[BOOT] UpdateCatalogs {catalogs.Count} catalogs...");
            var updateHandle = Addressables.UpdateCatalogs(catalogs, false);

            while (!updateHandle.IsDone)
            {
                yield return null;
            }

            if (updateHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("[BOOT] UpdateCatalogs FAILED: " + updateHandle.OperationException);
            }
            else
            {
                Debug.Log("[BOOT] UpdateCatalogs Succeeded");
            }

            Addressables.Release(updateHandle);
        }

        private static IEnumerator PreDownloadLabelsCoroutine(string[] labels)
        {
            if (labels == null || labels.Length == 0)
            {
                Debug.Log("[BOOT] No preDownloadLabels, skip.");
                yield break;
            }

            var ops = new List<AsyncOperationHandle>();

            foreach (var label in labels)
            {
                if (string.IsNullOrWhiteSpace(label))
                    continue;

                var op = Addressables.DownloadDependenciesAsync(
                    label,
                    Addressables.MergeMode.Union,
                    false
                );
                ops.Add(op);
            }

            if (ops.Count == 0)
                yield break;

            bool allDone = false;
            while (!allDone)
            {
                allDone = true;
                float avg = 0f;
                foreach (var op in ops)
                {
                    if (!op.IsDone) allDone = false;
                    avg += op.PercentComplete;
                }

                avg /= ops.Count;
                // 需要的话可以加个进度条
                // Debug.Log($"[BOOT] PreDownload progress: {avg:P0}");
                yield return null;
            }

            foreach (var op in ops)
            {
                if (!op.IsValid())
                {
                    Debug.LogError("[BOOT] DownloadDependencies got invalid handle.");
                    continue;
                }

                if (op.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError("[BOOT] DownloadDependencies FAILED: " + op.OperationException);
                }

                Addressables.Release(op);
            }

            Debug.Log("[BOOT] PreDownloadLabels done.");
        }

        private static IEnumerator LoadKeepNodeCoroutine()
        {
            const string key = "Assets/ResBundle/Prefabs/Persistent/KeepNode.prefab";
            Debug.Log("[BOOT] LoadKeepNode: " + key);

            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key);

            while (!handle.IsDone)
            {
                yield return null;
            }

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("[BOOT] LoadKeepNode FAILED: " + handle.OperationException);
                yield break;
            }

            var go = handle.Result;
            if (go != null)
            {
                go.name = "KeepNode";
                if (!go.activeSelf) go.SetActive(true);
                Debug.Log("[BOOT] KeepNode instantiated.");
            }
            else
            {
                Debug.LogError("[BOOT] LoadKeepNode: result is null");
            }
        }

        #endregion

        #region 热更 DLL 下载（SHA256 校验骨架）

        private static IEnumerator DownloadHotfixFilesCoroutine(PatchManifest manifest)
        {
            if (manifest == null || manifest.files == null || manifest.files.Length == 0)
            {
                Debug.Log("[HOTFIX] No files in manifest, skip.");
                yield break;
            }

            string baseUrl = manifest.cdnBaseUrl.TrimEnd('/');
            string localRoot = s_HotfixDir;

            if (!Directory.Exists(localRoot))
                Directory.CreateDirectory(localRoot);

            foreach (PatchManifestFile file in manifest.files)
            {
                if (file == null)
                    continue;

                if (!string.Equals(file.type, "hotfix", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                string url = baseUrl + "/" + file.url.TrimStart('/');
                string localPath = Path.Combine(localRoot, Path.GetFileName(file.name));

                // 已有并且校验通过就复用
                if (File.Exists(localPath))
                {
                    string localHash = ComputeSha256(localPath);
                    if (!string.IsNullOrEmpty(file.sha256) &&
                        string.Equals(localHash, file.sha256, System.StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log($"[HOTFIX] Use cached file: {localPath}");
                        continue;
                    }
                }

                Debug.Log($"[HOTFIX] Download {url}");
                using (var req = UnityWebRequest.Get(url))
                {
                    yield return req.SendWebRequest();

                    if (req.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("[HOTFIX] Download FAILED: " + req.error);
                        yield break;    // 策略：热更失败就不继续，你之后可以改成重试或跳过
                    }

                    try
                    {
                        File.WriteAllBytes(localPath, req.downloadHandler.data);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("[HOTFIX] Save file FAILED: " + e);
                        yield break;
                    }
                }

                if (!string.IsNullOrEmpty(file.sha256))
                {
                    string hash = ComputeSha256(localPath);
                    if (!string.Equals(hash, file.sha256, System.StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.LogError($"[HOTFIX] Sha256 mismatch for {localPath}");
                        yield break;
                    }
                }

                // 加载 Hotfix 程序集并调用入口
                LoadHotfixAssemblies();
            }

            Debug.Log("[HOTFIX] All hotfix files ready.");
        }

        private static string ComputeSha256(string path)
        {
            using (var stream = File.OpenRead(path))
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var hash = sha.ComputeHash(stream);
                return System.BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }


        private static void LoadHotfixAssemblies()
        {
            Debug.Log("[HOTFIX] LoadHotfixAssemblies" + s_HotfixDir);

            if (!Directory.Exists(s_HotfixDir))
            {
                Debug.LogWarning("[HOTFIX] hotfix dir not exist: " + s_HotfixDir);
                return;
            }

            var dllFiles = Directory.GetFiles(s_HotfixDir, "*.dll*");
            if (dllFiles.Length == 0)
            {
                Debug.Log("[HOTFIX] No hotfix dll found in " + s_HotfixDir);
                return;
            }

            foreach (string path in dllFiles)
            {
                try
                {
                    byte[] dllBytes = File.ReadAllBytes(path);

                    // HybridCLR 会接管 Assembly.Load
                    var asm = Assembly.Load(dllBytes);
                    Debug.Log("[HOTFIX] Loaded assembly: " + asm.FullName);

                    var entryType = asm.GetType("JO.HotfixEntry");
                    if (entryType != null)
                    {
                        var initMethod = entryType.GetMethod(
                            "Init",
                            BindingFlags.Public | BindingFlags.Static);

                        if (initMethod != null)
                        {
                            Debug.Log("[HOTFIX] Invoke HotfixEntry.Init()");
                            initMethod.Invoke(null, null);
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("[HOTFIX] Load assembly FAILED: " + path + "\n" + e);
                }
            }
        }

        #endregion

        #region 读取加载本地version
        private static LocalVersion LoadLocalVersion()
        {
            var path = Path.Combine(Application.persistentDataPath, "local_version.json");
            Debug.Log("[BOOT] LoadLocalVersion path = " + path);

            if (!File.Exists(path))
            {
                Debug.LogWarning("[BOOT] local_version.json not exist: ");
                return null;
            }

            try
            {
                var json = File.ReadAllText(path);
                if (string.IsNullOrEmpty(json))
                    return null;

                return JsonUtility.FromJson<LocalVersion>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[BOOT] LoadLocalVersion failed: " + e);
                return null;
            }
        }

        private static void SaveLocalVersion(long resVersion, long codeVersion)
        {
            var path = Path.Combine(Application.persistentDataPath, "local_version.json");

            try
            {
                var lv = new LocalVersion
                {
                    resVersion = resVersion,
                    codeVersion = codeVersion
                };

                var json = JsonUtility.ToJson(lv);
                File.WriteAllText(path, json);
                Debug.Log($"[BOOT] SaveLocalVersion: res={resVersion}, code={codeVersion}");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[BOOT] SaveLocalVersion failed: " + e);
            }
        }

        private static IEnumerator HandleResVersionChangeCoroutine(PatchManifest manifest)
        {
            if (manifest == null)
                yield break;

            LocalVersion local = LoadLocalVersion();
            long newResVer  = manifest.resVersion;
            long newCodeVer = manifest.codeVersion;

            // 第一次运行：没有本地版本文件，直接记录一次，不清缓存
            if (local == null)
            {
                Debug.Log("[BOOT] No local version, treat as first install.");
                SaveLocalVersion(newResVer, newCodeVer);
                yield break;
            }

            // 完全一致，直接用原有缓存
            if (local.resVersion == newResVer)
            {
                Debug.Log("[BOOT] ResVersion unchanged: " + newResVer);
                yield break;
            }

            Debug.Log($"[BOOT] Version changed: " +
                      $"localRes={local.resVersion}, remoteRes={newResVer}, " +
                      $"localCode={local.codeVersion}, remoteCode={newCodeVer}");

            // 策略：只清掉预下载 label 的缓存
            if (manifest.preDownloadLabels != null && manifest.preDownloadLabels.Length > 0)
            {
                foreach (var label in manifest.preDownloadLabels)
                {
                    if (string.IsNullOrWhiteSpace(label))
                        continue;

                    Debug.Log("[BOOT] ClearDependencyCache label=" + label);
                    AsyncOperationHandle<bool> clearHandle = Addressables.ClearDependencyCacheAsync(label, true);

                    while (!clearHandle.IsDone)
                        yield return null;

                    if (!clearHandle.IsValid() ||
                        clearHandle.Status != AsyncOperationStatus.Succeeded)
                    {
                        Debug.LogError("[BOOT] ClearDependencyCache failed for " + label +
                                       ": " + clearHandle.OperationException);
                    }
                }
            }

            // 更新本地版本记录
            SaveLocalVersion(newResVer, newCodeVer);
        }


        #endregion

        #region 其他初始化

        private void InitFocusInfo()
        {
#if UNITY_EDITOR
            Application.runInBackground = true;
#else
            Application.runInBackground = false;
#endif
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        /// <summary>
        /// 进行 UI 适配
        /// </summary>
        private void AdapterCanvas()
        {
            // 根据你自己的项目需要来适配
            // LDUIAdapter.AdapterMainScene(transform);
        }

        #endregion
    }
}