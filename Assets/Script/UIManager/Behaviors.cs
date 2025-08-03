using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Behaviors : MonoBehaviour, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Tooltip("行为类型标识（如moveright、moveleft）")]
    public string behaviorType;

    [Tooltip("拖拽时生成的临时对象预制体")]
    GameObject dragItemPrefab;

    DragSortablePanel targetPanel;

    Canvas parentCanvas;

    private RectTransform rectTransform; // 当前物体的RectTransform
    private GameObject tempDragObj; // 临时拖拽对象
    private Vector2 mouseOffset; // 鼠标相对源物体的偏移量（世界坐标）
    private Vector3 sourceWorldPos; // 源物体按下时的世界坐标
    private RectTransform tempRect; // 临时对象的RectTransform

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        dragItemPrefab = Resources.Load<GameObject>("UIprefab/Instruction");
    }

    // 鼠标按下时记录源物体位置和鼠标偏移
    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f);

        // 1. 记录源物体（当前物体）的世界坐标（包含所有父物体的变换）
        sourceWorldPos = rectTransform.position;

        // 2. 计算鼠标在世界坐标中的位置
        Vector3 mouseWorldPos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            eventData.position,
            parentCanvas.worldCamera,
            out mouseWorldPos))
        {
            // 3. 计算鼠标相对源物体的偏移量（世界坐标中）
            mouseOffset = new Vector2(
                mouseWorldPos.x - sourceWorldPos.x,
                mouseWorldPos.y - sourceWorldPos.y
            );
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GetComponent<Image>().color = Color.white;
        if (tempDragObj)
        {
            tempDragObj.GetComponent<DragItem>().OnEndDrag(eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<Image>().color = Color.white;
        targetPanel = this.transform.parent.parent.parent.GetComponent<ConsolePanel>().get_insPanel();
        if (dragItemPrefab == null || targetPanel == null || parentCanvas == null)
        {
            return;
        }

        // 生成临时对象，父节点设为Canvas
        tempDragObj = Instantiate(dragItemPrefab, parentCanvas.transform);
        tempRect = tempDragObj.GetComponent<RectTransform>();
        DragItem tempDragItem = tempDragObj.GetComponent<DragItem>();

        // 复制源物体的尺寸和完整视觉属性
        tempRect.sizeDelta = rectTransform.sizeDelta;
        CopyVisualProperties(tempDragObj, gameObject);

        // 初始化临时对象位置：与源物体世界坐标完全一致
        tempRect.position = sourceWorldPos;

        // 初始化拖拽组件
        tempDragItem.Initialize(targetPanel, parentCanvas, true, rectTransform);
        tempDragItem.SetType(behaviorType);
        tempDragItem.IsTemporary = true;
    }

    // 复制源物体到临时对象的视觉属性
    private void CopyVisualProperties(GameObject target, GameObject source)
    {
        // 复制Image
        Image sourceImg = source.GetComponent<Image>();
        Image targetImg = target.GetComponent<Image>();
        if (sourceImg != null && targetImg != null)
        {
            targetImg.sprite = sourceImg.sprite;
            targetImg.color = sourceImg.color;
            targetImg.type = sourceImg.type;
            targetImg.preserveAspect = sourceImg.preserveAspect;
        }

        // 复制Text
        Text sourceText = source.GetComponentInChildren<Text>();
        Text targetText = target.GetComponentInChildren<Text>();
        if (sourceText != null && targetText != null)
        {
            targetText.text = sourceText.text;
            targetText.font = sourceText.font;
            targetText.fontSize = sourceText.fontSize;
            targetText.color = sourceText.color;
            targetText.alignment = sourceText.alignment;
        }
    }

    // 拖拽时通过鼠标偏移量更新位置
    public void OnDrag(PointerEventData eventData)
    {
        if (tempDragObj == null) return;

        // 1. 获取当前鼠标的世界坐标
        Vector3 currentMouseWorldPos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            tempRect,
            eventData.position,
            parentCanvas.worldCamera,
            out currentMouseWorldPos))
        {
            // 2. 临时对象位置 = 鼠标世界坐标 - 初始偏移量
            tempRect.position = new Vector3(
                currentMouseWorldPos.x - mouseOffset.x,
                currentMouseWorldPos.y - mouseOffset.y,
                sourceWorldPos.z // 保持Z轴一致
            );
        }
    }
    public DragItem getTempDragObj()
    {
        return tempDragObj.GetComponent<DragItem>();
    }
    private void OnDestroy()
    {
        if (tempDragObj != null)
            Destroy(tempDragObj);
    }
 
}
