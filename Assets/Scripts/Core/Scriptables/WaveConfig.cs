using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "Configs/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    [Serializable]
    public struct EnemyGroup
    {
        public EnemyType Type;
        public int Count;
        public float SpawnDelay;
    }

    [Serializable]
    public struct Wave
    {
        public List<EnemyGroup> Groups;
    }

    public List<Wave> Waves;
    public float WavePauseDuration = 10f;
}