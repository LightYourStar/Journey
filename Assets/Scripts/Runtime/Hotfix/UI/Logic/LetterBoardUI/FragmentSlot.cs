using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FragmentSlot : MonoBehaviour, IDropHandler
{
    [NonSerialized]
    public int ColumnId;
    [NonSerialized]
    public int Index;
    [NonSerialized]
    public FragmentPiece CurrentPiece; //当前占据该槽的条目（空槽 = null）
    [NonSerialized]
    public bool Locked;  // 硬锁 任何插入都不允许

    public bool IsEmpty => CurrentPiece == null;

    public void Init(int columnId, int index)
    {
        ColumnId = columnId;
        Index = index;
    }

    public void SetPiece(FragmentPiece piece)
    {
        CurrentPiece = piece;
        piece.transform.SetParent(transform, false);
        piece.Rect.anchoredPosition = Vector2.zero;
        piece.CurrentSlot = this;
        piece.ColumnId = ColumnId;
        piece.Index = Index;

        piece.NotifyDropped();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (Locked) return;

        var piece = eventData.pointerDrag ? eventData.pointerDrag.GetComponent<FragmentPiece>() : null;
        if (piece == null) return;

        if (piece.IsLockedForDrag())
        {
            return; // 玩家不可拖的条目不处理
        }

        // 插入目标列 Index 处 → 下移让位 → 源列压缩”
        EventMgr.DispatchEvent(EventConf.DragSlotNotify, ColumnId, Index, piece);
    }
}