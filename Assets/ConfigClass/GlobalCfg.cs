//----------------------------------------------
//    Auto Generated. DO NOT edit manually!
//----------------------------------------------

using UnityEngine;

namespace JO {

	public partial class GlobalCfg : ScriptableObject {

		public static string CfgName = "GlobalCfg";

		public static GlobalCfg Data{ get { return Global.gApp.gGameData.GetData<GlobalCfg>(CfgName); } }
		[SerializeField, HideInInspector]
		private GlobalCfgItem[] _Items;
		public GlobalCfgItem[] items { get { return _Items; } }

		public GlobalCfgItem Get(int id) {
			int min = 0;
			int max = items.Length;
			while (min < max) {
				int index = (min + max) >> 1;
				GlobalCfgItem item = _Items[index];
				if (item.id == id) { return item; }
				if (id < item.id) {
					max = index;
				} else {
					min = index + 1;
				}
			}
			UnityEngine.Debug.LogError("GlobalCfg表找不到 => " + id);
			return null;
		}

		public bool TryGet(int id, out GlobalCfgItem item, bool logError = true) {
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
			if (logError) { UnityEngine.Debug.LogError("GlobalCfg表找不到 => " + id); }
			return false;
		}

	}

	[System.Serializable]
	public class GlobalCfgItem {

		[SerializeField, HideInInspector]
		private int _Id;
		/// <summary>
		/// 参数ID
		/// </summary>
		public int id { get { return _Id; } }

		[SerializeField, HideInInspector]
		private string _EntryName;
		/// <summary>
		/// 原key
		/// </summary>
		public string entryName { get { return _EntryName; } }

		[SerializeField, HideInInspector]
		private int _ValueInt;
		/// <summary>
		/// 备注
		/// </summary>
		public int valueInt { get { return _ValueInt; } }

		[SerializeField, HideInInspector]
		private int[] _ValueIntarray;
		/// <summary>
		/// 参数1
		/// </summary>
		public int[] valueIntarray { get { return _ValueIntarray; } }

		[SerializeField, HideInInspector]
		private string _ValueString;
		/// <summary>
		/// 参数2
		/// </summary>
		public string valueString { get { return _ValueString; } }

		[SerializeField, HideInInspector]
		private string[] _ValueStringarray;
		/// <summary>
		/// 参数3
		/// </summary>
		public string[] valueStringarray { get { return _ValueStringarray; } }

		public override string ToString() {
			return string.Format("[GlobalCfgItem]{{id:{0}, entryName:{1}, valueInt:{2}, valueIntarray:{3}, valueString:{4}, valueStringarray:{5}}}",
				id, entryName, valueInt, array2string(valueIntarray), valueString, array2string(valueStringarray));
		}

		private string array2string(System.Array array) {
			int len = array.Length;
			string[] strs = new string[len];
			for (int i = 0; i < len; i++) {
				strs[i] = string.Format("{0}", array.GetValue(i));
			}
			return string.Concat("[", string.Join(", ", strs), "]");
		}

		public static implicit operator bool(GlobalCfgItem item) {
			return item != null;
		}

	}

}
