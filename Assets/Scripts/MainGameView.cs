using UnityEngine;
using UnityEngine.UI;

public class MainGameView : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private PlayerSnakeBehavior playerSnake;
    [SerializeField] private EnemySnakeBehavior enemySnake;
    [SerializeField] private Text scoreCounter;
    [SerializeField] private BoxCollider2D spawnArea;
    [SerializeField] private GameObject commonFood;
    [SerializeField] private GameObject specialFood;

    private MainGamePresenter _presenter;
    private Bounds _spawnBounds;
    private Transform _spawnedFood;

    private void Awake()
    {
        _presenter = new MainGamePresenter(this);
    }

    private void Start()
    {
        startButton.onClick.AddListener(() => _presenter.OnStartButtonPressed());
        restartButton.onClick.AddListener(() => _presenter.OnRestartButtonPressed());
        exitButton.onClick.AddListener(() => _presenter.OnExitButtonPressed());

        playerSnake.AddFoodEatenListener(() => _presenter.OnPlayerFoodEaten());
        playerSnake.AddSnakeDiedListener(() => _presenter.OnPlayerSnakeDied());
        enemySnake.AddFoodEatenListener(() => _presenter.OnEnemyFoodEaten());

        _spawnBounds = spawnArea.bounds;
        spawnArea.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        Cursor.visible = false;
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);

        SpawnFood();
        playerSnake.SpawnSnake();
        enemySnake.SpawnSnake();
    }

    public void SetScore(int score)
    {
        scoreCounter.text = $"{score:0000}";
    }

    public void SpawnFood()
    {
        var xPosition = (float) Mathf.Round(Random.Range(_spawnBounds.min.x, _spawnBounds.max.x));
        var yPosition = (float) Mathf.Round(Random.Range(_spawnBounds.min.y, _spawnBounds.max.y));
        var spawnPosition = new Vector3(xPosition, yPosition, 0);
        // Has a 10% chance of spawning a special food instead of a common one
        var shouldSpawnSpecialFood = Random.Range(0, 10) == 0;

        _spawnedFood = Instantiate(shouldSpawnSpecialFood ? specialFood : commonFood).transform;
        _spawnedFood.position = spawnPosition;
        enemySnake.SetFoodTarget(_spawnedFood);
        playerSnake.IsSpecialFoodSpawned = shouldSpawnSpecialFood;
        enemySnake.IsSpecialFoodSpawned = shouldSpawnSpecialFood;
    }

    public void EndGame()
    {
        playerSnake.DestroySnake();
        enemySnake.DestroySnake();
        Destroy(_spawnedFood.gameObject);
        gameOverMenu.SetActive(true);
        Cursor.visible = true;
    }
}
