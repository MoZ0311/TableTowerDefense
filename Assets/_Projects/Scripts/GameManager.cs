using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        None,
        Bounding,
        Game,
        Result
    }

    [SerializeField] StageManager stageManager;
    [SerializeField] Tower towerInGamePrefab;
    [SerializeField] Transform towerRoot;
    [SerializeField] EnemySpawner enemySpawner;

    [Header("UI")]
    [SerializeField] GameObject gameUI;
    [SerializeField] GameObject tableBoundingUI;
    [SerializeField] GameObject resultUI;

    GameState gameState;
    Tower tower = null;

    void OnEnable()
    {
        stageManager.OnStageFinalized += StartGame;

        if (tower != null)
        {
            tower.OnTowerDestroyed += FinishGame;
        }
    }

    void OnDisable()
    {
        stageManager.OnStageFinalized -= StartGame;

        if (tower != null)
        {
            tower.OnTowerDestroyed -= FinishGame;
        }
    }

    void Start()
    {
        gameState = GameState.Bounding;
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.Bounding:
                gameUI.SetActive(false);
                tableBoundingUI.SetActive(true);
                resultUI.SetActive(false);
                break;

            case GameState.Game:
                gameUI.SetActive(true);
                tableBoundingUI.SetActive(false);
                resultUI.SetActive(false);
                break;

            case GameState.Result:
                gameUI.SetActive(false);
                tableBoundingUI.SetActive(false);
                resultUI.SetActive(true);
                break;
            default:
                return;
        }
    }

    void StartGame(Transform towerTransform)
    {
        // ゲーム用のタワーを生成
        tower = Instantiate(towerInGamePrefab, towerRoot, false);
        tower.transform.rotation = towerTransform.rotation;

        tower.OnTowerDestroyed += FinishGame;

        // ゲームの開始処理
        gameState = GameState.Game;

        SpawnEnemies(1);
    }

    void FinishGame()
    {
        tower.OnTowerDestroyed -= FinishGame;
        gameState = GameState.Result;
    }

    void UpdateGame()
    {
        // Time.t
    }

    void SpawnEnemies(int spawnCount)
    {
        enemySpawner.SpawnEnemies(stageManager.FinalizedBoundaryPoints, spawnCount);
    }
}
