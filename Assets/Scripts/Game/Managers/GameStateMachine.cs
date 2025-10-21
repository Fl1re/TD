using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


public enum GameState
{
    Menu,
    Playing,
    WaveInProgress,
    Paused,
    GameOver
}
public class GameStateMachine
{
    public GameState CurrentState { get; private set; }

    public event Action<GameState> OnStateChanged;

    public GameStateMachine()
    {
        CurrentState = GameState.Menu;
    }

    public async UniTask EnterStateAsync(GameState newState)
    {
        if (CurrentState == newState) return;

        await ExitCurrentStateAsync();

        CurrentState = newState;

        await EnterNewStateAsync();

        OnStateChanged?.Invoke(CurrentState);
    }

    private async UniTask ExitCurrentStateAsync()
    {
        switch (CurrentState)
        {
            case GameState.WaveInProgress:
                await UniTask.CompletedTask;
                break;
        }
    }

    private async UniTask EnterNewStateAsync()
    {
        switch (CurrentState)
        {
            case GameState.Menu:
                // Показать меню UI
                await UniTask.CompletedTask;
                break;
            case GameState.Playing:
                await UniTask.CompletedTask;
                break;
            case GameState.WaveInProgress:
                // Запустить волну
                await UniTask.CompletedTask;
                break;
            case GameState.Paused:
                // Пауза, показать UI улучшений
                await UniTask.CompletedTask;
                break;
            case GameState.GameOver:
                // Показать GameOver экран
                await UniTask.CompletedTask;
                break;
        }
    }
}
