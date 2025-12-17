using System.Collections;
using UnityEngine;

namespace JO
{
    public class App
    {
        public GameObject gKeepNode;

        public ResMgr gResMgr;
        public Global gGlobal;
        private Global m_Global;
        public GameCtrl gGameCtrl;
        public GameDatas gGameData;
        public MsgDispatcher gMsgDispatcher;
        public SceneMgr gSceneMgr;

        public void Awake(Global global, GameObject keepNode)
        {
            m_Global = global;
            InitNode(keepNode);
            gResMgr = new ResMgr();

            m_Global.StartCoroutine(PreInit());
            // OnTestChangeScene();
        }

        private static void OnCallBack()
        {
            Debug.Log("OnClickChangeScene  2222");
        }

        private void InitNode(GameObject keepNode)
        {
            gKeepNode = keepNode;
            gGlobal = keepNode.transform.Find("Global").GetComponent<Global>();
        }

        private IEnumerator PreInit()
        {
            gGameData = new GameDatas();

            yield return gGameData.WaitOnInitSucceed();

            gGameCtrl = new GameCtrl();
            gMsgDispatcher = new MsgDispatcher();
            gSceneMgr = new SceneMgr();
            gSceneMgr.InitCurrentScene();
        }

        public void DUpdate(float dt)
        {
        }

        public void OnDestroy()
        {
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}