using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IEnemy : IMovable,IDamageable
{
    void Initialize(Vector3[] path,EnemyConfig config);
    UniTask MoveAsync();
}
