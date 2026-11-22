using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] StageManager stageManager;
    [SerializeField] Tower towerInGamePrefab;
    [SerializeField] Transform towerRoot;
    [SerializeField] EnemySpawner enemySpawner;

    void OnEnable()
    {
        stageManager.OnStageFinalized += OnTowerSpawned;
    }

    void OnDisable()
    {
        stageManager.OnStageFinalized -= OnTowerSpawned;
    }

    void OnTowerSpawned(Transform towerTransform)
    {
        Transform inGameTowerTransform = Instantiate(towerInGamePrefab, towerRoot, false).transform;
        inGameTowerTransform.rotation = towerTransform.rotation;

        enemySpawner.SpawnEnemies(stageManager.FinalizedBoundaryPoints, 5);
    }
}
