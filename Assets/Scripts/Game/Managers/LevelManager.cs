using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class LevelManager : ILevelManager
{
    [Inject] private List<LevelConfig> _levels;

    public List<LevelConfig> GetLevels() => _levels;

    public bool IsLevelUnlocked(LevelConfig level)
    {
        int unlockedUpTo = PlayerPrefs.GetInt("UnlockedLevel", 1);
        return _levels.IndexOf(level) + 1 <= unlockedUpTo;
    }

    public void LoadLevel(LevelConfig level)
    {
        if (IsLevelUnlocked(level))
        {
            SceneManager.LoadScene(level.SceneName);
        }
    }

    public void UnlockNextLevel(LevelConfig current)
    {
        int nextIndex = _levels.IndexOf(current) + 2;
        PlayerPrefs.SetInt("UnlockedLevel", nextIndex);
    }
}
