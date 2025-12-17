using JO.UIManager;
using UnityEngine;
using UnityEngine.UI;
namespace UIManager
{
	public partial class EndUI:UIViewBase
	{
		//自动生成的UI组件代码，请勿手动修改，如需修改请修改对应的Prefab并重新生成
		public RectTransform m_Monologue_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<RectTransform>();
			}
		}
		public Button m_Monologue_Button
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<Button>();
			}
		}
		public RectTransform m_ImgBg_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[1].GetComponent<RectTransform>();
			}
		}
		public Image m_ImgBg_Image
		{
			get
			{
				return UIViewInfo.uiCom[1].GetComponent<Image>();
			}
		}
		public RectTransform m_Text_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[2].GetComponent<RectTransform>();
			}
		}
		public Text m_Text_Text
		{
			get
			{
				return UIViewInfo.uiCom[2].GetComponent<Text>();
			}
		}
		public RectTransform m_CloseButton_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[3].GetComponent<RectTransform>();
			}
		}
		public Image m_CloseButton_Image
		{
			get
			{
				return UIViewInfo.uiCom[3].GetComponent<Image>();
			}
		}
		public Button m_CloseButton_Button
		{
			get
			{
				return UIViewInfo.uiCom[3].GetComponent<Button>();
			}
		}
	}
}
