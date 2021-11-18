using UnityEngine;

public class MainGamePresenter
{
    private MainGameView _view;
    private int _score;

    public MainGamePresenter(MainGameView view)
    {
        _view = view;
    }

    public void OnStartButtonPressed()
    {
        _view.StartGame();
    }

    public void OnRestartButtonPressed()
    {
        _score = 0;
        _view.SetScore(_score);
        OnStartButtonPressed();
    }

    public void OnExitButtonPressed()
    {
        if (Application.isEditor) UnityEditor.EditorApplication.isPlaying = false;
        else Application.Quit();
    }

    public void OnPlayerFoodEaten()
    {
        _score++;
        _view.SetScore(_score);
        _view.SpawnFood();
    }

    public void OnPlayerSnakeDied()
    {
        _view.EndGame();
    }

    public void OnEnemyFoodEaten()
    {
        _view.SpawnFood();
    }

    public void OnLifeGained(int extraLives)
    {
        _view.UpdateExtraBlocksUI(extraLives, true);
    }

    public void OnLifeLost(int extraLives)
    {
        _view.UpdateExtraBlocksUI(extraLives, false);
    }
}
