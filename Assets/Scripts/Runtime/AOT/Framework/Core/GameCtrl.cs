using UnityEngine;

namespace JO
{
    public partial class GameCtrl
    {
        //private TouchMask m_GlobalMask;

        public float FrameDtTime { get; private set; }

        public void RemoveGlobalTouchMask()
        {
            /*if (m_GlobalMask != null)
            {
                m_GlobalMask.RemoveRef();
            }*/
        }

        public void ClearGlobalTouchMask()
        {
            /*if (m_GlobalMask != null)
            {
                m_GlobalMask.ClearRef();
            }*/
        }

        public void AddGlobalTouchMask()
        {
            /*if (m_GlobalMask == null)
            {
                GameObject touchMask = Global.gApp.gResMgr.InstantiateLoadObj(LDUICfg.TouchMaskUI, ResSceneType.Resident);
                //todo
                // touchMask.transform.SetParent(Global.gApp.gUiMgr.GetTopCanvasTsf(), false);
                touchMask.transform.SetAsFirstSibling();
                m_GlobalMask = touchMask.GetComponent<TouchMask>();
            }

            m_GlobalMask.AddRef();*/
        }
    }
}