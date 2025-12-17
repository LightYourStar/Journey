using System;
using JO;
using JO.UIManager;
using UnityEngine;
namespace UIManager
{
	public partial class EndUI
	{
		private BasicTypewriter curText = null;
		public override void OnInit()
		{
		}

		public override void OnOpen(object param1 = null, object param2 = null, object param3 = null)
		{
			m_Monologue_Button.onClick.AddListener(OnClick);
			m_CloseButton_Button.onClick.AddListener(GotoMain);

			EndShowItem endItem = (EndShowItem)param1;
			m_ImgBg_Image.sprite = Global.gApp.gResMgr.LoadSprite(endItem.BgIcon);
			curText = UIUtil.ShowWriterTxt(m_Text_Text.gameObject, endItem.Connect);


        }

		public override void OnClose()
		{
			m_Monologue_Button.onClick.RemoveAllListeners();
			m_CloseButton_Button.onClick.RemoveAllListeners();
        }

		public override void OnUpdate(float dt)
		{
		}

		private void OnClick()
		{
			if(curText && curText.IsAnimating())
			{
				curText.CompleteImmediately();
			}
		}

		private void GotoMain()
		{
			UIMgr.OpenUI(UIConf.StartUI);
		}
	}
}
