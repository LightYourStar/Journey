using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UIManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JO.UIManager
{

    public enum UILayer
    {
        FullScreen = 0, //全屏界面
        Pop = 1, //弹窗界面
        Tip = 2, //Tip界面-不参与界面互斥以及界面堆栈 比如飘字提示。道具获得飘字
        Top = 3, //顶层界面-不参与界面互斥以及界面堆栈  比如loading 界面，引导，网络异常弹窗等
    }



    public class UIMgr
    {

        private const int LayerStep = 20;

        

        private static UIMgr _instance;

        public static UIMgr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UIMgr();
                }

                return _instance;
            }
            protected set { }
        }

        /// <summary>
        ///  id - uiview映射
        /// </summary>
        private Dictionary<int, UIViewBase> uiViewMap = new Dictionary<int, UIViewBase>();

        /// <summary>
        /// 全屏界面所包含的弹出界面映射
        /// </summary>
        private Dictionary<int, List<int>> fullView2PopViewMap = new Dictionary<int, List<int>>();

        /// <summary>
        /// 全屏界面的ID栈
        /// </summary>
        private List<int> fullViewStack = new List<int>();

        /// <summary>
        /// tip界面栈
        /// </summary>
        private List<int> tipViewStack = new List<int>();

        /// <summary>
        /// top界面栈
        /// </summary>
        private List<int> topViewStack = new List<int>();

        /// <summary>
        /// 当前界面的层级
        /// </summary>
        private int curUILayer = 0;
        // Tip层起始值
        public int TipLayBase = 10000;
        // Top层起始值
        public int TopLayBase = 20000;

        private int idGen = 0;


        public static GameObject UIRoot { get; private set; }

        private UIMgr()
        {
            UIConf.InitRegisterScriptType();
        }

        public static void Init()
        {
            //生成UI根节点，以及UI相机
            UIRoot = new GameObject("UIRoot");
            GameObject.DontDestroyOnLoad(UIRoot);
            var canvas = UIRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler =  UIRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            UIRoot.AddComponent<GraphicRaycaster>();
            UIRoot.layer = LayerMask.NameToLayer("UI");
            Instance.curUILayer = 0;
            Instance.uiViewMap.Clear();
            Instance.fullView2PopViewMap.Clear();
            Instance.fullViewStack.Clear();
            Instance.idGen = 0;

            EventSystem eventSys = GameObject.FindFirstObjectByType<EventSystem>();
            if(eventSys == null)
            {
                GameObject eventObj = new GameObject("EventSystem");
                eventSys = eventObj.AddComponent<EventSystem>();
                eventObj.AddComponent<StandaloneInputModule>();
            }
            GameObject.DontDestroyOnLoad(eventSys.gameObject);
        }


        public static void Tick()
        {
            float dt = Time.deltaTime;
            for (int i = 0; i < Instance.fullViewStack.Count; i++)
            {
                int viewId = Instance.fullViewStack[i];
                var uiView = UIMgr.GetUIView(viewId);
                if (uiView != null && uiView.IsShow)
                {
                    if(uiView.IsNeedUpdate)
                        uiView.OnUpdate(dt);
                    if (Instance.fullView2PopViewMap.TryGetValue(viewId, out var popList))
                    {
                        for (int j = 0; j < popList.Count; j++)
                        {
                            int popViewId = popList[j];
                            var popView = UIMgr.GetUIView(popViewId);
                            if (popView != null && popView.IsShow)
                            {
                                if(popView.IsNeedUpdate)
                                    popView.OnUpdate(dt);
                            }
                        }
                    }
                }
            }
        }

        #region 外部接口
        public static UIViewBase GetUIView(int uiId)
        {
            if (Instance.uiViewMap.TryGetValue(uiId, out UIViewBase view))
            {
                return view;
            }
            return null;
        }

        public static UIViewBase GetUIView(string uiName)
        {
            foreach (var uiView in Instance.uiViewMap)
            {
                if (uiView.Value.UIName.Equals(uiName, StringComparison.OrdinalIgnoreCase))
                {
                    return uiView.Value;
                }
            }
            return null;
        }

        public static bool IsUIOpen(string uiName)
        {
            foreach (var uiView in Instance.uiViewMap)
            {
                if (uiView.Value.UIName.Equals(uiName, StringComparison.OrdinalIgnoreCase))
                {
                    return uiView.Value.IsShow;
                }
            }
            return false;
        }

       
        public static void OpenUI(string uiName, object param1 = null, object param2 = null, object param3 = null,
            Action callback = null)
        {
            Instance.TryOpenUI(uiName, param1, param2, param3, callback);
        }

        public static void CloseUI(string uiName)
        {
            Instance.TryCloseUI(uiName);
        }

        #endregion

        #region 开启界面

        private void TryOpenUI(string uiName, object param1 = null, object param2 = null, object param3 = null,
            Action callback = null)
        {
            UILayer layer = UIConf.GetUILayer(uiName);
            if(layer == UILayer.FullScreen || layer == UILayer.Pop)
                OpenFullOrPop(uiName,param1,param2,param3,callback);
            else
            {
                OpenTipOrTop(uiName,param1,param2,param3,callback); 
            }

        }

        #region 进堆栈的界面开启逻辑

        private void OpenFullOrPop(string uiName, object param1 = null, object param2 = null, object param3 = null,
            Action callback = null)
        {
            int viewId = CheckIsHadOpenUIViewTop(uiName);
            if (viewId > 0)
            {
                UIViewToTopShow(viewId, param1, param2, param3, callback);
                return;
            }

            UIViewBase uiview = TryLoadUI(uiName);

            if (uiview == null)
            {
                Debug.LogError($"TryOpenUI error: uiview is null for {uiName}");
                return;
            }

            uiViewMap.Add(uiview.ID, uiview);
            if (uiview.IsFullView)
            {
                int lastFullViewId = -1;
                int allCount = fullViewStack.Count;
                if (allCount > 0)
                {
                    lastFullViewId = fullViewStack[allCount - 1];

                    CloseFullView(lastFullViewId, false);
                }

                SetTopLay(uiview);
                uiview.SetParam(param1, param2, param3, callback);
                fullViewStack.Add(uiview.ID);
                SetViewActive(uiview, true);
            }
            else
            {
                int allCount = fullViewStack.Count;
                if (allCount == 0)
                {
                    Debug.LogError("TryOpenUI error: no full view in stack");
                    return;
                }

                int topFullViewId = fullViewStack[allCount - 1];
                List<int> popList = null;
                if (!fullView2PopViewMap.TryGetValue(topFullViewId, out popList))
                {
                    popList = new List<int>();
                    fullView2PopViewMap.Add(topFullViewId, popList);
                }

                popList.Add(uiview.ID);

                SetTopLay(uiview);
                uiview.SetParam(param1, param2, param3, callback);
                SetViewActive(uiview, true);
            }
        }


        private int CheckIsHadOpenUIViewTop(string uiName)
        {
            int allCount = fullViewStack.Count;
            if (allCount == 0)
            {
                return -1;
            }

            //遍历全屏界面
            for (int i = allCount - 1; i >= 0; i--)
            {
                int viewID = fullViewStack[i];
                if (uiViewMap.TryGetValue(viewID, out UIViewBase view))
                {
                    if (view.UIName.Equals(uiName, StringComparison.OrdinalIgnoreCase))
                    {
                        return viewID;
                    }
                }
            }

            int topFullViewId = fullViewStack[allCount - 1];

            List<int> popViewList = null;
            if (fullView2PopViewMap.TryGetValue(topFullViewId, out popViewList))
            {
                int popCount = popViewList.Count;
                for (int i = popCount - 1; i >= 0; i--)
                {
                    int popViewId = popViewList[i];
                    if (uiViewMap.TryGetValue(popViewId, out UIViewBase popView))
                    {
                        if (popView.UIName.Equals(uiName, StringComparison.OrdinalIgnoreCase))
                        {
                            return popViewId;
                        }
                    }
                }
            }

            return -1;
        }

        private void UIViewToTopShow(int uiviewId, object param1 = null, object param2 = null, object param3 = null,
            Action callback = null)
        {
            var uiview = uiViewMap[uiviewId];
            if (uiview == null)
            {
                Debug.LogError("UIViewToTapShow error: uiview is null");
                return;
            }

            bool isFull = uiview.IsFullView;

            if (isFull)
            {
                int allCount = fullViewStack.Count;
                for (int i = allCount - 1; i >= 0; i--)
                {
                    int viewId = fullViewStack[i];

                    if (viewId == uiviewId)
                    {
                        break;
                    }

                    CloseFullView(viewId);
                }


                List<int> thisChildPopList = null;
                if (fullView2PopViewMap.TryGetValue(uiviewId, out thisChildPopList))
                {
                    int popCount = thisChildPopList.Count;
                    for (int j = popCount - 1; j >= 0; j--)
                    {
                        int popViewId = thisChildPopList[j];
                        if (uiViewMap.TryGetValue(popViewId, out UIViewBase popView))
                        {
                            SetViewActive(popView, false,true);
                        }
                        uiViewMap.Remove(popViewId);
                    }

                    thisChildPopList.Clear();
                }

                uiview.SetParam(param1, param2, param3, callback);
                SetViewActive(uiview, true);
            }
            else
            {
                int allCount = fullViewStack.Count;
                if (allCount == 0)
                {
                    Debug.LogError("UIViewToTapShow error: no full view in stack");
                    return;
                }

                int topFullViewId = fullViewStack[allCount - 1];
                List<int> popList = null;
                if (!fullView2PopViewMap.TryGetValue(topFullViewId, out popList))
                {
                    Debug.LogError("UIViewToTapShow error: no pop list for top full view");
                    return;
                }

                popList.Remove(uiviewId);


                SetTopLay(uiview);
                uiview.SetParam(param1, param2, param3, callback);
                SetViewActive(uiview, true);
                popList.Add(uiviewId);
            }
        }

        public void SetTopLay(string name)
        {
            int viewId = CheckIsHadOpenUIViewTop(name);
            if (viewId > 0)
            {
                var uiview = uiViewMap[viewId];
                if (uiview == null)
                {
                    Debug.LogError("UIViewToTapShow error: uiview is null");
                    return;
                }
                SetTopLay(uiview);
                return;
            }
        }

        /// <summary>
        ///  设置到最顶层显示
        /// </summary>
        /// <param name="view"></param>
        private void SetTopLay(UIViewBase view)
        {

            if (view == null)   
                return;

            if (view.UILayer == UILayer.Tip)
            {
                if (view.UIObj != null)
                {
                    var canvas = view.UIObj.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        canvas.sortingOrder = TipLayBase;
                    }
                }
                TipLayBase += LayerStep;
                return;
            }
            else if (view.UILayer == UILayer.Top)
            {
                if (view.UIObj != null)
                {
                    var canvas = view.UIObj.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        canvas.sortingOrder = TopLayBase;
                    }
                }
                TopLayBase += LayerStep;
                return;
            }

            int curLayer = ++curUILayer * LayerStep;
            if (view.UIObj != null)
            {
                var canvas = view.UIObj.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.sortingOrder = curLayer;
                }
            }
        }

        #endregion


        #region 不进堆栈的界面开启逻辑

        private void OpenTipOrTop(string uiName, object param1 = null, object param2 = null, object param3 = null,
            Action callback = null)
        {
           
            UILayer uILayer = UIConf.GetUILayer(uiName);
            
            UIViewInfo uIViewInfo = Instance.GetUIViewInfoByAsset(uiName);

            if(uIViewInfo == null)
            {
                Debug.LogError($" no uiviewInfo in{uiName}");
                return;
            }
            
            bool isExclusion = uIViewInfo.isExclusion;

            var strack = uILayer == UILayer.Tip ? tipViewStack : topViewStack;

            if (isExclusion)
            {
                int allCount = strack.Count;
                for (int i = allCount - 1; i >= 0; i--)
                {
                    int viewId = strack[i];
                    if (uiViewMap.TryGetValue(viewId, out UIViewBase view))
                    {
                        if (view.UIName.Equals(uiName, StringComparison.OrdinalIgnoreCase))
                        {
                            CloseHadUIview(viewId);
                            break;
                        }
                    }
                }
            }
            UIViewBase uiview = TryLoadUI(uiName);
            if (uiview == null)
            {
                Debug.LogError($"TryOpenUI error: uiview is null for {uiName}");
                return;
            }

            uiViewMap.Add(uiview.ID, uiview);
            SetTopLay(uiview);
            uiview.SetParam(param1, param2, param3, callback);
            strack.Add(uiview.ID);
            SetViewActive(uiview, true);


        }

        #endregion

        private UIViewBase TryLoadUI(string uiName)
        {
            Type scriptType = UIConf.GetScriptType(uiName);
            if (scriptType == null)
            {
                Debug.LogError($"UI Script type not found: {uiName}");
                return null;
            }

            var uiPrefab = LoadUIPrefab(uiName);
            if (uiPrefab == null)
            {
                Debug.LogError($"UI Prefab not found: {uiName}");
                return null;
            }

            UIViewInfo info = uiPrefab.GetComponent<UIViewInfo>();
            if (info == null)
            {
                GameObject.DestroyImmediate(uiPrefab);
                Debug.LogError($"UIViewInfo component not found on prefab: {uiName}");
                return null;
            }


            UIViewBase uiview = Activator.CreateInstance(scriptType) as UIViewBase;

            uiview.SetInfo(++idGen, uiName, uiPrefab);

            uiview.OnInit();

            return uiview;
        }

        private GameObject LoadUIPrefab(string uiName)
        {
            string path = $"Prefabs/UIView/{uiName}";
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"LoadUIPrefab error: prefab is null for path {path}");
                return null;
            }
            prefab = GameObject.Instantiate(prefab);
            prefab.transform.SetParent(UIRoot.transform);
            Canvas canvas = prefab.GetComponent<Canvas>();
            if(canvas == null)
            {
                canvas = prefab.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            RectTransform rectTransform = prefab.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = prefab.AddComponent<RectTransform>();
            }
            //设置锚点全铺
            rectTransform.anchorMin = Vector2.zero; // 锚点左下角 (0,0)
            rectTransform.anchorMax = Vector2.one;  // 锚点右上角 (1,1)
            rectTransform.offsetMin = Vector2.zero; // 左下偏移清零（关键！）
            rectTransform.offsetMax = Vector2.zero; // 右上偏移清零
            rectTransform.localScale = Vector3.one;
            return prefab;
        }

      
        private UIViewInfo GetUIViewInfoByAsset(string uiName)
        {
            string path = $"Prefabs/UIView/{uiName}";
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"LoadUIPrefab error: prefab is null for path {path}");
                return null;
            }
           
            return prefab.GetComponent<UIViewInfo>();
        }

        #endregion

        #region 关闭界面

        private void TryCloseUI(string uiName)
        {
            foreach (var uiView in uiViewMap)
            {
                if (uiView.Value.UIName.Equals(uiName, StringComparison.OrdinalIgnoreCase))
                {
                    CloseHadUIview(uiView.Key);
                    return;
                }
            }
        }

        private void CloseHadUIview(int uiviewId)
        {
            UIViewBase uiview = null;
            if (!uiViewMap.TryGetValue(uiviewId, out uiview))
            {
                Debug.LogError("CloseHadUIview error: uiview is null");
                return;
            }

            UILayer uILayer = UIConf.GetUILayer(uiview.UIName);
            if(uILayer == UILayer.Tip)
            {
                tipViewStack.Remove(uiviewId);
                SetViewActive(uiview, false,true);
                uiViewMap.Remove(uiviewId);
                return;
            }
            else if(uILayer == UILayer.Top)
            {
                topViewStack.Remove(uiviewId);
                SetViewActive(uiview, false,true);
                uiViewMap.Remove(uiviewId);
                return;
            }

            bool isFull = uiview.IsFullView;
            if (isFull)
            {
                CloseFullView(uiviewId);
                int allCount = fullViewStack.Count;
                if (allCount > 0)
                {
                    int topFullViewId = fullViewStack[allCount - 1];
                    if (uiViewMap.TryGetValue(topFullViewId, out UIViewBase topFullView))
                    {
                        if(topFullView.IsShow)
                            return;
                        OpenFullView(topFullViewId);
                    }
                }
            }
            else
            {
                int allCount = fullViewStack.Count;
                if (allCount == 0)
                {
                    Debug.LogError("CloseHadUIview error: no full view in stack");
                    return;
                }

                int topFullViewId = fullViewStack[allCount - 1];
                List<int> popList = null;
                if (!fullView2PopViewMap.TryGetValue(topFullViewId, out popList))
                {
                    Debug.LogError("CloseHadUIview error: no pop list for top full view");
                    return;
                }
                SetViewActive(uiview, false,true);
                popList.Remove(uiviewId);
                uiViewMap.Remove(uiviewId);
            }
        }

        #endregion

        /// <summary>
        /// 关闭全屏界面
        /// </summary>
        /// <param name="fullViewId"></param>
        private void CloseFullView(int fullViewId,bool isRemove = true)
        {
            List<int> childPopList = null;
            if (fullView2PopViewMap.TryGetValue(fullViewId, out childPopList))
            {
                int popCount = childPopList.Count;
                for (int j = popCount - 1; j >= 0; j--)
                {
                    int popViewId = childPopList[j];
                    if (uiViewMap.TryGetValue(popViewId, out UIViewBase popView))
                    {
                        SetViewActive(popView, false,isRemove);
                        if(isRemove)
                            uiViewMap.Remove(popViewId);
                    }
                }
                if(isRemove)
                    childPopList.Clear();
            }

            if (uiViewMap.TryGetValue(fullViewId, out UIViewBase lastView))
            {
                SetViewActive(lastView, false,isRemove);
                if (isRemove)
                {
                    fullViewStack.Remove(fullViewId);
                    uiViewMap.Remove(fullViewId);
                }
            }
        }


        private void OpenFullView(int fullViewId)
        {
            if(fullView2PopViewMap.TryGetValue(fullViewId,out List<int> childPopList))
            {
                int popCount = childPopList.Count;
                for (int j = popCount - 1; j >= 0; j--)
                {
                    int popViewId = childPopList[j];
                    if (uiViewMap.TryGetValue(popViewId, out UIViewBase popView))
                    {
                        SetViewActive(popView, true);
                    }
                }
            }


            if (uiViewMap.TryGetValue(fullViewId, out UIViewBase fullView))
            {
                SetViewActive(fullView, true);
            }
        }

        private void SetViewActive(UIViewBase view, bool isActive, bool isDes = false)
        {
            if (view == null || view.UIObj == null)
            {
                return;
            }

            if (isActive == view.IsShow) return;

            view.UIObj.SetActive(isActive);
            if (isActive)
            { 
                view.OnOpen(view.param1, view.param2, view.param3);
                view.IsShow = true;
            }
            else
            {
                view.OnClose();
                view.IsShow = false;
                if(isDes)
                    GameObject.DestroyImmediate(view.UIObj);
            }
        }

    }
}