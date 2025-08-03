using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecycleZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IDropHandler
{
    private Image background;
    private Color originalColor;

    private void Awake()
    {
        background = GetComponent<Image>();
        originalColor = background.color;
    }

    public void OnDrop(PointerEventData eventData)
    {
        
        DragItem dragItem = eventData.pointerDrag?.GetComponent<Behaviors>().getTempDragObj();
        if (dragItem != null)
        {
            // 使用公共属性 ParentPanel 访问父面板
            if (!dragItem.IsTemporary)
            {
                // 调用父面板的 RemoveItem 方法移除物体
                dragItem.ParentPanel.RemoveItem(dragItem.GetComponent<RectTransform>());
            }
            else
            {

            }
        }
        background.color = originalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag?.GetComponent<Behaviors>() != null)
        {
            DragItem dragItem = eventData.pointerDrag?.GetComponent<Behaviors>().getTempDragObj();
            dragItem.setIsOverRecycleZone(false);
            background.color = new Color(1, 0.5f, 0.5f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag?.GetComponent<Behaviors>() != null)
        {
            DragItem dragItem = eventData.pointerDrag?.GetComponent<Behaviors>().getTempDragObj();
            dragItem.setIsOverRecycleZone(true);
            background.color = originalColor;
        }
    }
}