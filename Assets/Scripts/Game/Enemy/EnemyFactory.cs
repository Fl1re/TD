using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyFactory : IFactory<EnemyType,Vector3[],IEnemy>
{
    private readonly DiContainer _container;
    private readonly Dictionary<EnemyType,EnemyConfig> _configs;

    public EnemyFactory(DiContainer container, [Inject(Id = "EnemyConfigs")] Dictionary<EnemyType,EnemyConfig> config)
    {
        _container = container;
        _configs = config;
    }

    public IEnemy Create(EnemyType type,Vector3[] path)
    {
        if (_configs.TryGetValue(type, out var config))
        {
            var enemy = _container.InstantiatePrefabForComponent<IEnemy>(config.Prefab);
            enemy.Initialize(path,config);
            return enemy;
        }
        else
            return null;
    }
}