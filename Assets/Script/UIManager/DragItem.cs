using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected DragSortablePanel parentPanel;
    public DragSortablePanel ParentPanel => parentPanel;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private RectTransform parentRect;
    private Canvas canvas;
    private Camera uiCamera;

    private Vector2 originalPosition;
    private int originalIndex;
    private Vector2 dragOffset;
    private bool isDragging = false;
    private int lastTargetIndex = -1;
    private bool isOverRecycleZone = false; // 是否在回收区域上方
    public void setIsOverRecycleZone(bool input)
    {
        isOverRecycleZone = input;
    }
    [HideInInspector] public bool IsTemporary = false; // 是否为临时生成的拖拽对象
    [HideInInspector] public RectTransform SourceItem; // 关联的源物体（仅临时对象用）

    public bool IsDragging => isDragging;

    private string type; // 行为类型字段

    public void SetType(string type)
    {
        this.type = type;
    }

    public void Initialize(DragSortablePanel parent, Canvas canvas, bool isTemporary = false, RectTransform sourceItem = null)
    {
        parentPanel = parent;
        rectTransform = GetComponent<RectTransform>();
        parentRect = parentPanel.GetComponent<RectTransform>();
        this.canvas = canvas;
        this.IsTemporary = isTemporary;
        this.SourceItem = sourceItem;

        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        if (!canvasGroup)
        {
            canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
        }
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            uiCamera = canvas.worldCamera;
        else if (canvas.renderMode == RenderMode.WorldSpace)
            uiCamera = Camera.main;
        else
            uiCamera = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentPanel.SetGridLayoutEnabled(false);
        originalPosition = rectTransform.anchoredPosition;
        var children = parentPanel.GetChildren();
        originalIndex = children.IndexOf(rectTransform);
        lastTargetIndex = originalIndex;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, eventData.position, uiCamera, out Vector2 mouseLocalPos))
        {
            dragOffset = mouseLocalPos - originalPosition;
        }

        isDragging = true;
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        // 临时对象特殊处理：设置为顶级显示
        if (IsTemporary)
        {
            rectTransform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, eventData.position, uiCamera, out Vector2 mouseLocalPos))
        {
            rectTransform.anchoredPosition = mouseLocalPos - dragOffset;
        }

        // 临时对象不参与排序，仅原生对象处理排序逻辑
        if (!IsTemporary)
        {
            CalculateTargetPosition();
        }
    }

    private void CalculateTargetPosition()
    {
        var children = parentPanel.GetChildren();
        if (children.Count <= 1) return;

        int targetIndex = originalIndex;
        float currentY = rectTransform.anchoredPosition.y;
        float halfHeight = rectTransform.rect.height * 0.5f;
        bool foundTarget = false;

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] == rectTransform) continue;

            float otherY = children[i].anchoredPosition.y;
            float otherHalfHeight = children[i].rect.height * 0.5f;

            if (currentY + halfHeight > otherY + otherHalfHeight + 5f)
            {
                targetIndex = i;
                foundTarget = true;
                break;
            }
            else if (currentY - halfHeight < otherY - otherHalfHeight - 5f)
            {
                targetIndex = i + 1;
                foundTarget = true;
                continue;
            }
        }

        if (!foundTarget && currentY < GetLastItemBottomY(children))
        {
            targetIndex = children.Count;
        }

        targetIndex = Mathf.Clamp(targetIndex, 0, children.Count);

        if (targetIndex != lastTargetIndex)
        {
            lastTargetIndex = targetIndex;
            parentPanel.ReorderChildren(rectTransform, targetIndex, true);
        }
    }

    private float GetLastItemBottomY(List<RectTransform> children)
    {
        if (children.Count == 0) return 0;

        RectTransform lastItem = null;
        for (int i = children.Count - 1; i >= 0; i--)
        {
            if (children[i] != rectTransform)
            {
                lastItem = children[i];
                break;
            }
        }

        if (lastItem != null)
        {
            return lastItem.anchoredPosition.y - lastItem.rect.height * 0.5f - 10f;
        }
        return 0;
    }

    // 在DragItem.cs中修改OnEndDrag方法
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (IsTemporary)
        {
            // ① 回收区销毁逻辑
            if (isOverRecycleZone)
            {
                Destroy(gameObject);
                return;
            }

            // 判断是否在目标面板内
            bool isInsidePanel = IsCenterInsidePanel(rectTransform, parentRect);

            if (isInsidePanel)
            {
                // 核心修改：直接将临时临时对象转为正式对象并添加到面板
                IsTemporary = false; // 标记为非临时对象
                transform.SetParent(parentPanel.transform); // 变更父节点为目标面板

                // 计算插入位置并插入
                int insertIndex = CalculateInsertIndex();
                parentPanel.ReorderChildren(rectTransform, insertIndex, false);

                // 初始化为本地面板对象
                Initialize(parentPanel, canvas, false, null);
            }
            else
            {
                // ② 未进入面板则销毁
                //Destroy(gameObject);
            }
            return;
        }

        // 非临时对象原有逻辑
        parentPanel.FinalizePosition(rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
        isOverRecycleZone = false;
    }


    // 判断临时对象中心点是否在目标面板内部
    private bool IsCenterInsidePanel(RectTransform target, RectTransform panel)
    {
        // 将目标中心点转换为面板的本地坐标
        Vector3 targetCenter = target.TransformPoint(target.rect.center);
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panel,
            RectTransformUtility.WorldToScreenPoint(uiCamera, targetCenter),
            uiCamera,
            out localPos
        );

        // 检查是否在面板矩形范围内
        Rect panelRect = panel.rect;
        return panelRect.Contains(localPos);
    }


    // 计算插入索引（基于临时对象位置）
    private int CalculateInsertIndex()
    {
        List<RectTransform> children = parentPanel.GetChildren();
        if (children.Count == 0) return 0;

        // 获取临时对象在面板中的Y坐标（本地坐标）
        Vector2 tempLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            RectTransformUtility.WorldToScreenPoint(uiCamera, rectTransform.position),
            uiCamera,
            out tempLocalPos
        );
        float tempY = tempLocalPos.y;

        // 找到第一个Y坐标小于临时对象Y坐标的位置，插入到其上方
        for (int i = 0; i < children.Count; i++)
        {
            Vector2 childLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                RectTransformUtility.WorldToScreenPoint(uiCamera, children[i].position),
                uiCamera,
                out childLocalPos
            );
            if (childLocalPos.y < tempY)
            {
                return i;
            }
        }

        // 默认插入到末尾
        return children.Count;
    }


}
