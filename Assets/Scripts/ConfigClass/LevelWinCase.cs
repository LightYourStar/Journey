//----------------------------------------------
//    Auto Generated. DO NOT edit manually!
//----------------------------------------------

using UnityEngine;

namespace JO {

	public partial class LevelWinCase : ScriptableObject {

		public static string CfgName = "LevelWinCase";

		public static LevelWinCase Data{ get { return Global.gApp.gGameData.GetData<LevelWinCase>(CfgName); } }
		[SerializeField, HideInInspector]
		private LevelWinCaseItem[] _Items;
		public LevelWinCaseItem[] items { get { return _Items; } }

		public LevelWinCaseItem Get(int id) {
			int min = 0;
			int max = items.Length;
			while (min < max) {
				int index = (min + max) >> 1;
				LevelWinCaseItem item = _Items[index];
				if (item.id == id) { return item; }
				if (id < item.id) {
					max = index;
				} else {
					min = index + 1;
				}
			}
			UnityEngine.Debug.LogError("LevelWinCase表找不到 => " + id);
			return null;
		}

		public bool TryGet(int id, out LevelWinCaseItem item, bool logError = true) {
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
			if (logError) { UnityEngine.Debug.LogError("LevelWinCase表找不到 => " + id); }
			return false;
		}

	}

	[System.Serializable]
	public class LevelWinCaseItem {

		[SerializeField, HideInInspector]
		private int _Id;
		/// <summary>
		/// 关卡ID
		/// </summary>
		public int id { get { return _Id; } }

		[SerializeField, HideInInspector]
		private string _Conditioin;
		/// <summary>
		/// 
		/// </summary>
		public string conditioin { get { return _Conditioin; } }

		public override string ToString() {
			return string.Format("[LevelWinCaseItem]{{id:{0}, conditioin:{1}}}",
				id, conditioin);
		}

		public static implicit operator bool(LevelWinCaseItem item) {
			return item != null;
		}

	}

}
