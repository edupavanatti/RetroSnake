using UnityEngine;
using UnityEngine.UI;

public class MainGameView : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject blocksUI;
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private PlayerSnakeBehavior playerSnake;
    [SerializeField] private EnemySnakeBehavior enemySnake;
    [SerializeField] private Text scoreCounter;
    [SerializeField] private BoxCollider2D spawnArea;
    [SerializeField] private GameObject commonFood;
    [SerializeField] private GameObject speedFood;
    [SerializeField] private GameObject lifeFood;
    [SerializeField] private Image[] extraBlocks;
    [SerializeField] private Color enabledColor;
    [SerializeField] private Color disabledColor;

    private MainGamePresenter _presenter;
    private Bounds _spawnBounds;
    private Transform _spawnedFood;

    private void Awake()
    {
        _presenter = new MainGamePresenter(this);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        startButton.onClick.AddListener(() => _presenter.OnStartButtonPressed());
        restartButton.onClick.AddListener(() => _presenter.OnRestartButtonPressed());
        exitButton.onClick.AddListener(() => _presenter.OnExitButtonPressed());

        playerSnake.AddFoodEatenListener(() => _presenter.OnPlayerFoodEaten());
        playerSnake.AddSnakeDiedListener(() => _presenter.OnPlayerSnakeDied());
        playerSnake.AddLifeGainedListener((int extraLives) => _presenter.OnLifeGained(extraLives));
        playerSnake.AddLifeLostListener((int extraLives) => _presenter.OnLifeLost(extraLives));
        enemySnake.AddFoodEatenListener(() => _presenter.OnEnemyFoodEaten());

        _spawnBounds = spawnArea.bounds;
        spawnArea.gameObject.SetActive(false);
        startButton.Select();
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        blocksUI.SetActive(true);

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
        // Generates a random spawn position
        var xPosition = (float) Mathf.Round(Random.Range(_spawnBounds.min.x, _spawnBounds.max.x));
        var yPosition = (float) Mathf.Round(Random.Range(_spawnBounds.min.y, _spawnBounds.max.y));
        var spawnPosition = new Vector3(xPosition, yPosition, 0);

        // Defines the type of food to be spawned
        var foodType = commonFood;
        var foodSpawnChance = Random.Range(0, 10);
        if (foodSpawnChance == 0) foodType = lifeFood;
        else if (foodSpawnChance == 1 || foodSpawnChance == 2) foodType = speedFood;

        _spawnedFood = Instantiate(foodType).transform;
        _spawnedFood.position = spawnPosition;

        playerSnake.SetFoodTarget(_spawnedFood);
        enemySnake.SetFoodTarget(_spawnedFood);
    }

    public void EndGame()
    {
        playerSnake.DestroySnake();
        enemySnake.DestroySnake();
        Destroy(_spawnedFood.gameObject);
        blocksUI.SetActive(false);
        gameOverMenu.SetActive(true);
        restartButton.Select();
    }

    public void UpdateExtraBlocksUI(int extraLives, bool wasLifeGained)
    {
        if (wasLifeGained)
        {
            extraBlocks[extraLives].color = enabledColor;
        }
        else
        {
            for (int index = extraBlocks.Length - 1; index >= extraLives; index--)
            {
                extraBlocks[index].color = disabledColor;
            }
        }
    }
}
