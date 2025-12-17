using JO.UIManager;
using UnityEngine;
using UnityEngine.UI;
namespace UIManager
{
	public partial class TestUI:UIViewBase
	{
		//自动生成的UI组件代码，请勿手动修改，如需修改请修改对应的Prefab并重新生成
		public RectTransform m_PveBtn_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<RectTransform>();
			}
		}
		public Image m_PveBtn_Image
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<Image>();
			}
		}
		public Button m_PveBtn_Button
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<Button>();
			}
		}
		public RectTransform m_Text_Pve_Name_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[1].GetComponent<RectTransform>();
			}
		}
		public Text m_Text_Pve_Name_Text
		{
			get
			{
				return UIViewInfo.uiCom[1].GetComponent<Text>();
			}
		}
		public RectTransform m_TestMecha_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[2].GetComponent<RectTransform>();
			}
		}
		public Image m_TestMecha_Image
		{
			get
			{
				return UIViewInfo.uiCom[2].GetComponent<Image>();
			}
		}
		public Button m_TestMecha_Button
		{
			get
			{
				return UIViewInfo.uiCom[2].GetComponent<Button>();
			}
		}
		public RectTransform m_xxx_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[3].GetComponent<RectTransform>();
			}
		}
	}
}
