//----------------------------------------------
//    Auto Generated. DO NOT edit manually!
//----------------------------------------------

using UnityEngine;

namespace JO {

	public partial class LetterOutCome : ScriptableObject {

		public static string CfgName = "LetterOutCome";

		public static LetterOutCome Data{ get { return Global.gApp.gGameData.GetData<LetterOutCome>(CfgName); } }
		[SerializeField, HideInInspector]
		private LetterOutComeItem[] _Items;
		public LetterOutComeItem[] items { get { return _Items; } }

		public LetterOutComeItem Get(int id) {
			int min = 0;
			int max = items.Length;
			while (min < max) {
				int index = (min + max) >> 1;
				LetterOutComeItem item = _Items[index];
				if (item.id == id) { return item; }
				if (id < item.id) {
					max = index;
				} else {
					min = index + 1;
				}
			}
			UnityEngine.Debug.LogError("LetterOutCome表找不到 => " + id);
			return null;
		}

		public bool TryGet(int id, out LetterOutComeItem item, bool logError = true) {
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
			if (logError) { UnityEngine.Debug.LogError("LetterOutCome表找不到 => " + id); }
			return false;
		}

	}

	[System.Serializable]
	public class LetterOutComeItem {

		[SerializeField, HideInInspector]
		private int _Id;
		/// <summary>
		/// 结局id
		/// </summary>
		public int id { get { return _Id; } }

		[SerializeField, HideInInspector]
		private string _Desc;
		/// <summary>
		/// 内容
		/// </summary>
		public string desc { get { return _Desc; } }

		[SerializeField, HideInInspector]
		private string[] _Rule;
		/// <summary>
		/// 结果匹配
		/// </summary>
		public string[] rule { get { return _Rule; } }

		[SerializeField, HideInInspector]
		private int _Result;
		/// <summary>
		/// 结果
		/// </summary>
		public int result { get { return _Result; } }

		public override string ToString() {
			return string.Format("[LetterOutComeItem]{{id:{0}, desc:{1}, rule:{2}, result:{3}}}",
				id, desc, array2string(rule), result);
		}

		private string array2string(System.Array array) {
			int len = array.Length;
			string[] strs = new string[len];
			for (int i = 0; i < len; i++) {
				strs[i] = string.Format("{0}", array.GetValue(i));
			}
			return string.Concat("[", string.Join(", ", strs), "]");
		}

		public static implicit operator bool(LetterOutComeItem item) {
			return item != null;
		}

	}

}
