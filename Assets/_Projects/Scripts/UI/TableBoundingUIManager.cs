using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 机の境界取得時のUI管理クラス
/// </summary>
public class TableBoundingUIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] UIDocument tableBoundingUI;

    Button finalizeButton;  // 平面検出終了用のボタン

    void OnEnable()
    {
        // rootから各UI要素を検索、取得
        var root = tableBoundingUI.rootVisualElement;
        finalizeButton = root.Q<Button>();

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
        if (finalizeButton == null)
        {
            Debug.LogError("VisualElement is null");
            return;
        }

        // クリック時イベントの登録
        finalizeButton.clicked += OnClickedFinalize;
    }

    /// <summary>
    /// イベントの解除処理
    /// </summary>
    void RemoveEvent()
    {
        // 各VisualElementのnullチェック
        if (finalizeButton == null)
        {
            Debug.LogError("VisualElement is null");
            return;
        }

        // クリック時イベントの解除
        finalizeButton.clicked -= OnClickedFinalize;
    }

    /// <summary>
    /// finalize Buttonクリック時、ステージの外形を確定させる処理
    /// </summary>
    void OnClickedFinalize()
    {
        Debug.Log("FINALIZE!!");
    }
}
