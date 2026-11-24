using UnityEngine;
using UnityEngine.UIElements;

public class TowerUIManager : MonoBehaviour
{
    [SerializeField] UIDocument towerUI;
    [SerializeField] Tower tower;
    Camera mainCamera;
    VisualElement fill;

    void Start()
    {
        mainCamera = Camera.main;
        towerUI.enabled = true;

        var root = towerUI.rootVisualElement;
        fill = root.Q<VisualElement>("Fill");

        if (fill == null)
        {
            Debug.LogError("VisualElement is null");
        }
    }

    void Update()
    {
        fill.style.flexGrow = tower.HealthRatio;
    }

    void LateUpdate()
    {
        transform.rotation = mainCamera.transform.rotation;
    }
}
