using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Transform levelsContainer;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private Toggle levelButtonPrefab;
    [SerializeField] private Button startLevelButton;

    [Inject] private ILevelManager _levelManager;

    private void Start()
    {
        GenerateLevelButtons();
    }

    private void GenerateLevelButtons()
    {
        foreach (var level in _levelManager.GetLevels())
        {
            Toggle toggle = Instantiate(levelButtonPrefab, levelsContainer);
            toggle.GetComponentInChildren<TextMeshProUGUI>().text = level.LevelName;
            toggle.interactable = _levelManager.IsLevelUnlocked(level);
            toggle.group = toggleGroup;
            toggle.onValueChanged.AddListener((selected =>
            {
                if (selected)
                {
                    OnLevelSelected(level);
                }
            }));
        }
    }

    private void OnLevelSelected(LevelConfig level)
    {
        startLevelButton.onClick.RemoveAllListeners();
        
        startLevelButton.onClick.AddListener(() => _levelManager.LoadLevel(level));
    }
}
