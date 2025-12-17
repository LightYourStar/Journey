using UnityEngine;

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
            var prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"Load failed: {path}");
                return ;
            }

            var go = Instantiate(prefab);
            go.name = "KeepNode";
            if (!go.activeSelf) go.SetActive(true);
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