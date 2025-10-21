using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TowerUpgradeUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI sellText;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button upgradeButton;

    [Inject] private EconomyManager _economyManager;

    private Tower _selectedTower;

    private void Awake()
    {
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        sellButton.onClick.AddListener(OnSellClicked);
        panel.SetActive(false);
    }

    public void ShowForTower(Tower tower)
    {
        _selectedTower = tower;
        UpdateUI();
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
        _selectedTower = null;
    }

    private void UpdateUI()
    {
        if (_selectedTower == null) return;

        levelText.text = $"Level: {_selectedTower.CurrentLevel} / {_selectedTower.MaxLevel}";
        sellText.text = $"Sell for {GetSellCost()}";
        if (GetCost() > 0)
        {
            costText.text = $"Upgrade Cost: {GetCost()}";
            upgradeButton.interactable = _economyManager.Resources >= GetCost();
        }
        else
        {
            costText.text = "Max Level";
            upgradeButton.interactable = false;
        }
    }

    private int GetSellCost()
    {
        return _selectedTower._config.Cost / 2 + GetCost() / 2;
    }

    private int GetCost()
    {
        return _selectedTower.GetUpgradeCost();
    }

    private void OnUpgradeClicked()
    {
        if (_selectedTower != null)
        {
            _selectedTower.Upgrade();
            UpdateUI();
        }
    }

    private void OnSellClicked()
    {
        if (_selectedTower != null)
        {
            var spot = _selectedTower.transform.parent.GetComponent<TowerPlacementSpot>();
            if (spot != null)
            {
                _economyManager.AddResources(GetSellCost());
                Destroy(_selectedTower.gameObject);
                spot.Free();
                Hide();
            }
        }
    }
}
