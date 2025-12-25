//----------------------------------------------
//    Auto Generated. DO NOT edit manually!
//----------------------------------------------

using UnityEngine;

namespace JO {

	public partial class Dialogue : ScriptableObject {

		public static string CfgName = "Dialogue";

		public static Dialogue Data{ get { return Global.gApp.gGameData.GetData<Dialogue>(CfgName); } }
		[SerializeField, HideInInspector]
		private DialogueItem[] _Items;
		public DialogueItem[] items { get { return _Items; } }

		public DialogueItem Get(int id) {
			int min = 0;
			int max = items.Length;
			while (min < max) {
				int index = (min + max) >> 1;
				DialogueItem item = _Items[index];
				if (item.id == id) { return item; }
				if (id < item.id) {
					max = index;
				} else {
					min = index + 1;
				}
			}
			UnityEngine.Debug.LogError("Dialogue表找不到 => " + id);
			return null;
		}

		public bool TryGet(int id, out DialogueItem item, bool logError = true) {
			int min = 0;
			int max = items.Length;
			while (min < max) {
				int index = (min + max) >> 1;
				item = _Items[index];
				if (item.id == id) { return true; }
				if (id < item.id) {
					max = index;
				} else {
					min = index + 1;
				}
			}
			item = null;
			if (logError) { UnityEngine.Debug.LogError("Dialogue表找不到 => " + id); }
			return false;
		}

	}

	[System.Serializable]
	public class DialogueItem {

		[SerializeField, HideInInspector]
		private int _Id;
		/// <summary>
		/// 剧情Id
		/// </summary>
		public int id { get { return _Id; } }

		[SerializeField, HideInInspector]
		private string _BgIcon;
		/// <summary>
		/// 剧情背景Icon
		/// </summary>
		public string BgIcon { get { return _BgIcon; } }

		[SerializeField, HideInInspector]
		private int _ShowType;
		/// <summary>
		/// 2
		/// </summary>
		public int ShowType { get { return _ShowType; } }

		[SerializeField, HideInInspector]
		private string _BGM;
		/// <summary>
		///  小游戏
		/// </summary>
		public string BGM { get { return _BGM; } }

		[SerializeField, HideInInspector]
		private string _Connect;
		/// <summary>
		/// 对话内容
		/// </summary>
		public string Connect { get { return _Connect; } }

		[SerializeField, HideInInspector]
		private int[] _NextId;
		/// <summary>
		/// 下一句ID
		/// </summary>
		public int[] NextId { get { return _NextId; } }

		[SerializeField, HideInInspector]
		private int[] _NextIdNeed;
		/// <summary>
		/// 下一句ID需求值
		/// </summary>
		public int[] NextIdNeed { get { return _NextIdNeed; } }

		[SerializeField, HideInInspector]
		private int _IsFinish;
		/// <summary>
		/// 是否是结束
		/// </summary>
		public int IsFinish { get { return _IsFinish; } }

		[SerializeField, HideInInspector]
		private string _ConnectName;
		/// <summary>
		/// 说话者名称
		/// </summary>
		public string ConnectName { get { return _ConnectName; } }

		[SerializeField, HideInInspector]
		private int _Speek;
		/// <summary>
		/// 说话者类型
		/// </summary>
		public int Speek { get { return _Speek; } }

		[SerializeField, HideInInspector]
		private string _RightRole;
		/// <summary>
		/// 右边角色立绘
		/// </summary>
		public string RightRole { get { return _RightRole; } }

		[SerializeField, HideInInspector]
		private string _LeftRole;
		/// <summary>
		/// 左边角色立绘
		/// </summary>
		public string LeftRole { get { return _LeftRole; } }

		[SerializeField, HideInInspector]
		private string _SpeekSound;
		/// <summary>
		/// 对话音效
		/// </summary>
		public string SpeekSound { get { return _SpeekSound; } }

		[SerializeField, HideInInspector]
		private int _SoudTime;
		/// <summary>
		/// 对话音效播放时间（毫秒）
		/// </summary>
		public int SoudTime { get { return _SoudTime; } }

		[SerializeField, HideInInspector]
		private string[] _Selets;
		/// <summary>
		/// 选项
		/// </summary>
		public string[] Selets { get { return _Selets; } }

		[SerializeField, HideInInspector]
		private int[] _SeletJumpId;
		/// <summary>
		/// 选项跳转剧情Id
		/// </summary>
		public int[] SeletJumpId { get { return _SeletJumpId; } }

		[SerializeField, HideInInspector]
		private int[] _SeletAdd;
		/// <summary>
		/// 选项增加值
		/// </summary>
		public int[] SeletAdd { get { return _SeletAdd; } }

		[SerializeField, HideInInspector]
		private int _GameType;
		/// <summary>
		/// 小游戏类型
		/// </summary>
		public int GameType { get { return _GameType; } }

		[SerializeField, HideInInspector]
		private int _GameId;
		/// <summary>
		/// 小游戏ID
		/// </summary>
		public int GameId { get { return _GameId; } }

		[SerializeField, HideInInspector]
		private int[] _GameResult;
		/// <summary>
		/// 小游戏结果对应对话id
		/// </summary>
		public int[] GameResult { get { return _GameResult; } }

		public override string ToString() {
			return string.Format("[DialogueItem]{{id:{0}, BgIcon:{1}, ShowType:{2}, BGM:{3}, Connect:{4}, NextId:{5}, NextIdNeed:{6}, IsFinish:{7}, ConnectName:{8}, Speek:{9}, RightRole:{10}, LeftRole:{11}, SpeekSound:{12}, SoudTime:{13}, Selets:{14}, SeletJumpId:{15}, SeletAdd:{16}, GameType:{17}, GameId:{18}, GameResult:{19}}}",
				id, BgIcon, ShowType, BGM, Connect, array2string(NextId), array2string(NextIdNeed), IsFinish, ConnectName, Speek, RightRole, LeftRole, SpeekSound, SoudTime, array2string(Selets), array2string(SeletJumpId), array2string(SeletAdd), GameType, GameId, array2string(GameResult));
		}

		private string array2string(System.Array array) {
			int len = array.Length;
			string[] strs = new string[len];
			for (int i = 0; i < len; i++) {
				strs[i] = string.Format("{0}", array.GetValue(i));
			}
			return string.Concat("[", string.Join(", ", strs), "]");
		}

		public static implicit operator bool(DialogueItem item) {
			return item != null;
		}

	}

}
