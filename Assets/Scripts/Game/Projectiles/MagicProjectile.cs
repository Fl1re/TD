using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class MagicProjectile : Projectile
{
    [SerializeField] private float radiusAttack;
    [SerializeField] private VisualEffect magicEffect;
    private const float DamageDivider = 2;

    private float currentDamage;

    protected override async UniTask ApplyDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radiusAttack);

        currentDamage = _damage;

        var enemies = hits.Select(c => c.GetComponent<IEnemy>()).Where(e => e != null && e.Health > 0).ToList();
        
        Debug.Log(enemies.Count);
        
        foreach (var enemy in enemies)
        {
            var effect = Instantiate(magicEffect, (enemy as MonoBehaviour).transform.position,Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject,1f);
            enemy.TakeDamage(currentDamage);
            currentDamage /= DamageDivider;

            await UniTask.WaitForSeconds(0.2f);
        }

        await UniTask.CompletedTask;
    }
}
