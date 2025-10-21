using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TowerPlacementSystem
{
    [Inject] private IFactory<TowerType, Transform, ITower> _towerFactory;
    [Inject] private EconomyManager _economyManager;
    [Inject(Id = "TowerConfigs")] private Dictionary<TowerType, TowerConfig> _configs;

    public bool CanPlaceTower(TowerType type)
    {
        if (!_configs.TryGetValue(type, out var config)) return false;
        return _economyManager.Resources >= config.Cost;
    }

    public bool PlaceTowerAt(TowerPlacementSpot spot, TowerType type = TowerType.Basic)
    {
        if (!CanPlaceTower(type) || spot.IsOccupied) return false;

        if (!_configs.TryGetValue(type, out var config)) return false;

        if (!_economyManager.SpendResources(config.Cost)) return false;

        ITower tower = _towerFactory.Create(type, spot.transform);
        spot.Occupy(tower);
        return true;
    }
}