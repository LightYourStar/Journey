using JO.UIManager;
using UIManager;
using UnityEngine;

namespace JO
{
    public class Global : MonoBehaviour
    {
        public static App gApp = new App();
        public static bool ShowLog = true;

        void Awake()
        {
            DontDestroyOnLoad(gameObject.transform.parent.gameObject);
            gApp.Awake(this, gameObject.transform.parent.gameObject);
            UIMgr.Init();
            UIMgr.OpenUI(UIConf.StartUI);
        }

        private void Update()
        {
            float dtTime = Time.deltaTime;
            if (gApp != null)
            {
                gApp.DUpdate(dtTime);
                UIMgr.Tick();
            }
        }

        private void OnDestroy()
        {
#if (UNITY_EDITOR)
            if (gApp != null)
            {
                gApp.OnDestroy();
            }

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
#endif
        }
    }
}