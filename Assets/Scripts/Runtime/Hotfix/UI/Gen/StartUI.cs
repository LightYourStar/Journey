using JO.UIManager;
using UnityEngine;
using UnityEngine.UI;
namespace UIManager
{
	public partial class StartUI:UIViewBase
	{
		//自动生成的UI组件代码，请勿手动修改，如需修改请修改对应的Prefab并重新生成
		public RectTransform m_Btn_Exit_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<RectTransform>();
			}
		}
		public Image m_Btn_Exit_Image
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<Image>();
			}
		}
		public Button m_Btn_Exit_Button
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<Button>();
			}
		}
		public RectTransform m_Btn_Start_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[1].GetComponent<RectTransform>();
			}
		}
		public Image m_Btn_Start_Image
		{
			get
			{
				return UIViewInfo.uiCom[1].GetComponent<Image>();
			}
		}
		public Button m_Btn_Start_Button
		{
			get
			{
				return UIViewInfo.uiCom[1].GetComponent<Button>();
			}
		}
		public RectTransform m_Btn_Continue_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[2].GetComponent<RectTransform>();
			}
		}
		public Image m_Btn_Continue_Image
		{
			get
			{
				return UIViewInfo.uiCom[2].GetComponent<Image>();
			}
		}
		public Button m_Btn_Continue_Button
		{
			get
			{
				return UIViewInfo.uiCom[2].GetComponent<Button>();
			}
		}
	}
}
