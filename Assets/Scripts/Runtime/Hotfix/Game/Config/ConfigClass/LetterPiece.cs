//----------------------------------------------
//    Auto Generated. DO NOT edit manually!
//----------------------------------------------

using UnityEngine;

namespace JO {

	public partial class LetterPiece : ScriptableObject {

		public static string CfgName = "LetterPiece";

		public static LetterPiece Data{ get { return Global.gApp.gGameData.GetData<LetterPiece>(CfgName); } }
		[SerializeField, HideInInspector]
		private LetterPieceItem[] _Items;
		public LetterPieceItem[] items { get { return _Items; } }

		public LetterPieceItem Get(int id) {
			int min = 0;
			int max = items.Length;
			while (min < max) {
				int index = (min + max) >> 1;
				LetterPieceItem item = _Items[index];
				if (item.id == id) { return item; }
				if (id < item.id) {
					max = index;
				} else {
					min = index + 1;
				}
			}
			UnityEngine.Debug.LogError("LetterPiece表找不到 => " + id);
			return null;
		}

		public bool TryGet(int id, out LetterPieceItem item, bool logError = true) {
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
			if (logError) { UnityEngine.Debug.LogError("LetterPiece表找不到 => " + id); }
			return false;
		}

	}

	[System.Serializable]
	public class LetterPieceItem {

		[SerializeField, HideInInspector]
		private int _Id;
		/// <summary>
		/// 片段id
		/// </summary>
		public int id { get { return _Id; } }

		[SerializeField, HideInInspector]
		private string _Desc;
		/// <summary>
		/// 内容
		/// </summary>
		public string desc { get { return _Desc; } }

		[SerializeField, HideInInspector]
		private int _Dragable;
		/// <summary>
		/// 可拖拽  1表示是，空着表示不可
		/// </summary>
		public int dragable { get { return _Dragable; } }

		public override string ToString() {
			return string.Format("[LetterPieceItem]{{id:{0}, desc:{1}, dragable:{2}}}",
				id, desc, dragable);
		}

		public static implicit operator bool(LetterPieceItem item) {
			return item != null;
		}

	}

}
