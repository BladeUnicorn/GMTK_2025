using UnityEngine;
using System.Collections;

// 定义UI层级枚举
public enum UILayer
{
    Normal,   // 普通层级
    PopUp,    // 弹窗层级
    Top,      // 顶层
    Tips      // 提示层级
}

public class UIPanel : MonoBehaviour
{
    // 面板所在层级，可在Inspector中手动设置
    [SerializeField] private UILayer layer = UILayer.Normal;

    // 公开的只读属性，供外部访问层级
    public UILayer Layer => layer;

    // 面板是否为模态窗口（点击背景是否关闭）
    [SerializeField] protected bool isModal = false;
    public bool IsModal => isModal;

    // 移动动画的持续时间
    [SerializeField] protected float moveDuration = 0.3f;

    // 面板的RectTransform组件
    protected RectTransform rectTransform;

    protected virtual void Awake()
    {
        // 获取RectTransform组件
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = gameObject.AddComponent<RectTransform>();
        }
    }

    // 面板的生命周期方法
    public virtual void Initialize() { }
    public virtual void Show() { gameObject.SetActive(true); }
    public virtual void Hide() { gameObject.SetActive(false); }


    protected virtual void OnDestroy() { }
}
