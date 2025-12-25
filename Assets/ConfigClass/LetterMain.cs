//----------------------------------------------
//    Auto Generated. DO NOT edit manually!
//----------------------------------------------

using UnityEngine;

namespace JO {

	public partial class LetterMain : ScriptableObject {

		public static string CfgName = "LetterMain";

		public static LetterMain Data{ get { return Global.gApp.gGameData.GetData<LetterMain>(CfgName); } }
		[SerializeField, HideInInspector]
		private LetterMainItem[] _Items;
		public LetterMainItem[] items { get { return _Items; } }

		public LetterMainItem Get(int id) {
			int min = 0;
			int max = items.Length;
			while (min < max) {
				int index = (min + max) >> 1;
				LetterMainItem item = _Items[index];
				if (item.id == id) { return item; }
				if (id < item.id) {
					max = index;
				} else {
					min = index + 1;
				}
			}
			UnityEngine.Debug.LogError("LetterMain表找不到 => " + id);
			return null;
		}

		public bool TryGet(int id, out LetterMainItem item, bool logError = true) {
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
			if (logError) { UnityEngine.Debug.LogError("LetterMain表找不到 => " + id); }
			return false;
		}

	}

	[System.Serializable]
	public class LetterMainItem {

		[SerializeField, HideInInspector]
		private int _Id;
		/// <summary>
		/// 信件ID
		/// </summary>
		public int id { get { return _Id; } }

		[SerializeField, HideInInspector]
		private int[] _ColumnIds;
		/// <summary>
		/// 信件列Id
		/// </summary>
		public int[] columnIds { get { return _ColumnIds; } }

		[SerializeField, HideInInspector]
		private int[] _OutcomeIds;
		/// <summary>
		/// 结局Id
		/// </summary>
		public int[] outcomeIds { get { return _OutcomeIds; } }

		[SerializeField, HideInInspector]
		private int _DefaultOutcomeId;
		/// <summary>
		/// 默认结局Id
		/// </summary>
		public int defaultOutcomeId { get { return _DefaultOutcomeId; } }

		[SerializeField, HideInInspector]
		private int _HintOutcomeId;
		/// <summary>
		/// 提示结局Id
		/// </summary>
		public int hintOutcomeId { get { return _HintOutcomeId; } }

		public override string ToString() {
			return string.Format("[LetterMainItem]{{id:{0}, columnIds:{1}, outcomeIds:{2}, defaultOutcomeId:{3}, hintOutcomeId:{4}}}",
				id, array2string(columnIds), array2string(outcomeIds), defaultOutcomeId, hintOutcomeId);
		}

		private string array2string(System.Array array) {
			int len = array.Length;
			string[] strs = new string[len];
			for (int i = 0; i < len; i++) {
				strs[i] = string.Format("{0}", array.GetValue(i));
			}
			return string.Concat("[", string.Join(", ", strs), "]");
		}

		public static implicit operator bool(LetterMainItem item) {
			return item != null;
		}

	}

}
