//----------------------------------------------
//    Auto Generated. DO NOT edit manually!
//----------------------------------------------

using UnityEngine;

namespace JO {

	public partial class LetterColumn : ScriptableObject {

		public static string CfgName = "LetterColumn";

		public static LetterColumn Data{ get { return Global.gApp.gGameData.GetData<LetterColumn>(CfgName); } }
		[SerializeField, HideInInspector]
		private LetterColumnItem[] _Items;
		public LetterColumnItem[] items { get { return _Items; } }

		public LetterColumnItem Get(int id) {
			int min = 0;
			int max = items.Length;
			while (min < max) {
				int index = (min + max) >> 1;
				LetterColumnItem item = _Items[index];
				if (item.id == id) { return item; }
				if (id < item.id) {
					max = index;
				} else {
					min = index + 1;
				}
			}
			UnityEngine.Debug.LogError("LetterColumn表找不到 => " + id);
			return null;
		}

		public bool TryGet(int id, out LetterColumnItem item, bool logError = true) {
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
			if (logError) { UnityEngine.Debug.LogError("LetterColumn表找不到 => " + id); }
			return false;
		}

	}

	[System.Serializable]
	public class LetterColumnItem {

		[SerializeField, HideInInspector]
		private int _Id;
		/// <summary>
		/// 信件列ID
		/// </summary>
		public int id { get { return _Id; } }

		[SerializeField, HideInInspector]
		private int[] _SlotInfo;
		/// <summary>
		/// 位置信息:  【碎片Id】
		/// </summary>
		public int[] slotInfo { get { return _SlotInfo; } }

		public override string ToString() {
			return string.Format("[LetterColumnItem]{{id:{0}, slotInfo:{1}}}",
				id, array2string(slotInfo));
		}

		private string array2string(System.Array array) {
			int len = array.Length;
			string[] strs = new string[len];
			for (int i = 0; i < len; i++) {
				strs[i] = string.Format("{0}", array.GetValue(i));
			}
			return string.Concat("[", string.Join(", ", strs), "]");
		}

		public static implicit operator bool(LetterColumnItem item) {
			return item != null;
		}

	}

}
