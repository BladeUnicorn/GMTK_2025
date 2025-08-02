using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//// 示例面板 - 主菜单
//public class MainMenuPanel : UIPanel
//{
//    [SerializeField] private Button startButton;
//    [SerializeField] private Button settingsButton;
//    [SerializeField] private Button exitButton;

//    public override void Initialize()
//    {
//        base.Initialize();

//        // 绑定按钮事件
//        startButton.onClick.AddListener(OnStartButtonClicked);
//        settingsButton.onClick.AddListener(OnSettingsButtonClicked);
//        exitButton.onClick.AddListener(OnExitButtonClicked);
//    }

//    private void OnStartButtonClicked()
//    {
//        // 隐藏当前面板并显示游戏面板
//        //UIManager.Instance.HidePanel<MainMenuPanel>();
//        UIManager.Instance.ShowPanel<ConsolePanel>();
//    }

//    private void OnSettingsButtonClicked()
//    {
//        // 显示设置面板（弹窗层级）
//        //UIManager.Instance.ShowPanel<SettingsPanel>();
//    }

//    private void OnExitButtonClicked()
//    {
//        // 显示确认退出面板
//        //UIManager.Instance.ShowPanel<ConfirmExitPanel>();
//    }

//    protected override void OnDestroy()
//    {
//        base.OnDestroy();

//        // 移除事件监听，防止内存泄漏
//        startButton.onClick.RemoveListener(OnStartButtonClicked);
//        settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
//        exitButton.onClick.RemoveListener(OnExitButtonClicked);
//    }
//}

//// 示例面板 - 游戏面板
//public class GamePanel : UIPanel
//{
//    [SerializeField] private Button pauseButton;
//    [SerializeField] private Text scoreText;

//    private int currentScore = 0;

//    public override void Initialize()
//    {
//        base.Initialize();

//        pauseButton.onClick.AddListener(OnPauseButtonClicked);

//        // 模拟分数更新
//        InvokeRepeating("UpdateScore", 1f, 1f);
//    }

//    private void UpdateScore()
//    {
//        currentScore += 10;
//        scoreText.text = $"分数: {currentScore}";
//    }

//    private void OnPauseButtonClicked()
//    {
//        //UIManager.Instance.ShowPanel<PausePanel>();
//    }

//    protected override void OnDestroy()
//    {
//        base.OnDestroy();
//        pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
//        CancelInvoke("UpdateScore");
//    }
//}

// 示例面板 - 游戏面板
public class ConsolePanel : UIPanel
{
    //[SerializeField] private Button foldButton;
    //[SerializeField] private float moveWidth = 1100f;
    //public override void Initialize()
    //{
    //    base.Initialize();

    //    // 绑定按钮事件
    //    foldButton.onClick.AddListener(OnFoldButtonClicked);

    //}

    //private void OnFoldButtonClicked()
    //{
    //    Debug.Log("触发onfoadbtn");
    //    //UIManager.Instance.HidePanel<ConsolePanel>();
    //}
    [SerializeField] private Button toggleButton; // 右侧的切换按钮
    [SerializeField] private RectTransform panelContent; // 需要收纳的面板主体
    [SerializeField] private float animationDuration = 0.3f; // 动画持续时间

    private bool isExpanded = true; // 面板是否展开
    private float contentWidth; // 面板主体宽度
    private Vector2 hiddenPosition; // 隐藏时的位置
    private Vector2 shownPosition; // 显示时的位置

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
}
