using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class AOEProjectile : Projectile
{
    [SerializeField] private float aoeRadius;
    [SerializeField] private VisualEffect magicEffect;

    protected override async UniTask ApplyDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);

        var enemies = hits.Select(c => c.GetComponent<IEnemy>()).Where(e => e != null && e.Health > 0).ToList();

        var effect = Instantiate(magicEffect, (_enemy as MonoBehaviour).transform.position, Quaternion.identity);
        effect.Play();
        
        foreach (var enemy in enemies)
        {
            enemy.TakeDamage(_damage);
        }

        await UniTask.CompletedTask;
    }
}
