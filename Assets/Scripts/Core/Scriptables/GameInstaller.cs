using UnityEngine;
using Zenject;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameInstaller", menuName = "Installers/GameInstaller")]
public class GameInstaller : ScriptableObjectInstaller<GameInstaller>
{
    [SerializeField] private WaveConfig waveConfig;
    [SerializeField] private EnemyConfig basicEnemyConfig;
    [SerializeField] private EnemyConfig fastEnemyConfig;
    [SerializeField] private EnemyConfig fatEnemyConfig;
    [SerializeField] private TowerConfig basicTowerConfig;
    [SerializeField] private TowerConfig fastTowerConfig;
    [SerializeField] private TowerConfig cannonTowerConfig;

    public override void InstallBindings()
    {
        Container.Bind<WaveManager>().AsSingle();
        Container.Bind<EconomyManager>().AsSingle();
        Container.Bind<GameStateMachine>().AsSingle();
        Container.Bind<PlayerBase>().FromComponentInHierarchy().AsSingle();
        Container.Bind<TowerUpgradeUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<EnemyConfig>().FromInstance(basicEnemyConfig).AsSingle();
        Container.Bind<Dictionary<EnemyType, EnemyConfig>>().WithId("EnemyConfigs")
            .FromInstance(new Dictionary<EnemyType, EnemyConfig>
            {
                { EnemyType.BasicEnemy ,basicEnemyConfig},
                { EnemyType.FastEnemy ,fastEnemyConfig},
                { EnemyType.FatEnemy ,fatEnemyConfig}
            }).AsSingle();
        Container.Bind<WaveConfig>().FromInstance(waveConfig).AsSingle();

        Container.Bind<EnemyFactory>().AsSingle();
        Container.Bind<PathHolder>().FromComponentInHierarchy().AsSingle();

        Container.Bind<TowerPlacementSystem>().AsSingle();
        Container.Bind<Dictionary<TowerType, TowerConfig>>()
            .WithId("TowerConfigs")
            .FromInstance(new Dictionary<TowerType, TowerConfig>
            {
                { TowerType.Basic, basicTowerConfig },
                { TowerType.Fast, fastTowerConfig },
                { TowerType.Canonball , cannonTowerConfig}
            })
            .AsSingle();
        Container.Bind<IFactory<TowerType, Transform, ITower>>().To<TowerFactory>().AsSingle();
    }
}