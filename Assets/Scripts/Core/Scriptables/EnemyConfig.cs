using UnityEngine;

public enum EnemyType
{
    BasicEnemy,
    FastEnemy,
    FatEnemy
}

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public EnemyType Type;
    public float Health = 100f;
    public float Speed = 2f;
    public int Reward = 10;
    public int Damage = 5;
    public GameObject Prefab;
}
