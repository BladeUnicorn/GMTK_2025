using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    // 单例实例
    public static UIManager Instance { get; private set; }

    // UI面板预设路径
    [SerializeField] private string uiPrefabPath = "UIprefab/";

    // 各层级UI的父物体
    [SerializeField] private Transform uiRoot;
    [SerializeField] private Transform normalLayer;
    [SerializeField] private Transform popUpLayer;
    [SerializeField] private Transform topLayer;
    [SerializeField] private Transform tipsLayer;

    // 已加载的UI面板字典
    private Dictionary<Type, UIPanel> loadedPanels = new Dictionary<Type, UIPanel>();
    // 当前显示的面板队列
    private Stack<UIPanel> currentPanels = new Stack<UIPanel>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLayers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 初始化UI层级
    private void InitializeLayers()
    {
        if (uiRoot == null)
        {
            GameObject rootObj = new GameObject("UIRoot");
            uiRoot = rootObj.transform;
            DontDestroyOnLoad(rootObj);
        }

        // 创建各层级    
        if (normalLayer == null) normalLayer = CreateLayer("NormalLayer");
        if (popUpLayer == null) popUpLayer = CreateLayer("PopUpLayer");
        if (topLayer == null) topLayer = CreateLayer("TopLayer");
        if (tipsLayer == null) tipsLayer = CreateLayer("TipsLayer");
    }

    // 创建UI层级
    private Transform CreateLayer(string layerName)
    {
        GameObject layerObj = new GameObject(layerName);
        layerObj.transform.SetParent(uiRoot);

        // 获取或添加RectTransform组件
        RectTransform rectTransform = layerObj.AddComponent<RectTransform>();

        // 设置锚点为全屏
        rectTransform.anchorMin = Vector2.zero;      // 左下锚点
        rectTransform.anchorMax = Vector2.one;       // 右上锚点
        rectTransform.pivot = new Vector2(0.5f, 0.5f); // 中心点在中心

        // 清除位置和大小偏移，确保完全铺满
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        // 重置缩放和局部位置
        layerObj.transform.localScale = Vector3.one;

        // 添加Canvas组件
        Canvas canvas = layerObj.AddComponent<Canvas>();
        canvas.overrideSorting = true;

        // 设置排序层级
        if (layerName == "NormalLayer") canvas.sortingOrder = 10;
        else if (layerName == "PopUpLayer") canvas.sortingOrder = 20;
        else if (layerName == "TopLayer") canvas.sortingOrder = 30;
        else if (layerName == "TipsLayer") canvas.sortingOrder = 40;

        // 添加GraphicRaycaster用于UI交互
        layerObj.AddComponent<GraphicRaycaster>();

        return layerObj.transform;
    }

    // 获取指定类型的UI面板
    public T GetPanel<T>() where T : UIPanel
    {
        Type panelType = typeof(T);
        if (loadedPanels.TryGetValue(panelType, out UIPanel panel))
        {
            return panel as T;
        }
        return null;
    }

    // 显示指定类型的UI面板
    public void ShowPanel<T>(UnityAction<T> onInitialized = null) where T : UIPanel
    {
        Type panelType = typeof(T);

        // 如果面板已加载，直接显示
        if (loadedPanels.TryGetValue(panelType, out UIPanel panel))
        {
            panel.Show();
            onInitialized?.Invoke(panel as T);
            return;
        }

        // 加载面板预设
        string prefabName = panelType.Name;
        GameObject prefab = Resources.Load<GameObject>($"{uiPrefabPath}{prefabName}");

        if (prefab == null)
        {
            Debug.LogError($"找不到UI面板预设: {uiPrefabPath}{prefabName}");
            return;
        }

        // 实例化面板
        GameObject panelObj = Instantiate(prefab);
        T newPanel = panelObj.GetComponent<T>();

        if (newPanel == null)
        {
            Debug.LogError($"UI面板 {prefabName} 上没有挂载 {panelType.Name} 组件");
            Destroy(panelObj);
            return;
        }

        // 设置面板父物体
        SetPanelParent(newPanel);

        // 初始化面板
        newPanel.Initialize();
        loadedPanels.Add(panelType, newPanel);

        // 显示面板
        newPanel.Show();
        currentPanels.Push(newPanel);

        onInitialized?.Invoke(newPanel);
    }

    // 设置面板父物体，保持Prefab原始锚点
    private void SetPanelParent(UIPanel panel)
    {
        // 获取面板的RectTransform
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            Debug.LogError("面板没有RectTransform组件");
            return;
        }

        // 保存Prefab原有的RectTransform属性
        Vector2 originalAnchorMin = panelRect.anchorMin;
        Vector2 originalAnchorMax = panelRect.anchorMax;
        Vector2 originalPivot = panelRect.pivot;
        Vector2 originalAnchoredPosition = panelRect.anchoredPosition;
        Vector2 originalSizeDelta = panelRect.sizeDelta;

        // 根据面板层级设置父物体
        Transform targetParent = GetLayerTransform(panel.Layer);
        panelRect.SetParent(targetParent);

        // 恢复Prefab原有的锚点和位置属性
        panelRect.anchorMin = originalAnchorMin;
        panelRect.anchorMax = originalAnchorMax;
        panelRect.pivot = originalPivot;
        panelRect.anchoredPosition = originalAnchoredPosition;
        panelRect.sizeDelta = originalSizeDelta;

        // 确保缩放不受父物体影响
        panelRect.localScale = Vector3.one;
    }

    // 获取对应层级的Transform
    private Transform GetLayerTransform(UILayer layer)
    {
        switch (layer)
        {
            case UILayer.Normal:
                return normalLayer;
            case UILayer.PopUp:
                return popUpLayer;
            case UILayer.Top:
                return topLayer;
            case UILayer.Tips:
                return tipsLayer;
            default:
                return normalLayer;
        }
    }


    // 隐藏指定类型的UI面板
    public void HidePanel<T>(bool destroy = false) where T : UIPanel
    {
        Type panelType = typeof(T);
        if (loadedPanels.TryGetValue(panelType, out UIPanel panel))
        {
            panel.Hide();

            if (destroy)
            {
                loadedPanels.Remove(panelType);
                currentPanels = new Stack<UIPanel>(currentPanels); // 重新创建栈以避免修改枚举中的集合
                Destroy(panel.gameObject);
            }
        }
    }

    // 隐藏当前最上层的面板
    public void HideTopPanel(bool destroy = false)
    {
        if (currentPanels.Count > 0)
        {
            UIPanel topPanel = currentPanels.Pop();
            topPanel.Hide();

            if (destroy)
            {
                loadedPanels.Remove(topPanel.GetType());
                Destroy(topPanel.gameObject);
            }
        }
    }

    // 隐藏所有面板
    public void HideAllPanels(bool destroy = false)
    {
        foreach (var panel in loadedPanels.Values)
        {
            panel.Hide();
            if (destroy)
            {
                Destroy(panel.gameObject);
            }
        }

        if (destroy)
        {
            loadedPanels.Clear();
            currentPanels.Clear();
        }
    }

    // 销毁指定类型的UI面板
    public void DestroyPanel<T>() where T : UIPanel
    {
        HidePanel<T>(true);
    }

    // 销毁所有UI面板
    public void DestroyAllPanels()
    {
        HideAllPanels(true);
    }
}
