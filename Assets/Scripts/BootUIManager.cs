using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BootUIManager : MonoBehaviour
{
    [SerializeField] UIDocument bootUI;

    Button planeDetection;
    Button imageTracking;

    void OnEnable()
    {
        var root = bootUI.rootVisualElement;
        planeDetection = root.Q<Button>("plane");
        imageTracking = root.Q<Button>("image");

        planeDetection.clicked += OnClickedPlaneDetection;
        imageTracking.clicked += OnClickedImageTrancking;
    }

    void OnDisable()
    {
        planeDetection.clicked -= OnClickedPlaneDetection;
        imageTracking.clicked -= OnClickedImageTrancking;
    }

    void OnClickedPlaneDetection()
    {
        SceneManager.LoadScene("PlaneDetection");
    }

    void OnClickedImageTrancking()
    {
        SceneManager.LoadScene("ImageTracking");
    }
}
