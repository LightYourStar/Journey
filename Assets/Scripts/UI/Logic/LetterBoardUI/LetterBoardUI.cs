using System.Collections.Generic;
using JO;
using JO.UIManager;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using System.Threading.Tasks;

namespace UIManager
{
    public partial class LetterBoardUI
    {
        private LetterMainItem m_LetterMainCfg;
        private Dictionary<int, RectTransform> m_ColumnMap = new();

        private Dictionary<int, List<FragmentSlot>> m_ColumnSlots = new();

        private int m_LetterId;
        private int m_QuickPassTime = 10;

        public override void OnOpen(object param1 = null, object param2 = null, object param3 = null)
        {
            m_LetterId = param1 != null ? (int)param1 : 201;
            m_LetterMainCfg = LetterMain.Data.Get(m_LetterId);

            SetBgShow();
            RefreshColumnShow();

            BuildSlotsAndPieceInfo();

            InitBtns();

            EventMgr.AddEvent(EventConf.DragSlotNotify, OnDragSlotNotify);
            EventMgr.AddEvent(EventConf.DragSlotResetNotify, OnDragSlotResetNotify);
        }

        private void SetBgShow()
        {
            int colCount = m_LetterMainCfg.columnIds.Length;
            m_Bg1_Image.gameObject.SetActive(colCount == 1);
            m_Bg2_Image.gameObject.SetActive(colCount > 1);
        }

        private void RefreshColumnShow()
        {
            m_ColumnMap[0] = m_Colu_L_RectTransform;

            // 初始化列槽数组容器
            m_ColumnSlots[0] = new List<FragmentSlot>();

            if (m_LetterMainCfg.columnIds.Length > 1)
            {
                m_ColumnMap[1] = m_Colu_R_RectTransform;
                m_ColumnSlots[1] = new List<FragmentSlot>();
                m_Colu_R_RectTransform.gameObject.SetActive(true);
            }
            else
            {
                m_Colu_R_RectTransform.gameObject.SetActive(false);
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            EventMgr.RemoveEvent(EventConf.DragSlotNotify, OnDragSlotNotify);
            EventMgr.RemoveEvent(EventConf.DragSlotResetNotify, OnDragSlotResetNotify);
        }

        private void InitBtns()
        {
            m_Btn_Back_Button.onClick.AddListener(CloseSelf);
            m_Btn_Confirm_Button.onClick.AddListener(OnClickConfirm);
            m_Btn_Reset_Button.onClick.AddListener(OnClickReset);
            m_Btn_Hint_Button.onClick.AddListener(OnClickHint);

            ShowQuickPassCD();
        }

        private async Task ShowQuickPassCD()
        {
            if (m_QuickPassTime <= 0)
            {
                m_Text_Hint_Text.text = $"速通";
                return;
            }
            else
            {
                m_Text_Hint_Text.text = $"速通({m_QuickPassTime}s)";
            }
            await Task.Delay(1000);
            m_QuickPassTime -= 1;
            ShowQuickPassCD();
        }

        private void OnClickConfirm()
        {
            ShowResult();
        }

        #region 结局判定

        private void ShowResult()
        {
            List<string> outcomes = GetOutComes();
            foreach (string outcome in outcomes)
            {
                Debug.Log($"结果串 outcome: {outcome}");
            }

            bool isUseDefault = true;
            foreach (int outcomeId in m_LetterMainCfg.outcomeIds)
            {
                LetterOutComeItem outcomeCfg = LetterOutCome.Data.Get(outcomeId);
                bool isComplete = true;
                for (var index = 0; index < outcomeCfg.rule.Length; index++)
                {
                    string cfgResultStr = outcomeCfg.rule[index];
                    string outcome = outcomes[index];
                    if (!outcome.Equals(cfgResultStr))
                    {
                        isComplete = false;
                        break;
                    }
                }
                if (isComplete)
                {
                    isUseDefault = false;

                    DialogueMgr.Instance.GameFinish(outcomeCfg.result);
                    UIMgr.CloseUI(UIConf.LetterBoardUI);

                    Debug.Log($"<color=#00ff00>匹配结果 outcomeId: {outcomeId}</color>");
                    break;
                }
            }
            if (isUseDefault)
            {
                LetterOutComeItem outcomeCfg = LetterOutCome.Data.Get(m_LetterMainCfg.defaultOutcomeId);
                DialogueMgr.Instance.GameFinish(outcomeCfg.result);
                UIMgr.CloseUI(UIConf.LetterBoardUI);
                Debug.Log($"<color=#ff0000>未匹配结果 使用默认结果: {m_LetterMainCfg.defaultOutcomeId}</color>");
            }
        }

        private List<string> GetOutComes()
        {
            List<string> outcomes = new List<string>();
            for (var index = 0; index < m_LetterMainCfg.columnIds.Length; index++)
            {
                string outcome = $"{m_LetterId}";

                List<FragmentSlot> slots = m_ColumnSlots[index];
                foreach (FragmentSlot fragmentSlot in slots)
                {
                    if (fragmentSlot == null) continue;
                    FragmentPiece pieceInfo = fragmentSlot.CurrentPiece;
                    // if (!pieceInfo.IsLockedForDrag())
                    // {
                    outcome += $"_{pieceInfo.PieceId}";
                    // }
                }

                outcomes.Add(outcome);
            }

            return outcomes;
        }

        #endregion

        #region 重置

        private void OnClickReset()
        {
            if (m_LetterMainCfg == null || m_LetterMainCfg.id == 0)
            {
                return;
            }

            InternalResetAndRebuild();
        }

        private void OnClickHint()
        {
            if (m_QuickPassTime > 0)
            {
                return;
            }

            ClearGo_LR();
            RefreshColumnShow();

            BuildHintPieceInfo();
            RebuildLayouts(m_Colu_L_RectTransform);
            RebuildLayouts(m_Colu_R_RectTransform);
        }

        private void InternalResetAndRebuild()
        {
            ClearGo_LR();
            RefreshColumnShow();

            BuildSlotsAndPieceInfo();
            RebuildLayouts(m_Colu_L_RectTransform);
            RebuildLayouts(m_Colu_R_RectTransform);
        }

        private void ClearGo_LR()
        {
            DestroyAllChildren(m_ColumnMap[0]);
            if (m_ColumnMap.Count > 1)
            {
                DestroyAllChildren(m_ColumnMap[1]);
            }
        }

        private void RebuildLayouts(Transform tr)
        {
            LayoutGroup[] layouts = tr.GetComponentsInChildren<LayoutGroup>();
            foreach (LayoutGroup layout in layouts)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
            }
        }

        private void DestroyAllChildren(RectTransform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i);
                if (child != null) Object.DestroyImmediate(child.gameObject);
            }
        }

        #endregion
        private void BuildHintPieceInfo()
        {
            LetterOutComeItem hintOutComeItem = LetterOutCome.Data.Get(m_LetterMainCfg.hintOutcomeId);
            int columnIdx = 0;

            foreach (string ruleStr in hintOutComeItem.rule)
            {
                string[] strIds = ruleStr.Split("_");
                List<int> pieceIds = new();
                for (int i = 1; i < strIds.Length; i++)
                {
                    pieceIds.Add(int.Parse(strIds[i]));
                }

                int currentLockGroupId = -1; // 锁组id计数
                bool prevWasLocked = false;

                for (int idx = 0; idx < pieceIds.Count; idx++)
                {
                    int pieceId = pieceIds[idx];

                    int slotIdx = idx + 1;
                    LetterPieceItem pieceItem = LetterPiece.Data.Get(pieceId);
                    bool draggable = pieceItem.dragable == 1;

                    //FragmentSlot
                    GameObject slot = InstantiateObj("Prefabs/Letter/FragmentSlot", m_ColumnMap[columnIdx]);
                    slot.name = $"Slot_{pieceId}_{slotIdx}_{(draggable ? "dragAble" : "dragDisable")}";

                    FragmentSlot slotComp = slot.GetComponent<FragmentSlot>();
                    slotComp.Init(columnIdx, slotIdx);

                    while (m_ColumnSlots[columnIdx].Count < slotIdx + 1) m_ColumnSlots[columnIdx].Add(null);
                    m_ColumnSlots[columnIdx][slotIdx] = slotComp;

                    //FragmentPiece
                    GameObject piece = InstantiateObj("Prefabs/Letter/FragmentPiece", m_ColumnMap[columnIdx]);
                    piece.name = $"Piece_{pieceId}";

                    // Piece放到FragmentSlot/Root节点下
                    // Transform slotRoot = slot.transform.Find("Root") != null ? slot.transform.Find("Root") : slot.transform;
                    piece.transform.SetParent(slot.transform, false);

                    FragmentPiece pieceComp = piece.GetComponent<FragmentPiece>();
                    pieceComp.Init(pieceId, columnIdx, slotIdx, slotComp, draggable);
                    // 这里我们决定它的锁组编号
                    if (!draggable) // 这是不可拖 piece
                    {
                        if (!prevWasLocked)
                        {
                            // 刚进入一段新的锁组
                            currentLockGroupId++;
                        }
                        pieceComp.LockGroupId = currentLockGroupId;
                        prevWasLocked = true;
                    }
                    else
                    {
                        // 可拖 piece
                        pieceComp.LockGroupId = -1;
                        prevWasLocked = false;
                    }

                    slotComp.CurrentPiece = pieceComp;

                    //第一行锁定
                    // slotComp.Locked = !draggable;   // 不可拖的片段所在行锁死（如果你想这样表达）
                    slotComp.Locked = slotIdx == 1; // 例：第1行不能被穿越/占用
                }
                columnIdx++;
            }
        }
        private void BuildSlotsAndPieceInfo()
        {
            int columnIdx = 0;
            foreach (int columnId in m_LetterMainCfg.columnIds)
            {
                LetterColumnItem letterColCfg = LetterColumn.Data.Get(columnId);
                int currentLockGroupId = -1; // 锁组id计数
                bool prevWasLocked = false;

                for (int idx = 0; idx < letterColCfg.slotInfo.Length; idx++)
                {
                    int pieceId = letterColCfg.slotInfo[idx];

                    int slotIdx = idx + 1;
                    LetterPieceItem pieceItem = LetterPiece.Data.Get(pieceId);
                    bool draggable = pieceItem.dragable == 1;

                    //FragmentSlot
                    GameObject slot = InstantiateObj("Prefabs/Letter/FragmentSlot", m_ColumnMap[columnIdx]);
                    slot.name = $"Slot_{pieceId}_{slotIdx}_{(draggable ? "dragAble" : "dragDisable")}";

                    FragmentSlot slotComp = slot.GetComponent<FragmentSlot>();
                    slotComp.Init(columnIdx, slotIdx);

                    while (m_ColumnSlots[columnIdx].Count < slotIdx + 1) m_ColumnSlots[columnIdx].Add(null);
                    m_ColumnSlots[columnIdx][slotIdx] = slotComp;

                    //FragmentPiece
                    GameObject piece = InstantiateObj("Prefabs/Letter/FragmentPiece", m_ColumnMap[columnIdx]);
                    piece.name = $"Piece_{pieceId}";

                    // Piece放到FragmentSlot/Root节点下
                    // Transform slotRoot = slot.transform.Find("Root") != null ? slot.transform.Find("Root") : slot.transform;
                    piece.transform.SetParent(slot.transform, false);

                    FragmentPiece pieceComp = piece.GetComponent<FragmentPiece>();
                    pieceComp.Init(pieceId, columnIdx, slotIdx, slotComp, draggable);
                    // 这里我们决定它的锁组编号
                    if (!draggable) // 这是不可拖 piece
                    {
                        if (!prevWasLocked)
                        {
                            // 刚进入一段新的锁组
                            currentLockGroupId++;
                        }
                        pieceComp.LockGroupId = currentLockGroupId;
                        prevWasLocked = true;
                    }
                    else
                    {
                        // 可拖 piece
                        pieceComp.LockGroupId = -1;
                        prevWasLocked = false;
                    }

                    slotComp.CurrentPiece = pieceComp;

                    //第一行锁定
                    // slotComp.Locked = !draggable;   // 不可拖的片段所在行锁死（如果你想这样表达）
                    slotComp.Locked = slotIdx == 1; // 例：第1行不能被穿越/占用
                }
                columnIdx++;
            }
        }

        private void OnDragSlotNotify(params object[] param)
        {
            int targetColId = (int)param[0];
            int insertIndex = (int)param[1];
            FragmentPiece piece = param[2] as FragmentPiece;

            InsertFromPieceImpl(targetColId, insertIndex, piece);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Colu_L_RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Colu_R_RectTransform);
        }


        private void OnDragSlotResetNotify(params object[] param)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Colu_L_RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Colu_R_RectTransform);
        }

        private void InsertFromPieceImpl(int targetColId, int insertIndex, FragmentPiece piece)
        {
            if (!m_ColumnSlots.TryGetValue(targetColId, out var targetList) || targetList == null) return;

            // 记录来源并先把来源槽清空（防止同列移动时当作阻挡）
            FragmentSlot srcSlot = piece.CurrentSlot;
            int srcColId = -1;
            int srcIndex = -1;
            if (srcSlot != null)
            {
                srcColId = srcSlot.ColumnId;
                srcIndex = srcSlot.Index;
                srcSlot.CurrentPiece = null;
            }

            int oldCount = targetList.Count;

            // 规范化插入索引
            if (insertIndex < 0) insertIndex = 0;
            if (insertIndex > oldCount) insertIndex = oldCount;

            // Ensure之前判定是否是末尾追加
            bool append = (insertIndex == oldCount);

            //确保插入位存在（如果是末尾追加，这里会创建 目标位 那一个空槽）
            EnsureSlotIndexExists(targetColId, insertIndex);

            // 命中锁位则推进到下一个非锁位（可能把插入变成末尾）
            if (targetList[insertIndex] != null && targetList[insertIndex].Locked)
            {
                insertIndex = FindNextUnlockedIndex(targetColId, insertIndex);
                if (insertIndex < 0)
                {
                    // 找不到合法落点：回滚来源
                    if (srcSlot != null) srcSlot.SetPiece(piece);
                    return;
                }

                // 如果推进到了末尾以外，改为末尾追加并确保该位置存在
                if (insertIndex >= m_ColumnSlots[targetColId].Count)
                {
                    append = true;
                    EnsureSlotIndexExists(targetColId, insertIndex);
                }
                else
                {
                    append = (insertIndex == m_ColumnSlots[targetColId].Count - 1) && (m_ColumnSlots[targetColId][insertIndex].CurrentPiece == null);
                }
            }

            if (!CanInsertHere(targetColId, insertIndex, piece))
            {
                if (srcSlot != null)
                {
                    srcSlot.SetPiece(piece); // 回滚
                }
                return;
            }
            Debug.Log($"InsertFromPiece ----- PieceId:{piece.PieceId}  targetColId:{targetColId}  insertIndex:{insertIndex}  append:{append}");

            if (append)
            {
                //末尾追加 直接放入目标位，不需要缓冲槽与下移
                m_ColumnSlots[targetColId][insertIndex].SetPiece(piece);
            }
            else
            {
                //中间插入 确保尾部存在一个缓冲空槽，用于整体下移后的落脚
                bool tailAdded = EnsureTailBuffer(targetColId);

                // 自底向上把 [insertIndex到]到[末尾-1]的内容整体下移一格
                for (int i = m_ColumnSlots[targetColId].Count - 2; i >= insertIndex; i--)
                {
                    FragmentSlot from = m_ColumnSlots[targetColId][i];
                    FragmentSlot to = m_ColumnSlots[targetColId][i + 1];

                    // 不能把内容推到锁位
                    if (to.Locked)
                    {
                        if (tailAdded) RemoveLastSlotIfEmpty(targetColId);
                        if (srcSlot != null) srcSlot.SetPiece(piece);
                        return;
                    }

                    if (from.CurrentPiece == null) continue;

                    FragmentPiece victim = from.CurrentPiece;
                    if (!victim.IsSystemMovable())
                    {
                        if (tailAdded) RemoveLastSlotIfEmpty(targetColId);
                        if (srcSlot != null) srcSlot.SetPiece(piece);
                        return;
                    }

                    from.CurrentPiece = null;
                    to.SetPiece(victim);
                }

                // 把拖来的 piece 放入空出的 insertIndex
                m_ColumnSlots[targetColId][insertIndex].SetPiece(piece);

                //同列上移：先全列压缩，再裁掉尾部空槽,防止中间留下空槽
                if (srcColId == targetColId && insertIndex <= srcIndex)
                {
                    CompactColumn(targetColId);
                    TrimTrailingEmptySlots(targetColId);
                }
            }

            // 源列压缩:被拿走位置之下的条目整体上移，并删掉末尾空槽
            if (srcColId >= 0)
            {
                bool needCompress = !(srcColId == targetColId && insertIndex <= srcIndex);
                if (needCompress)
                {
                    CompressAfterRemove(srcColId, srcIndex);
                }
            }

            //刷新两个列的 Index
            ReindexColumn(targetColId);
            if (srcColId >= 0) ReindexColumn(srcColId);
        }

        /// <summary>
        /// 把本列的非空 piece 稳定地向前“紧凑”，保证中间不出现空槽；
        /// - 只移动到可写的空槽（!Locked && CurrentPiece==null）
        /// - 遇到锁位或已占用的槽会绕过
        /// - 相对顺序不变
        /// </summary>
        private void CompactColumn(int colId)
        {
            if (!m_ColumnSlots.TryGetValue(colId, out var list) || list == null) return;

            int write = 0;

            // 找到第一个可写空槽（跳过所有锁位或已占用槽）
            while (write < list.Count &&
                   (list[write] == null || list[write].Locked || list[write].CurrentPiece != null))
            {
                write++;
            }

            for (int read = write + 1; read < list.Count; read++)
            {
                var readSlot = list[read];
                if (readSlot == null) continue;

                var p = readSlot.CurrentPiece;
                if (p == null) continue; // 空槽跳过
                if (readSlot.Locked) continue; // 锁位的内容不挪动

                // 确保 write 指向下一个“可写空槽”
                while (write < list.Count &&
                       (list[write] == null || list[write].Locked || list[write].CurrentPiece != null))
                {
                    write++;
                }
                if (write >= list.Count) break; // 已无可写槽

                // 如果 read 在 write 前面，说明 write 追到了 read，继续找下一个
                if (read <= write) continue;

                // 把 piece 从 read 移到 write
                readSlot.CurrentPiece = null;
                var toSlot = list[write];
                toSlot.SetPiece(p);

                // 写指针继续前进到下一个“可写空槽”
                while (write < list.Count &&
                       (list[write] == null || list[write].Locked || list[write].CurrentPiece != null))
                {
                    write++;
                }
            }
        }

        private bool CanInsertHere(int targetColId, int insertIndex, FragmentPiece draggingPiece)
        {
            if (draggingPiece.IsLockedForDrag())
                return true;

            if (!m_ColumnSlots.TryGetValue(targetColId, out var col) || col == null)
                return true;

            if (insertIndex < 0) insertIndex = 0;
            if (insertIndex > col.Count) insertIndex = col.Count;

            int LockGroupAt(int idx)
            {
                if (idx < 0 || idx >= col.Count) return -1;
                var slot = col[idx];
                if (slot == null || slot.CurrentPiece == null) return -1;

                return slot.CurrentPiece.GetLockGroupId(); // -1 表示自由 / 非锁
            }

            int leftGroup  = LockGroupAt(insertIndex - 1);
            int rightGroup = LockGroupAt(insertIndex);

            if (leftGroup != -1 && leftGroup == rightGroup)
            {
                return false;
            }

            if (insertIndex == 0 && rightGroup != -1)
            {
                return false;
            }

            return true;
        }

        // 连续裁掉列尾的空槽（直到尾部不是空槽为止）
        private void TrimTrailingEmptySlots(int colId)
        {
            var list = m_ColumnSlots[colId];
            while (list.Count > 0)
            {
                FragmentSlot last = list[^1];
                if (last == null || last.CurrentPiece != null) break;

                list.RemoveAt(list.Count - 1);
                if (last != null)
                {
                    Object.Destroy(last.gameObject);
                }
            }
        }

        //若末尾没有空槽就补一个，有的话复用
        private bool EnsureTailBuffer(int colId)
        {
            List<FragmentSlot> list = m_ColumnSlots[colId];
            if (list.Count == 0)
            {
                AddSlotAtEnd(colId);
                return true;
            }

            FragmentSlot last = list[^1];
            if (last == null || last.CurrentPiece != null)
            {
                AddSlotAtEnd(colId);
                return true;
            }
            return false; //末尾有空槽
        }

        private void EnsureSlotIndexExists(int colId, int index)
        {
            if (m_ColumnSlots[colId].Count <= index)
            {
                AddSlotAtEnd(colId);
            }
        }

        // 在列末尾新增一个空 Slot
        private FragmentSlot AddSlotAtEnd(int colId)
        {
            RectTransform parent = m_ColumnMap[colId];
            GameObject go = InstantiateObj("Prefabs/Letter/FragmentSlot", parent);
            FragmentSlot slot = go.GetComponent<FragmentSlot>();
            int newIndex = m_ColumnSlots[colId].Count;
            slot.Init(colId, newIndex);
            slot.Locked = false;

            m_ColumnSlots[colId].Add(slot);

            Debug.Log($"AddSlotAtEnd -----  colId:{colId}   newIndex:{newIndex}");
            return slot;
        }

        // 如果列末尾是空槽就删除
        private void RemoveLastSlotIfEmpty(int colId)
        {
            List<FragmentSlot> list = m_ColumnSlots[colId];
            if (list.Count == 0) return;

            FragmentSlot lastSlot = list[^1];
            if (lastSlot != null && lastSlot.CurrentPiece == null)
            {
                list.RemoveAt(list.Count - 1);
                if (lastSlot != null)
                {
                    Object.Destroy(lastSlot.gameObject);
                }
            }
        }

        //整体上移 + 删尾部空槽
        private void CompressAfterRemove(int colId, int removedIndex)
        {
            if (!m_ColumnSlots.TryGetValue(colId, out var list) || list == null) return;
            if (removedIndex < 0 || removedIndex >= list.Count) return;

            for (int i = removedIndex + 1; i < list.Count; i++)
            {
                FragmentSlot from = list[i];
                FragmentSlot to = list[i - 1];
                if (from.CurrentPiece == null) continue;

                FragmentPiece mover = from.CurrentPiece;
                from.CurrentPiece = null;
                to.SetPiece(mover);
            }

            // 删掉末尾连续空槽
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].CurrentPiece != null) break;
                GameObject go = list[i].gameObject;
                list.RemoveAt(i);
                if (go != null) Object.Destroy(go);
            }
        }

        private int FindNextUnlockedIndex(int colId, int start)
        {
            List<FragmentSlot> list = m_ColumnSlots[colId];
            for (int i = start; i < list.Count; i++)
            {
                if (!list[i].Locked) return i;
            }
            return -1;
        }

        // 刷新列内 FragmentSlot.Index
        private void ReindexColumn(int colId)
        {
            if (!m_ColumnSlots.TryGetValue(colId, out List<FragmentSlot> list) || list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                FragmentSlot slot = list[i];
                if (slot != null) slot.Index = i;
            }
        }

        private GameObject InstantiateObj(string path, Transform parent)
        {
            GameObject go = Global.gApp.gResMgr.InstantiateObj(path, parent);
            if (!go.activeSelf) go.SetActive(true);

            return go;
        }
    }
}