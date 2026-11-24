using UnityEngine;
using UnityEngine.UIElements;
using Scene = SceneChanger.Scene;

public class ResultUIManager : MonoBehaviour
{
    [SerializeField] UIDocument resultUI;
    Label resultLabel;
    Button titleButton;
    Button retryButton;

    void OnEnable()
    {
        // rootから各UI要素を検索、取得
        var root = resultUI.rootVisualElement;
        resultLabel = root.Q<Label>();
        titleButton = root.Q<Button>("TitleButton");
        retryButton = root.Q<Button>("RetryButton");

        // イベント登録
        AddEvent();
    }

    void OnDisable()
    {
        // イベント解除
        RemoveEvent();
    }

    /// <summary>
    /// イベントの登録処理
    /// </summary>
    void AddEvent()
    {
        // 各VisualElementのnullチェック
        if (titleButton == null ||
            retryButton == null)
        {
            Debug.LogError("VisualElement is null");
            return;
        }

        // クリック時イベントの登録
        titleButton.clicked += OnClickedTitleButton;
        retryButton.clicked += OnClickedRetryButton;

    }

    /// <summary>
    /// イベントの解除処理
    /// </summary>
    void RemoveEvent()
    {
        // 各VisualElementのnullチェック
        if (titleButton == null ||
            retryButton == null)
        {
            Debug.LogError("VisualElement is null");
            return;
        }

        // クリック時イベントの解除
        titleButton.clicked -= OnClickedTitleButton;
        retryButton.clicked -= OnClickedRetryButton;
    }

    void OnClickedTitleButton()
    {
        SceneChanger.LoadScene(Scene.Boot);
    }

    void OnClickedRetryButton()
    {
        SceneChanger.LoadScene(Scene.Game);
    }
}
