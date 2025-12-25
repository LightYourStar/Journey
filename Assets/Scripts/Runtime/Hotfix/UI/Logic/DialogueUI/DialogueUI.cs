using JO;
using JO.UIManager;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace UIManager
{
	public partial class DialogueUI
	{


		private DelayedCallback delayedCallback = new DelayedCallback();

        /// <summary>
        /// 当前对话类型
        /// </summary>
        public DialogueType curType = DialogueType.None;


		public BasicTypewriter curText = null;


		private List<GameObject> selets = new List<GameObject>();

		public override void OnInit()
		{
			m_Selet1_RectTransform.gameObject.SetActive(false);
			m_Selet2_RectTransform.gameObject.SetActive(false);
			
        }

		public override void OnOpen(object param1 = null, object param2 = null, object param3 = null)
		{
			AddListeners();

            HideAllUI();
        }

		public override void OnClose()
		{
			RemoveListeners();
            delayedCallback.Cancel();
        }

		public override void OnUpdate(float dt)
		{
		}

		private void AddListeners()
		{
			m_Monologue_Button.onClick.AddListener(ClickMonologue);
			m_LeftContent_Button.onClick.AddListener(ClickMonologue);
			m_RightContent_Button.onClick.AddListener(ClickMonologue);
			m_Avg_Button.onClick.AddListener(ClickMonologue);
            EventMgr.AddEvent(EventConf.RefreDialogueInfo, OnRefreDialogueInfo);
			EventMgr.AddEvent(EventConf.HideAllDialogue, HideAllUI);

        }

        private void RemoveListeners()
		{
			m_Monologue_Button.onClick.RemoveAllListeners();
			m_LeftContent_Button.onClick.RemoveAllListeners();
			m_RightContent_Button.onClick.RemoveAllListeners();
			m_Avg_Button.onClick.RemoveAllListeners();
            EventMgr.RemoveEvent(EventConf.RefreDialogueInfo, OnRefreDialogueInfo);
            EventMgr.RemoveEvent(EventConf.HideAllDialogue, HideAllUI);
        }

        #region 刷新显示

        /// <summary>
        /// 刷新剧情显示
        /// </summary>
        /// <param name="obj"></param>
        private void OnRefreDialogueInfo(params object[] param)
		{
            RsetLast();

            int curId = (int)param[0];
			int lastId = (int)param[1];
			bool isNeedRefreshLast = (bool)param[2];
			if(isNeedRefreshLast)
			{
				if(lastId > 0)
				{
                    RefreshUI(lastId, true);
                }
            }

			RefreshUI(curId,false);

        }

		private void RefreshUI(int curId,bool isImme)
		{
            var cfg = DialogueMgr.Instance.GetDialogueCfgById(curId);
			
			if(!string.IsNullOrEmpty(cfg.BGM ))
			{
				SoundMgr.PlayBgm(cfg.BGM);
			}
			delayedCallback.Cancel();

            if (!string.IsNullOrEmpty(cfg.SpeekSound))
			{
				delayedCallback.SetTimeout(TimeSpan.FromSeconds(cfg.SoudTime / 1000f), () =>
				{
					SoundMgr.PlaySound(cfg.SpeekSound);
				});
            }
            curType = (DialogueType)cfg.ShowType;
            switch (curType)
            {
                //独白
                case DialogueType.Monologue:
                    RefreshMonologueUI(cfg);
                    break;
                //对话
                case DialogueType.Dialogue:
                    RefreshDialogueUI(cfg);
                    break;
                //选项
                case DialogueType.Seletion:
                    RefreshChoiceUI(cfg);
                    break;
                //小游戏
                case DialogueType.Game:
                    RefreshGame(cfg);
                    break;
                default:
                    break;

            }

        }


        /// <summary>
        /// 隐藏所有UI
        /// </summary>
        private void HideAllUI(params object[] param)
		{
			//独白
			m_Monologue_RectTransform.gameObject.SetActive(false);
			//avg
			m_Avg_RectTransform.gameObject.SetActive(false);
			m_ContentPart_Selet_RectTransform.gameObject.SetActive(false);
			m_LeftContent_RectTransform.gameObject.SetActive(false);
			m_RightContent_RectTransform.gameObject.SetActive(false);
			m_LeftRole_RectTransform.gameObject.SetActive(false);
			m_RightRole_RectTransform.gameObject.SetActive(false);
			curText = null;
        }

		private void RsetLast()
		{
			curText = null;
			for (int i = 0; i < selets.Count; i++)
			{
				selets[i].SetActive(false);
            }
        }


        /// <summary>
        /// 刷新旁白
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="lastId"></param>
        private void RefreshMonologueUI(DialogueItem cfg)
		{
			m_Monologue_RectTransform.gameObject.SetActive(true);
			m_Avg_RectTransform.gameObject.SetActive(false);
            m_ImgBg_Image.sprite = Global.gApp.gResMgr.LoadSprite(cfg.BgIcon);
            curText = UIUtil.ShowWriterTxt(m_Text_Text.gameObject, cfg.Connect);

        }

		/// <summary>
		/// 刷新对话
		/// </summary>
		/// <param name="cfg"></param>
		/// <param name="lastId"></param>
		private void RefreshDialogueUI(DialogueItem cfg)
		{
			m_Monologue_RectTransform.gameObject.SetActive(false);
			m_Avg_RectTransform.gameObject.SetActive(true);

			m_ContentPart_Selet_RectTransform.gameObject.SetActive(false);

            m_AvgBg_Image.gameObject.SetActive(!string.IsNullOrEmpty(cfg.BgIcon));
			if (!string.IsNullOrEmpty(cfg.BgIcon))
			{
            m_AvgBg_Image.sprite = Global.gApp.gResMgr.LoadSprite(cfg.BgIcon);
            }

            bool isHadLeft = !string.IsNullOrEmpty(cfg.LeftRole);
            m_LeftRole_RectTransform.gameObject.SetActive(isHadLeft);
			if(isHadLeft)
			{
				m_LeftRole_Image.sprite = Global.gApp.gResMgr.LoadSprite(cfg.LeftRole);
            }

			bool isHadRight = !string.IsNullOrEmpty(cfg.RightRole);
            m_RightRole_RectTransform.gameObject.SetActive(isHadRight);
			if (isHadRight)
			{
				m_RightRole_Image.sprite = Global.gApp.gResMgr.LoadSprite(cfg.RightRole);
            }

			m_LeftContent_RectTransform.gameObject.SetActive(cfg.Speek == (int)DialogueSpeekType.Left);

			m_RightContent_RectTransform.gameObject.SetActive(cfg.Speek == (int)DialogueSpeekType.Right);

			if(cfg.Speek == (int)DialogueSpeekType.Left)
			{
				curText = UIUtil.ShowWriterTxt(m_LeftContentText_Text.gameObject, cfg.Connect);
            }
			else if(cfg.Speek == (int)DialogueSpeekType.Right)
			{
				curText = UIUtil.ShowWriterTxt(m_RightContentText_Text.gameObject, cfg.Connect);
            }
        }

		/// <summary>
		/// 刷新选择框
		/// </summary>
		/// <param name="cfg"></param>
		/// <param name="lastId"></param>
		private void RefreshChoiceUI(DialogueItem cfg)
		{
			curText = null;
			m_Monologue_RectTransform.gameObject.SetActive(false);
			m_Avg_RectTransform.gameObject.SetActive(true);
			m_ContentPart_Selet_RectTransform.gameObject.SetActive(true);


			for (int i = 0; i < cfg.Selets.Length; i++)
			{
				int curSelet = i;
				if(i <  selets.Count)
				{
					var go = selets[i];
					go.SetActive(true);
					go.transform.SetAsLastSibling();
					Button button = go.GetComponent<Button>();
					Text text = go.GetComponentInChildren<Text>();
					text.text = cfg.Selets[curSelet];
					button.onClick.RemoveAllListeners();
					button.onClick.AddListener(() =>
					{
						DialogueMgr.Instance.OnSelectChoice(cfg.SeletJumpId[curSelet], cfg.SeletAdd[curSelet]);
					});
                }
				else
				{
					var seleCell = i % 2 == 0 ? m_Selet1_RectTransform : m_Selet2_RectTransform;
                    var go = GameObject.Instantiate<GameObject>(seleCell.gameObject, m_ContentPart_Selet_RectTransform);
					selets.Add(go);
                    go.SetActive(true);
                    go.transform.SetAsLastSibling();
                    Button button = go.GetComponent<Button>();
                    Text text = go.GetComponentInChildren<Text>();
                    text.text = cfg.Selets[curSelet];
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                    {
                        DialogueMgr.Instance.OnSelectChoice(cfg.SeletJumpId[curSelet], cfg.SeletAdd[curSelet]);
                    });
                }
			}
        }

		/// <summary>
		/// 刷新小游戏
		/// </summary>
		/// <param name="cfg"></param>
		/// <param name="lastId"></param>
		private void RefreshGame(DialogueItem cfg)
		{
            curText = null;
            //小游戏暂未实现
			DialogueMgr.Instance.GotoGame((GameType)cfg.GameType, cfg.GameId,cfg.id);
        }

        #endregion

        #region 按钮事件


		private void ClickMonologue()
		{
			NextShow();
        }



		private void NextShow()
		{
			if(curType == DialogueType.Monologue || curType == DialogueType.Dialogue)
			{
                if (curText != null && curText.IsAnimating())
                {
                    curText.CompleteImmediately();
                }
                else
                {
                    DialogueMgr.Instance.CheckIsFinish();
                }
            }
        }
        #endregion

        #region 延迟回调

      

        #endregion
    }
}
