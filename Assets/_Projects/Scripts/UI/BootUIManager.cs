using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// 起動時のUI管理クラス
/// </summary>
public class BootUIManager : MonoBehaviour
{
    [SerializeField] UIDocument bootUI;

    Button planeDetection;  // PlaneDetection Scene遷移用のボタン
    Button imageTracking;   // ImageTracking Scene遷移用のボタン
    Button tableBounding;   // TableBounding Scene遷移用のボタン

    void OnEnable()
    {
        // rootから各UI要素を検索、取得
        var root = bootUI.rootVisualElement;
        planeDetection = root.Q<Button>("plane");
        imageTracking = root.Q<Button>("image");
        tableBounding = root.Q<Button>("bounding");

        // クリック時イベントの登録
        planeDetection.clicked += OnClickedPlaneDetection;
        imageTracking.clicked += OnClickedImageTrancking;
        tableBounding.clicked += OnClickedTableBounding;
    }

    void OnDisable()
    {
        // クリック時イベントの解除
        planeDetection.clicked -= OnClickedPlaneDetection;
        imageTracking.clicked -= OnClickedImageTrancking;
        tableBounding.clicked -= OnClickedTableBounding;
    }

    /// <summary>
    /// planeDetection Buttonクリック時、対応シーンへの遷移処理
    /// </summary>
    void OnClickedPlaneDetection()
    {
        SceneManager.LoadScene("PlaneDetection");
    }

    /// <summary>
    /// imageTrancking Buttonクリック時、対応シーンへの遷移処理
    /// </summary>
    void OnClickedImageTrancking()
    {
        SceneManager.LoadScene("ImageTracking");
    }

    /// <summary>
    /// tableBounding Buttonクリック時、対応シーンへの遷移処理
    /// </summary>
    void OnClickedTableBounding()
    {
        SceneManager.LoadScene("TableBounding");
    }
}
