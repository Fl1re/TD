using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelManager
{
    List<LevelConfig> GetLevels();
    bool IsLevelUnlocked(LevelConfig level);
    void LoadLevel(LevelConfig level);
}
