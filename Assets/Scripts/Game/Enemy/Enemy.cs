using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour, IEnemy
{
    private EnemyConfig _config;
    [Inject] private EconomyManager _economyManager;
    [Inject] private PlayerBase _playerBase;

    private Vector3[] _path;
    private int _currentWaypointIndex;
    private float _health;

    public float Health => _health;
    public event Action OnDeath;
    public event Action OnHealthChange;
    
    private CancellationTokenSource _cts;

    public void Initialize(Vector3[] path,EnemyConfig config)
    {
        _path = path;
        _config = config;
        _health = _config.Health;
        _currentWaypointIndex = 0;

        _cts = new CancellationTokenSource();
    }
    public async UniTask MoveAsync()
    {
        while (_currentWaypointIndex < _path.Length)
        {
            Vector3 target = _path[_currentWaypointIndex];
            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    _config.Speed * Time.deltaTime
                );
                await UniTask.Yield(cancellationToken: _cts.Token);
            }
            _currentWaypointIndex++;
        }
        _playerBase.TakeDamage(_config.Damage);
        OnEnemyDeath();
        await UniTask.CompletedTask;
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        OnHealthChange?.Invoke();
        if (_health <= 0)
        {
            _economyManager.AddResources(_config.Reward);
            OnEnemyDeath();
        }
    }

    private void OnEnemyDeath()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
    }
}