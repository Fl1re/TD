using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    private CancellationTokenSource _cts;

    protected IEnemy _enemy;
    protected float _damage;
    private float _speed;
    private bool _isArc;
    private readonly float _arcHeight = 5f;
    
    public virtual void Initialize(IEnemy target, float damage, float speed, bool isArc = false)
    {
        _cts = new CancellationTokenSource();
        _enemy = target;
        _damage = damage;
        _speed = speed;
        _isArc = isArc;
        _ = ReachTargetLoop();
    }

    protected virtual async UniTask ReachTargetLoop()
    {
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        Vector3 endPos = Vector3.zero;

        while (!_cts.IsCancellationRequested)
        {
            if (_enemy != null && _enemy.Health > 0)
            {
                var enemyTransform = (_enemy as MonoBehaviour)?.transform;
                if (enemyTransform == null) break;

                endPos = enemyTransform.position;

                if (_isArc)
                {
                    float dist = Vector3.Distance(startPos, endPos);
                    float duration = dist / _speed;
                    float t = (Time.time - startTime) / duration;
                    if (t >= 1f)
                    {
                        await ApplyDamage();
                        _cts.Cancel();
                        Destroy(gameObject);
                        break;
                    }

                    Vector3 lerpedPos = Vector3.Lerp(startPos, endPos, t);
                    lerpedPos.y += _arcHeight * Mathf.Sin(Mathf.PI * t);
                    transform.position = lerpedPos;
                    transform.LookAt(endPos);
                }
                else
                {
                    transform.LookAt(enemyTransform);
                    var newPos = Vector3.MoveTowards(transform.position, enemyTransform.position, _speed * Time.deltaTime);
                    transform.position = newPos;

                    if (Vector3.Distance(transform.position, enemyTransform.position) < 0.1f)
                    {
                        await ApplyDamage();
                        _cts.Cancel();
                        Destroy(gameObject);
                        break;
                    }
                }
            }
            else
            {
                _cts.Cancel();
                Destroy(gameObject);
                break;
            }
            await UniTask.Yield(cancellationToken: _cts.Token);
        }
    }
    
    protected virtual async UniTask ApplyDamage()
    {
        if (_enemy != null)
        {
            _enemy.TakeDamage(_damage);
        }
        await UniTask.CompletedTask;
    }
}
