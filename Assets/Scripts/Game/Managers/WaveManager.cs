using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Zenject;

public class WaveManager
{
    [Inject] private EnemyFactory _enemyFactory;
    [Inject] private WaveConfig _waveConfig;
    [Inject] private PathHolder _pathHolder;
    [Inject] private GameStateMachine _stateMachine;
    [Inject] private PlayerBase _playerBase;

    public event Action<int,int> OnWaveStarted;
    public event Action<int,int> OnWaveEnded;
    public event Action<int> OnEnemiesCountChanged;
    public event Action<float> OnWavePauseStarted;
    
    private int _enemiesAlive;
    private int _currentWave;
    
    public int CurrentWave => _currentWave;
    public int WavesCount => _waveConfig.Waves.Count;
    public int EnemiesAlive => _enemiesAlive;
    
    private CancellationTokenSource _waveCts;

    public async UniTask StartWaveAsync()
    {
        _waveCts = new CancellationTokenSource();
        var cts = _waveCts.Token;

        _playerBase.Initialize();
        await _stateMachine.EnterStateAsync(GameState.Playing, cts);
        
        OnWavePauseStarted?.Invoke(_waveConfig.WavePauseDuration);
        await UniTask.Delay(TimeSpan.FromSeconds(_waveConfig.WavePauseDuration), cancellationToken: cts);
        
        for (int waveIndex = 0; waveIndex < _waveConfig.Waves.Count; waveIndex++)
        {
            _currentWave = waveIndex + 1;
            var wave = _waveConfig.Waves[waveIndex];
            OnWaveStarted?.Invoke(_currentWave,_waveConfig.Waves.Count);
            _enemiesAlive = 0;
            foreach (var group in wave.Groups)
            {
                _enemiesAlive += group.Count;
            }
            OnEnemiesCountChanged?.Invoke(_enemiesAlive);

            foreach (var group in wave.Groups)
            {
                for (int i = 0; i < group.Count; i++)
                {
                    var enemy = _enemyFactory.Create(group.Type, GetPath());
                    enemy.OnDeath += OnEnemyDeath;
                    enemy.MoveAsync(cts);
                    await UniTask.Delay(TimeSpan.FromSeconds(group.SpawnDelay), cancellationToken: cts);
                }
            }

            await UniTask.WaitUntil(() => _enemiesAlive <= 0, cancellationToken: cts);
            OnWaveEnded?.Invoke(_currentWave,_waveConfig.Waves.Count);

            if (waveIndex < _waveConfig.Waves.Count - 1 && !cts.IsCancellationRequested)
            {
                OnWavePauseStarted?.Invoke(_waveConfig.WavePauseDuration);
                await UniTask.Delay(TimeSpan.FromSeconds(_waveConfig.WavePauseDuration), cancellationToken: cts);
            }
        }

        await UniTask.WaitUntil(() => _enemiesAlive == 0 && _playerBase.Health > 0);
        await _stateMachine.EnterStateAsync(GameState.Victory,cts);
    }

    private void StopWaves()
    {
        _waveCts?.Cancel();
    }

    private void OnEnemyDeath()
    {
        _enemiesAlive--;
        OnEnemiesCountChanged?.Invoke(_enemiesAlive);
    }

    private Vector3[] GetPath()
    {
        return _pathHolder.Waypoints;
    }
}