using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DragSortablePanel : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayout;
    private Canvas canvas;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private GameObject dragItemPrefab; // 临时拖拽对象预制体
    private List<RectTransform> childItems = new List<RectTransform>();
    private Coroutine currentAnimation;
    private RectTransform draggingItem = null;
    private Dictionary<RectTransform, Vector2> targetPositions = new Dictionary<RectTransform, Vector2>();

    private void Awake()
    {
        if (gridLayout == null)
            gridLayout = GetComponent<GridLayoutGroup>();

        if (canvas == null)
            canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();

        // 初始化布局
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 1;
        gridLayout.startAxis = GridLayoutGroup.Axis.Vertical;


        InitializeChildren();
    }

    private void InitializeChildren()
    {
        childItems.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = transform.GetChild(i).GetComponent<RectTransform>();
            if (child != null)
            {
                childItems.Add(child);
                DragItem dragItem = child.GetComponent<DragItem>();
                if (dragItem == null)
                {
                    dragItem = child.gameObject.AddComponent<DragItem>();
                    dragItem.SetType("_");
                }
                dragItem.Initialize(this, canvas);
            }
        }
    }




    // 从面板移除子物体
    public void RemoveItem(RectTransform item)
    {
        if (childItems.Contains(item))
        {
            childItems.Remove(item);
            Destroy(item.gameObject);
            UpdateTargetPositions();
            StartCoroutine(AnimateReordering(false));
        }
    }

    public void ReorderChildren(RectTransform movingItem, int targetIndex, bool isDragging)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        draggingItem = isDragging ? movingItem : null;

        int originalCount = childItems.Count;
        bool isInList = childItems.Contains(movingItem);

        int tempIndex = Mathf.Clamp(targetIndex, 0, originalCount);

        if (isInList)
        {
            childItems.Remove(movingItem);
            int newCount = childItems.Count;
            int validIndex = Mathf.Clamp(tempIndex, 0, newCount);
            childItems.Insert(validIndex, movingItem);


        }
        else
        {
            int validIndex = Mathf.Clamp(tempIndex, 0, childItems.Count);
            childItems.Insert(validIndex, movingItem);

        }
        UpdateTargetPositions();
        if (isDragging){
            currentAnimation = StartCoroutine(AnimateReordering(isDragging));
        }
    }

    private void UpdateTargetPositions()
    {
        targetPositions.Clear();
        for (int i = 0; i < childItems.Count; i++)
        {
            RectTransform child = childItems[i];
            targetPositions[child] = CalculateGridPosition(i);
        }
    }

    private IEnumerator AnimateReordering(bool isDragging)
    {
        Dictionary<RectTransform, Vector2> startPositions = new Dictionary<RectTransform, Vector2>();
        foreach (var child in childItems)
        {
            startPositions[child] = child.anchoredPosition;
        }

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / animationDuration);

            foreach (var child in childItems)
            {
                if (isDragging && child == draggingItem)
                    continue;

                child.anchoredPosition = Vector2.Lerp(
                    startPositions[child],
                    targetPositions[child],
                    t
                );
            }

            yield return null;
        }

        foreach (var child in childItems)
        {
            if (child == draggingItem)
                continue;

            child.anchoredPosition = targetPositions[child];
        }

        for (int i = 0; i < childItems.Count; i++)
        {
            childItems[i].SetSiblingIndex(i);
        }

        currentAnimation = null;
    }

    public void FinalizePosition(RectTransform item)
    {
        if (targetPositions.TryGetValue(item, out Vector2 targetPos))
        {
            StartCoroutine(MoveToTargetPosition(item, targetPos));
        }
        draggingItem = null;
    }

    private IEnumerator MoveToTargetPosition(RectTransform item, Vector2 targetPos)
    {
        Vector2 startPos = item.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / animationDuration);
            item.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        if(elapsedTime >= animationDuration)
        {
            gridLayout.enabled = true;
            item.anchoredPosition = targetPos;
        }
            

    }

    private Vector2 CalculateGridPosition(int index)
    {
        float x = gridLayout.padding.left + gridLayout.cellSize.x * 0.5f;
        float y = -(gridLayout.padding.top + index * (gridLayout.cellSize.y + gridLayout.spacing.y) + gridLayout.cellSize.y * 0.5f);
        return new Vector2(x, y);
    }

    public List<RectTransform> GetChildren() => new List<RectTransform>(childItems);
    public void SetGridLayoutEnabled(bool enabled)
    {
        gridLayout.enabled = enabled;
    }
}
