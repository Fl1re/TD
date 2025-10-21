using UnityEngine;
using Zenject;

public class PlayerBase : BaseEntity
{
    [Inject] private GameStateMachine _gameStateMachine;

    public override void Initialize()
    {
        base.Initialize();
        OnDeath += OnBaseDestroyed;
    }

    private void OnBaseDestroyed()
    {
        _gameStateMachine.EnterStateAsync(GameState.GameOver);
    }

    protected void OnDestroy()
    {
        OnDeath -= OnBaseDestroyed;
    }
}
