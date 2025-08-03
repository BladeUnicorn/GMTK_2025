using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 示例面板 - 游戏面板
public class ConsolePanel : UIPanel
{

    [SerializeField] private Button toggleButton; // 右侧的切换按钮
    [SerializeField] private RectTransform panelContent; // 需要收纳的面板主体
    [SerializeField] private float animationDuration = 0.3f; // 动画持续时间
    GameObject behavior_prfb;
    Transform select_panel;
    private bool isExpanded = true; // 面板是否展开
    private float contentWidth; // 面板主体宽度
    private Vector2 hiddenPosition; // 隐藏时的位置
    private Vector2 shownPosition; // 显示时的位置
    public string[] Behaviors = new string[] { 
    "MoveTop","MoveDown","MoveLeft","MoveRihgt"
    };

    public override void Initialize()
    {
        base.Initialize();

        if (panelContent == null)
        {
            panelContent = GetComponent<RectTransform>();
        }

        // 记录初始状态为展开
        isExpanded = true;

        // 计算面板主体宽度
        contentWidth = panelContent.rect.width;

        // 记录显示时的位置
        shownPosition = panelContent.anchoredPosition;

        // 计算隐藏时的位置（只露出按钮）
        hiddenPosition = new Vector2(-500,
                                     panelContent.anchoredPosition.y);

        // 绑定按钮事件
        toggleButton.onClick.AddListener(TogglePanel);

        behavior_prfb = Resources.Load<GameObject>("UIprefab/Select_Behavior");
        select_panel = transform.GetChild(2).Find("Select");
        init_behavior();
    }

    // 切换面板展开/收起状态
    private void TogglePanel()
    {
        isExpanded = !isExpanded;
        StopAllCoroutines();

        if (isExpanded)
        {
            StartCoroutine(MovePanel(shownPosition));
        }
        else
        {
            StartCoroutine(MovePanel(hiddenPosition));
        }
    }

    // 面板移动动画
    private IEnumerator MovePanel(Vector2 targetPosition)
    {
        Vector2 startPosition = panelContent.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            // 使用平滑插值使动画更自然
            panelContent.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        // 确保最终位置准确
        panelContent.anchoredPosition = targetPosition;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveListener(TogglePanel);
        }
    }
    public void init_behavior()
    {
        for (int i = 0;i< Behaviors.Length; i++)
        {
            GameObject obj = Instantiate(behavior_prfb, select_panel);
            obj.GetComponent<Behaviors>().behaviorType = Behaviors[i];
            obj.transform.GetChild(0).GetComponent<Text>().text = Behaviors[i];
        }
    }
    public DragSortablePanel get_insPanel()
    {
        DragSortablePanel insPanel = this.transform.GetChild(1).GetChild(1).GetComponent<DragSortablePanel>();

        return insPanel;
    }
}
