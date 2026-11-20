using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] UIDocument gameUI;
    [SerializeField] CannonController cannonController;
    Button fireButton;

    void OnEnable()
    {
        // rootから各UI要素を検索、取得
        var root = gameUI.rootVisualElement;
        fireButton = root.Q<Button>();

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
        if (fireButton == null)
        {
            Debug.LogError("VisualElement is null");
            return;
        }

        // クリック時イベントの登録
        fireButton.clicked += OnClickedFireButton;
    }

    /// <summary>
    /// イベントの解除処理
    /// </summary>
    void RemoveEvent()
    {
        // 各VisualElementのnullチェック
        if (fireButton == null)
        {
            Debug.LogError("VisualElement is null");
            return;
        }

        // クリック時イベントの解除
        fireButton.clicked -= OnClickedFireButton;
    }

    void OnClickedFireButton()
    {
        if (cannonController == null)
        {
            Debug.LogError("CannonController is null");
            return;
        }

        cannonController.LaunchProjectile();
    }
}