using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class TowerFactory : IFactory<TowerType, Transform, ITower>
{
    private readonly DiContainer _container;
    private readonly Dictionary<TowerType, TowerConfig> _configs;

    [Inject]
    public TowerFactory(DiContainer container, [Inject(Id = "TowerConfigs")] Dictionary<TowerType, TowerConfig> configs)
    {
        _container = container;
        _configs = configs;
    }

    public ITower Create(TowerType type, Transform transform)
    {
        if (!_configs.TryGetValue(type, out var config))
            throw new System.Exception($"No config found for TowerType {type}");

        var go = _container.InstantiatePrefab(config.Prefab, transform.position, Quaternion.identity, transform);
        var tower = go.GetComponent<ITower>();
        _container.Inject(tower);
        tower.Initialize(config);
        return tower;
    }
}