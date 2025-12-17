using JO.UIManager;
using UnityEngine;
using UnityEngine.UI;
namespace UIManager
{
	public partial class LoadingUI:UIViewBase
	{
		//自动生成的UI组件代码，请勿手动修改，如需修改请修改对应的Prefab并重新生成
		public RectTransform m_Slider_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<RectTransform>();
			}
		}
		public Slider m_Slider_Slider
		{
			get
			{
				return UIViewInfo.uiCom[0].GetComponent<Slider>();
			}
		}
		public RectTransform m_Progress_RectTransform
		{
			get
			{
				return UIViewInfo.uiCom[1].GetComponent<RectTransform>();
			}
		}
		public Text m_Progress_Text
		{
			get
			{
				return UIViewInfo.uiCom[1].GetComponent<Text>();
			}
		}
	}
}
