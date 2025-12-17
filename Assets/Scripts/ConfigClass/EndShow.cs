//----------------------------------------------
//    Auto Generated. DO NOT edit manually!
//----------------------------------------------

using UnityEngine;

namespace JO {

	public partial class EndShow : ScriptableObject {

		public static string CfgName = "EndShow";

		public static EndShow Data{ get { return Global.gApp.gGameData.GetData<EndShow>(CfgName); } }
		[SerializeField, HideInInspector]
		private EndShowItem[] _Items;
		public EndShowItem[] items { get { return _Items; } }

		public EndShowItem Get(int id) {
			int min = 0;
			int max = items.Length;
			while (min < max) {
				int index = (min + max) >> 1;
				EndShowItem item = _Items[index];
				if (item.id == id) { return item; }
				if (id < item.id) {
					max = index;
				} else {
					min = index + 1;
				}
			}
			UnityEngine.Debug.LogError("EndShow表找不到 => " + id);
			return null;
		}

		public bool TryGet(int id, out EndShowItem item, bool logError = true) {
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
			if (logError) { UnityEngine.Debug.LogError("EndShow表找不到 => " + id); }
			return false;
		}

	}

	[System.Serializable]
	public class EndShowItem {

		[SerializeField, HideInInspector]
		private int _Id;
		/// <summary>
		/// 结局id
		/// </summary>
		public int id { get { return _Id; } }

		[SerializeField, HideInInspector]
		private string _BgIcon;
		/// <summary>
		/// 背景ICON
		/// </summary>
		public string BgIcon { get { return _BgIcon; } }

		[SerializeField, HideInInspector]
		private string _Connect;
		/// <summary>
		/// 对话内容
		/// </summary>
		public string Connect { get { return _Connect; } }

		[SerializeField, HideInInspector]
		private int[] _Value;
		/// <summary>
		/// 需求值
		/// </summary>
		public int[] Value { get { return _Value; } }

		public override string ToString() {
			return string.Format("[EndShowItem]{{id:{0}, BgIcon:{1}, Connect:{2}, Value:{3}}}",
				id, BgIcon, Connect, array2string(Value));
		}

		private string array2string(System.Array array) {
			int len = array.Length;
			string[] strs = new string[len];
			for (int i = 0; i < len; i++) {
				strs[i] = string.Format("{0}", array.GetValue(i));
			}
			return string.Concat("[", string.Join(", ", strs), "]");
		}

		public static implicit operator bool(EndShowItem item) {
			return item != null;
		}

	}

}
