//----------------------------------------------
//    Auto Generated. DO NOT edit manually!
//----------------------------------------------

using UnityEngine;

namespace JO {

	public partial class LevelMain : ScriptableObject {

		public static string CfgName = "LevelMain";

		public static LevelMain Data{ get { return Global.gApp.gGameData.GetData<LevelMain>(CfgName); } }
		[SerializeField, HideInInspector]
		private LevelMainItem[] _Items;
		public LevelMainItem[] items { get { return _Items; } }

		public LevelMainItem Get(int id) {
			int min = 0;
			int max = items.Length;
			while (min < max) {
				int index = (min + max) >> 1;
				LevelMainItem item = _Items[index];
				if (item.id == id) { return item; }
				if (id < item.id) {
					max = index;
				} else {
					min = index + 1;
				}
			}
			UnityEngine.Debug.LogError("LevelMain表找不到 => " + id);
			return null;
		}

		public bool TryGet(int id, out LevelMainItem item, bool logError = true) {
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
			if (logError) { UnityEngine.Debug.LogError("LevelMain表找不到 => " + id); }
			return false;
		}

	}

	[System.Serializable]
	public class LevelMainItem {

		[SerializeField, HideInInspector]
		private int _Id;
		/// <summary>
		/// 关卡ID
		/// </summary>
		public int id { get { return _Id; } }

		[SerializeField, HideInInspector]
		private string[] _ShowStr;
		/// <summary>
		/// 显示文字
		/// </summary>
		public string[] showStr { get { return _ShowStr; } }

		[SerializeField, HideInInspector]
		private int[] _WinCondition;
		/// <summary>
		/// 获胜条件
		/// </summary>
		public int[] winCondition { get { return _WinCondition; } }

		[SerializeField, HideInInspector]
		private int _GridRows;
		/// <summary>
		/// 关卡网格行
		/// </summary>
		public int gridRows { get { return _GridRows; } }

		[SerializeField, HideInInspector]
		private int _GridCols;
		/// <summary>
		/// 关卡网格列
		/// </summary>
		public int gridCols { get { return _GridCols; } }

		[SerializeField, HideInInspector]
		private string[] _TargetPos;
		/// <summary>
		/// 推到位置的坐标
		/// </summary>
		public string[] targetPos { get { return _TargetPos; } }

		public override string ToString() {
			return string.Format("[LevelMainItem]{{id:{0}, showStr:{1}, winCondition:{2}, gridRows:{3}, gridCols:{4}, targetPos:{5}}}",
				id, array2string(showStr), array2string(winCondition), gridRows, gridCols, array2string(targetPos));
		}

		private string array2string(System.Array array) {
			int len = array.Length;
			string[] strs = new string[len];
			for (int i = 0; i < len; i++) {
				strs[i] = string.Format("{0}", array.GetValue(i));
			}
			return string.Concat("[", string.Join(", ", strs), "]");
		}

		public static implicit operator bool(LevelMainItem item) {
			return item != null;
		}

	}

}
