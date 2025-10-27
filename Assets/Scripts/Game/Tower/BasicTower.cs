
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BasicTower : Tower
{
    private int ProjectileCount => CurrentLevel;
    public override async void Attack(IEnemy target)
    {
        for (int i = 0; i < ProjectileCount; i++)
        {
            var projectileGO = Instantiate(_config.ProjectilePrefab.gameObject, firePoint.position, firePoint.rotation);
            var projectile = projectileGO.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(target, Damage, ProjectileSpeed, isArc: true);
            }

            await UniTask.WaitForSeconds(0.2f);
        }
    }
}
