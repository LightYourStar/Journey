using System;
using System.Collections;
using JO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FragmentPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Text m_DescDraggable;
    [SerializeField]
    private Text m_DescDragDisable;
    [SerializeField]
    private GameObject m_BgDraggable;
    [SerializeField]
    private GameObject m_BgDragDisable;

    [NonSerialized] public int PieceId;
    [NonSerialized] public int ColumnId;
    [NonSerialized] public int Index;
    [NonSerialized] public int LockGroupId = -1;
    public FragmentSlot CurrentSlot { get; set; }
    public RectTransform Rect { get; private set; }
    public CanvasGroup CanvasGroup { get; private set; }

    private Canvas m_Canvas;
    private Transform m_OriginParent;
    private bool m_DroppedOnSlot;

    private bool m_Draggable = true;

    public void Init(int pieceId, int columnId, int index, FragmentSlot currentSlot, bool draggable)
    {
        PieceId = pieceId;
        ColumnId = columnId;
        Index = index;
        CurrentSlot = currentSlot;
        m_Draggable = draggable;

        LetterPieceItem pieceItem = LetterPiece.Data.Get(pieceId);
        m_BgDraggable.SetActive(m_Draggable);
        m_BgDragDisable.SetActive(!m_Draggable);
        if (m_Draggable)
        {
            m_DescDraggable.text = pieceItem.desc;
        }
        else
        {
            m_DescDragDisable.text = pieceItem.desc;
        }
    }


    //是否不可拖
    public bool IsLockedForDrag() => !m_Draggable;

    public int GetLockGroupId()
    {
        return m_Draggable ? -1 : LockGroupId;
    }

    //是否可以插入时被顺序下移
    public bool IsSystemMovable() => true;

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
        CanvasGroup = GetComponent<CanvasGroup>();
        m_Canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (!m_Draggable) return;

        m_OriginParent = transform.parent;
        transform.SetParent(m_Canvas.transform, true);
        CanvasGroup.blocksRaycasts = false;   // 让 Slot 能接收 OnDrop
        CanvasGroup.alpha = 0.75f;
        Rect.localScale = Vector3.one * 1.1f;
        m_DroppedOnSlot = false;
    }

    public void OnDrag(PointerEventData e)
    {
        if (!m_Draggable) return;
        Rect.anchoredPosition += e.delta / m_Canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (!m_Draggable) return;

        CanvasGroup.alpha = 1f;
        Rect.localScale = Vector3.one;

        // 若没有任何Slot接住，就回到原位
        if (!m_DroppedOnSlot)
        {
            transform.SetParent(m_OriginParent, false);
            Rect.anchoredPosition = Vector2.zero;
            EventMgr.DispatchEvent(EventConf.DragSlotResetNotify);
        }

        StartCoroutine(RestoreRaycastNextFrame());
    }

    private IEnumerator RestoreRaycastNextFrame()
    {
        yield return null;
        CanvasGroup.blocksRaycasts = true;
    }

    public void NotifyDropped()
    {
        m_DroppedOnSlot = true;
    }
}