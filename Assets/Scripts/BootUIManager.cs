using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BootUIManager : MonoBehaviour
{
    [SerializeField] UIDocument bootUI;

    Button planeDetection;
    Button imageTracking;
    Button tableBounding;

    void OnEnable()
    {
        var root = bootUI.rootVisualElement;
        planeDetection = root.Q<Button>("plane");
        imageTracking = root.Q<Button>("image");
        tableBounding = root.Q<Button>("bounding");

        planeDetection.clicked += OnClickedPlaneDetection;
        imageTracking.clicked += OnClickedImageTrancking;
        tableBounding.clicked += OnClickedTableBounding;
    }

    void OnDisable()
    {
        planeDetection.clicked -= OnClickedPlaneDetection;
        imageTracking.clicked -= OnClickedImageTrancking;
        tableBounding.clicked -= OnClickedTableBounding;
    }

    void OnClickedPlaneDetection()
    {
        SceneManager.LoadScene("PlaneDetection");
    }

    void OnClickedImageTrancking()
    {
        SceneManager.LoadScene("ImageTracking");
    }

    void OnClickedTableBounding()
    {
        SceneManager.LoadScene("TableBounding");
    }
}
