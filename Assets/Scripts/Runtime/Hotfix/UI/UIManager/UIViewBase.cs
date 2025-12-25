using System;
using System.Net.NetworkInformation;
using UnityEngine;
using Object = System.Object;
using UnityEngine.UI;

namespace JO.UIManager
{
    public class UIViewBase
    {
        public int ID { get; set; }
        public string UIName { get; private set; }

        public UIViewInfo UIViewInfo
        {
            get
            {
                return UIObj != null ? UIObj.GetComponent<UIViewInfo>() : null;
            }
            set
            {
                UIObj = value?.gameObject;
            }
        }

        public GameObject UIObj { get; private set; }

        public bool IsFullView => UIViewInfo != null && UIViewInfo.UILayer == UILayer.FullScreen;

        public UILayer UILayer => UIViewInfo != null ? UIViewInfo.UILayer : UILayer.FullScreen;

        public bool IsShow { get; set; } = false;

        public object param1;

        public object param2;

        public object param3;

        public Action OnShowCallBack;

        public bool IsNeedUpdate { get; set; } = false;

        public void Resert()
        {
            UIName = null;
            UIObj = null;
            param1 = null;
            param2 = null;
            param3 = null;
            OnShowCallBack = null;
            IsNeedUpdate = false;
        }

        public void SetInfo(int Id, string uiName, GameObject uiObj)
        {
            ID = Id;
            UIName = uiName;
            UIObj = uiObj;
        }

        public void SetParam(object param1 = null, object param2 = null, object param3 = null, Action onShowCallBack = null)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            OnShowCallBack = onShowCallBack;
        }


        public virtual void OnInit()
        {

        }



        public virtual void OnOpen(object param1 = null, object param2 = null, object param3 = null)
        {

        }

        public virtual void OnClose()
        {

        }

        public virtual void OnUpdate(float dt)
        {

        }

        public void CloseSelf()
        {
            UIMgr.CloseUI(UIName);
        }
    }
}