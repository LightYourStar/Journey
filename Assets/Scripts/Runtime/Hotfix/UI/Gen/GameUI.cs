using JO.UIManager;
using UnityEngine;
using UnityEngine.UI;
namespace UIManager
{
	public partial class GameUI:UIViewBase
	{
		//自动生成的UI组件代码，请勿手动修改，如需修改请修改对应的Prefab并重新生成
		public RectTransform m_Grid_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<RectTransform>();
			}
		}
		public RectTransform m_Item_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[1].GetComponent<RectTransform>();
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
		public RectTransform m_Img_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[3].GetComponent<RectTransform>();
			}
		}
		public Image m_Img_Image
		{
			get
			{
				return UIViewInfo.uiCom[3].GetComponent<Image>();
			}
		}
		public RectTransform m_play_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[4].GetComponent<RectTransform>();
			}
		}
		public Image m_play_Image
		{
			get
			{
				return UIViewInfo.uiCom[4].GetComponent<Image>();
			}
		}
	}
}
