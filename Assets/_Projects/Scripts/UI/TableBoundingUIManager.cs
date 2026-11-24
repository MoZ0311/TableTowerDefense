using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// 机の境界取得時のUI管理クラス
/// </summary>
public class TableBoundingUIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] UIDocument tableBoundingUI;
    [SerializeField] ARSession arSession;

    [Header("Scripts")]
    [SerializeField] MarkerTracking markerTracking;
    [SerializeField] StageManager stageManager;

    Button finalizeButton;  // 平面検出終了用のボタン
    Button resetButton;     // 検出リセット用のボタン

    void OnEnable()
    {
        // rootから各UI要素を検索、取得
        var root = tableBoundingUI.rootVisualElement;
        finalizeButton = root.Q<Button>("FinalizeButton");
        resetButton = root.Q<Button>("ResetButton");

        // イベント登録
        AddEvent();
    }

    void OnDisable()
    {
        // イベント解除
        RemoveEvent();
    }

    void Update()
    {
        if (stageManager == null)
        {
            return;
        }
        finalizeButton.SetEnabled(stageManager.CanFinalizeStage);
    }

    /// <summary>
    /// イベントの登録処理
    /// </summary>
    void AddEvent()
    {
        // 各VisualElementのnullチェック
        if (finalizeButton == null ||
            resetButton == null)
        {
            Debug.LogError("VisualElement is null");
            return;
        }

        // クリック時イベントの登録
        finalizeButton.clicked += OnClickedFinalize;
        resetButton.clicked += OnClickedReset;
    }

    /// <summary>
    /// イベントの解除処理
    /// </summary>
    void RemoveEvent()
    {
        // 各VisualElementのnullチェック
        if (finalizeButton == null ||
            resetButton == null)
        {
            Debug.LogError("VisualElement is null");
            return;
        }

        // クリック時イベントの解除
        finalizeButton.clicked -= OnClickedFinalize;
        resetButton.clicked -= OnClickedReset;
    }

    /// <summary>
    /// finalize Buttonクリック時、ステージの外形を確定させる処理
    /// </summary>
    void OnClickedFinalize()
    {
        if (stageManager == null)
        {
            Debug.LogError("StageManager is null");
            return;
        }

        stageManager.FinalizeStage();
    }

    /// <summary>
    /// リセットボタンクリック時、平面検出をやり直すための処理
    /// </summary>
    void OnClickedReset()
    {
        markerTracking.enabled = false;
        arSession.Reset();
        markerTracking.enabled = true;
    }
}
