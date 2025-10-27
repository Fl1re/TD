using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    Menu,
    Playing,
    WaveInProgress,
    Paused,
    GameOver,
    Victory
}
public class GameStateMachine
{
    public GameState CurrentState { get; private set; }

    public event Action<GameState> OnStateChanged;

    public GameStateMachine()
    {
        CurrentState = GameState.Menu;
    }

    public async UniTask EnterStateAsync(GameState state, CancellationToken ct = default)
    {
        CurrentState = state;
        OnStateChanged?.Invoke(state);

        switch (state)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
            case GameState.GameOver:
            case GameState.Victory:
                Time.timeScale = 0f;
                break;
        }
    }

    public void Pause() => EnterStateAsync(GameState.Paused).Forget();

    public void Resume() => EnterStateAsync(GameState.Playing).Forget();

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
