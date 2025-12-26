using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace JO.UIManager
{
    public class UIViewInfo : MonoBehaviour
    {

        public UILayer UILayer = UILayer.FullScreen;

        /*
         *  是否是互斥界面 只针对UILayer.Tip 和 UILayer.Top 
            UILayer.FullScreen 默认互斥，并且同时只会存在一个全屏界面
            UILayer.Pop 每个UILayer.FullScreen 界面可以存在多个 UILayer.Pop 界面，但每个 UILayer.FullScreen 中相同的 UILayer.Pop 界面是互斥的，只会存在一个，不同的 UILayer.FullScreen 界面可以同时存在相同的 UILayer.Pop 界面
            举例子 A 全屏界面，B Pop界面，C Pop界面
            可以存在C.B.A 这样的全在开启的界面，但不会存在 C.C.B.B.A这种堆栈
         */
        public bool isExclusion;

        //界面需要用到的组件组件
        public List<GameObject> uiCom = new List<GameObject>();

        //后续可以添加一些UI的配置信息

        public void AutoInfo()
        {
#if UNITY_EDITOR
            GameObject go = gameObject;

            Canvas canvas = go.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = Undo.AddComponent<Canvas>(go);
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                Undo.RecordObject(go, "Add Canvas");
            }

            GraphicRaycaster raycaster = go.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = Undo.AddComponent<GraphicRaycaster>(go);
                Undo.RecordObject(go, "Add GraphicRaycaster");
            }

            Undo.RecordObject(this, "Collect m_ UI Components");
            this.uiCom.Clear();
            Transform[] allChildren = go.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.name.StartsWith("m_"))
                {
                    if (!uiCom.Contains(child.gameObject))
                    {
                        uiCom.Add(child.gameObject);
                    }
                }
            }
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(gameObject);
#endif
        }
    }
}