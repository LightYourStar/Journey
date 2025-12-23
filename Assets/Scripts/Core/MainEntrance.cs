using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JO
{
    public class LDMainEntrance : LDBaseMono
    {
        private void Awake()
        {
            AdapterCanvas();
            StartGame();
        }

        private static void StartGame()
        {

            InitAddressables();
            LoadKeepNode();
        }

        private static void InitAddressables()
        {
            string aaBase = MainIoUtils.BundlePath.TrimEnd('/');
            AddressablesRuntimeProperties.ClearCachedPropertyValues();
            AddressablesRuntimeProperties.SetPropertyValue("MainApp.MainIoUtils.BundlePath", aaBase);
            Debug.Log("InitAddressables 111, BundlePath=" + aaBase);

            var init = Addressables.InitializeAsync();
            init.Completed += op =>
            {
                Debug.Log($"InitAddressables Completed, status={op.Status}");
                if (op.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError("[InitAddressables] FAILED: " + op.OperationException);
                }
            };

            init.WaitForCompletion();   // 先保留，等看到错误信息再改成协程
            Debug.Log("InitAddressables 222");
        }


        public static void LoadKeepNode()
        {
            AsyncOperationHandle<GameObject> keepNodeHandle =
                Addressables.InstantiateAsync("Assets/ResBundle/Prefabs/Persistent/KeepNode.prefab");
            keepNodeHandle.WaitForCompletion();
        }

        private static void ReloadAddressables()
        {
            string catalogFile = MainIoUtils.BundlePath + "/catalog.json";
            Debug.Log(" InitAddressables 111 " + catalogFile);
            if (!File.Exists(catalogFile))
            {
                return;
            }
            Debug.Log(" InitAddressables 2222 " + catalogFile);
            Addressables.ClearResourceLocators();
            AsyncOperationHandle<IResourceLocator> loadLog = Addressables.LoadContentCatalogAsync(catalogFile);
            loadLog.WaitForCompletion();
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