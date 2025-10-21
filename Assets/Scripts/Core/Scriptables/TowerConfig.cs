using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    Basic,
    Fast,
    Canonball
}

[CreateAssetMenu(fileName = "TowerConfig", menuName = "Configs/TowerConfig")]
public class TowerConfig : ScriptableObject
{
    [System.Serializable]
    public class UpgradeLevel
    {
        public float Damage = 20f;
        public float ProjectileSpeed = 60f;
        public float AttackRadius = 5f;
        public float AttackInterval = 1f;
        public float RotationSpeed = 6f;
        public int UpgradeCost = 0;
    }
    
    public TowerType Type;
    public GameObject Prefab;
    public Projectile ProjectilePrefab;
    public int Cost = 50;
    public UpgradeLevel BaseLevel;
    public List<UpgradeLevel> UpgradeLevels;
}