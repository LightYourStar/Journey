using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JO
{
    public class LDMainEntrance : LDBaseMono
    {
        private void Awake()
        {
            InitFocusInfo();
            AdapterCanvas();
            StartGame();
        }

        private static void StartGame()
        {
            LoadKeepNode();
        }

        public static void LoadKeepNode()
        {
            string path = "Prefabs/Persistent/KeepNode";
            // var prefab = Resources.Load<GameObject>(path);
            // if (prefab == null)
            // {
            //     Debug.LogError($"Load failed: {path}");
            //     return ;
            // }
            //
            // var go = Instantiate(prefab);
            // go.name = "KeepNode";
            // if (!go.activeSelf) go.SetActive(true);
            ReloadAddressables();
            AsyncOperationHandle<GameObject> keepNodeHandle = Addressables.InstantiateAsync("Assets/ResBundle/Prefabs/Persistent/KeepNode.prefab");
            keepNodeHandle.WaitForCompletion();
        }

        private static void ReloadAddressables()
        {
// #if USE_ADDRESSABLES
            // string catalogFile = MainIoUtils.BundlePath + "/catalog.json";
            // Debug.Log(" InitAddressables 111 " + catalogFile);
            // if (!File.Exists(catalogFile))
            // {
            //     return;
            // }
            // Debug.Log(" InitAddressables 2222 " + catalogFile);
            // Addressables.ClearResourceLocators();
            // AsyncOperationHandle<IResourceLocator> loadLog = Addressables.LoadContentCatalogAsync(catalogFile);
            // loadLog.WaitForCompletion();
// #endif

        }

        /// <summary>
        /// 初始化焦点信息
        /// </summary>
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
        /// 进行ui适配
        /// </summary>
        private void AdapterCanvas()
        {
            // LDUIAdapter.AdapterMainScene(gameObject.transform);
        }
    }
}