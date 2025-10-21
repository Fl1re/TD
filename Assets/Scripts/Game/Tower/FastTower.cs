using Zenject;

public class FastTower : Tower
{
    [Inject] private EconomyManager _economyManager;

    public override void Attack(IEnemy target)
    {
        base.Attack(target);
    }
}
