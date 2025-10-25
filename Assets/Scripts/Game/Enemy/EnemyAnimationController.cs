using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator _animator;
    private Enemy _enemy;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemy = GetComponent<Enemy>();

        _enemy.OnDeath += OnDeathAnimation;
    }

    private void OnDeathAnimation()
    {
        _animator.SetTrigger("Die");
    }

    private void OnDestroy()
    {
        if (_enemy != null)
        {
            _enemy.OnDeath -= OnDeathAnimation;
        }
    }
}
