using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TowerTypeUI : MonoBehaviour
{
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private List<Toggle> towerToggles;

    [Inject] private TowerPlacementInput _towerPlacementInput;

    private void Awake()
    {
        for (int i = 0; i < towerToggles.Count; i++)
        {
            towerToggles[i].group = toggleGroup;
            var type = i;
            towerToggles[i].onValueChanged.AddListener(isOn => {if(isOn){_towerPlacementInput.SetNewTowerType(type);}});
        }
    }
}
