using Cysharp.Threading.Tasks;

public interface ITower
{
    void Initialize(TowerConfig config);
    UniTask AttackLoopAsync();
}
