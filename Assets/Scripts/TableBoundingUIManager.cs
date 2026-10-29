using UnityEngine;
using UnityEngine.UIElements;

public class TableBoundingUIManager : MonoBehaviour
{
    [SerializeField] UIDocument tableBoundingUI;

    Button finalizeButton;

    void OnEnable()
    {
        var root = tableBoundingUI.rootVisualElement;
        finalizeButton = root.Q<Button>();

        finalizeButton.clicked += OnClickedFinalize;
    }

    void OnDisable()
    {
        finalizeButton.clicked -= OnClickedFinalize;
    }

    void OnClickedFinalize()
    {
        Debug.Log("FINALIZE!!");
    }
}
