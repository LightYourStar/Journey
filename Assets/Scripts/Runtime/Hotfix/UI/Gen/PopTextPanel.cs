using JO.UIManager;
using UnityEngine;
using UnityEngine.UI;
namespace UIManager
{
	public partial class PopTextPanel:UIViewBase
	{
		//自动生成的UI组件代码，请勿手动修改，如需修改请修改对应的Prefab并重新生成
		public RectTransform m_Button_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<RectTransform>();
			}
		}
		public Image m_Button_Image
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<Image>();
			}
		}
		public Button m_Button_Button
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<Button>();
			}
		}
		public RectTransform m_Text_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[1].GetComponent<RectTransform>();
			}
		}
		public Text m_Text_Text
		{
			get
			{
				return UIViewInfo.uiCom[1].GetComponent<Text>();
			}
		}
		public RectTransform m_Button2_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[2].GetComponent<RectTransform>();
			}
		}
		public Image m_Button2_Image
		{
			get
			{
				return UIViewInfo.uiCom[2].GetComponent<Image>();
			}
		}
		public Button m_Button2_Button
		{
			get
			{
				return UIViewInfo.uiCom[2].GetComponent<Button>();
			}
		}
	}
}
