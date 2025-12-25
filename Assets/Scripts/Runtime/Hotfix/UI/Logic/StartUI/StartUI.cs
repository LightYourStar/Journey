using System;
using JO;
using JO.UIManager;
using UnityEngine;
namespace UIManager
{
    public partial class StartUI
    {
        public override void OnInit()
        {
            m_Btn_Start_Button.onClick.AddListener(ClickStartButton);
            m_Btn_Continue_Button.onClick.AddListener(ClickContinuerButton);
            m_Btn_Exit_Button.onClick.AddListener(ClickExitButton);
        }

        public override void OnOpen(object param1 = null, object param2 = null, object param3 = null)
        {
            SoundMgr.PlayBgm("music_Loginbgm");
        }

        public override void OnClose()
        {
        }

        public override void OnUpdate(float dt)
        {
        }

        /// <summary>
        /// 开始新游戏
        /// </summary>
        private void ClickStartButton()
        {
            DialogueMgr.Instance.NextDialogue();
        }

        /// <summary>
        /// 继续游戏，读取存档
        /// </summary>
        private void ClickContinuerButton()
        {
            DialogueMgr.Instance.StartDialogue();
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        private void ClickExitButton()
        {
            Application.Quit();
        }
    }
}