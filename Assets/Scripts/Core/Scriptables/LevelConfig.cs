using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public string LevelName;
    public string SceneName;
    public int RequiredStarsToUnlock;
    public WaveConfig WaveConfig;
}
