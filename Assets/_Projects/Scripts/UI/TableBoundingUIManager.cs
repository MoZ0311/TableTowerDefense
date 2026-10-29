using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 机の境界取得時のUI管理クラス
/// </summary>
public class TableBoundingUIManager : MonoBehaviour
{
    [SerializeField] UIDocument tableBoundingUI;

    Button finalizeButton;

    void OnEnable()
    {
        // rootから各UI要素を検索、取得
        var root = tableBoundingUI.rootVisualElement;
        finalizeButton = root.Q<Button>();

        // クリック時イベントの登録
        finalizeButton.clicked += OnClickedFinalize;
    }

    void OnDisable()
    {
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
