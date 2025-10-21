
public class BasicTower : Tower
{
    public override void Attack(IEnemy target)
    {
        var projectileGO = Instantiate(_config.ProjectilePrefab.gameObject, firePoint.position, firePoint.rotation);
        var projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(target, Damage, ProjectileSpeed, isArc: true);
        }
    }
}
