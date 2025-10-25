using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Threading;
using Zenject;

public abstract class Tower : MonoBehaviour, ITower, IAttackable
{
    [SerializeField] private Transform horizontalRotator;
    [SerializeField] private Transform rifflesParent;
    [SerializeField] public Transform firePoint;
    
    [Inject] private EconomyManager _economyManager;

    private float _damage;
    private float _projectileSpeed;
    private float _attackRadius;
    private float _attackInterval;
    private float _rotationSpeed;
    private Projectile _projectilePrefab;

    public TowerConfig _config;
    protected IEnemy _currentTarget;

    private bool _isAttacking;
    private CancellationTokenSource _cts;

    private int _currentLevelIndex = 0;

    public float Damage => _damage;
    public float AttackInterval => _attackInterval;
    public float AttackRadius => _attackRadius;
    public float ProjectileSpeed => _projectileSpeed;
    public int CurrentLevel => _currentLevelIndex + 1;
    public int LevelIndex => _currentLevelIndex;
    public int MaxLevel => _config.UpgradeLevels.Count + 1;

    public virtual void Initialize(TowerConfig config)
    {
        _config = config;
        _projectilePrefab = _config.ProjectilePrefab;
        ApplyLevel(_config.BaseLevel);
        _cts = new CancellationTokenSource();
        _ = TargetSearchLoopAsync();
        _ = AttackLoopAsync();
        _ = RotateLoopAsync();
    }
    
    private void ApplyLevel(TowerConfig.UpgradeLevel level)
    {
        _damage = level.Damage;
        _projectileSpeed = level.ProjectileSpeed;
        _attackRadius = level.AttackRadius;
        _attackInterval = level.AttackInterval;
        _rotationSpeed = level.RotationSpeed;
        ChangeRiffleOnUpgrade(_currentLevelIndex);
    }

    private bool CanUpgrade() => _currentLevelIndex < _config.UpgradeLevels.Count;
    public int GetUpgradeCost() => CanUpgrade() ? _config.UpgradeLevels[_currentLevelIndex].UpgradeCost : 0;
    
    public void Upgrade()
    {
        if (!CanUpgrade()) return;

        int cost = GetUpgradeCost();
        if (_economyManager.SpendResources(cost))
        {
            var nextLevel = _config.UpgradeLevels[_currentLevelIndex];
            _currentLevelIndex++;
            ApplyLevel(nextLevel);
        }
    }

    private void ChangeRiffleOnUpgrade(int level)
    {
        if(level >= MaxLevel) return;
        
        foreach (Transform riffle in rifflesParent.transform)
        {
            riffle.gameObject.SetActive(false);
        }
        rifflesParent.GetChild(level).gameObject.SetActive(true);
    }

    protected virtual async UniTask TargetSearchLoopAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            if (_currentTarget == null || _currentTarget.Health <= 0 || IsOutOfRange(_currentTarget))
            {
                if (_currentTarget != null)
                {
                    _currentTarget.OnDeath -= OnTargetDeath;
                    _currentTarget = null;
                }
                _currentTarget = FindNearestEnemy();
                if (_currentTarget != null)
                {
                    _currentTarget.OnDeath += OnTargetDeath;
                }
            }
            await UniTask.Yield(cancellationToken: _cts.Token);
        }
    }
    
    protected bool IsOutOfRange(IEnemy target)
    {
        if (target == null) return true;
        Transform targetTransform = (target as MonoBehaviour)?.transform;
        if (targetTransform == null) return true;
        return Vector3.Distance(transform.position, targetTransform.position) > _attackRadius;
    }

    public virtual async UniTask AttackLoopAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            if (_currentTarget != null && !_isAttacking)
            {
                _isAttacking = true;
                Attack(_currentTarget);
                await UniTask.Delay(TimeSpan.FromSeconds(_attackInterval), cancellationToken: _cts.Token);
                _isAttacking = false;
            }
            await UniTask.Yield(cancellationToken: _cts.Token);
        }
    }

    private void OnTargetDeath()
    {
        if (_currentTarget != null)
        {
            _currentTarget.OnDeath -= OnTargetDeath;
            _currentTarget = null;
        }
    }

    protected virtual async UniTask RotateLoopAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            if (_currentTarget != null && _currentTarget.Health <= 0)
            {
                _currentTarget = null;
            }
            if (_currentTarget != null)
            {
                LookAtTarget(_currentTarget);
            }
            else
            {
                SetRotatorBack();
            }
            await UniTask.Yield(cancellationToken: _cts.Token);
        }
    }

    protected virtual void LookAtTarget(IEnemy target)
    {
        if (target == null) return;
        Transform targetTransform = (target as MonoBehaviour)?.transform;
        if (targetTransform == null) return;

        if (horizontalRotator != null)
        {
            var lookPos = targetTransform.position - horizontalRotator.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            horizontalRotator.rotation = Quaternion.Slerp(horizontalRotator.rotation, rotation, Time.deltaTime * _rotationSpeed);
        }
    }

    private void SetRotatorBack()
    {
        if(horizontalRotator)
            horizontalRotator.rotation = Quaternion.Slerp(horizontalRotator.rotation, Quaternion.Euler(0,0,0), Time.deltaTime * _rotationSpeed);
    }

    public virtual void Attack(IEnemy target)
    {
        var projectileGO = Instantiate(_projectilePrefab.gameObject, firePoint.position, firePoint.rotation);
        var projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(target, _damage, _projectileSpeed);
        }
    }

    protected IEnemy FindNearestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _attackRadius);
        IEnemy nearest = hits
            .Select(c => c.GetComponent<IEnemy>())
            .Where(e => e != null && e.Health > 0)
            .OrderBy(e => Vector3.Distance(transform.position, ((MonoBehaviour)e).transform.position))
            .FirstOrDefault();
        return nearest;
    }

    protected virtual void OnDestroy()
    {
        _cts?.Cancel();
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green; 

        Gizmos.DrawWireSphere(transform.position, _attackRadius); 
    }
}