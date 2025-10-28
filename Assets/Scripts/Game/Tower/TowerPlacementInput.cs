using UnityEngine;
using Zenject;

public class TowerPlacementInput : MonoBehaviour
{
    [Inject] private TowerPlacementSystem _placementSystem;

    private TowerPlacementSpot _lastHighlightedSpot;
    private LayerMask _placementLayer;

    private int typeInt;

    private void Awake()
    {
        _placementLayer = LayerMask.GetMask("PlacementSpots");
    }

    private void Update()
    {
        HandleHighlight();
        HandlePlacement(typeInt);
    }

    private void HandleHighlight()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _placementLayer))
        {
            var spot = hit.collider.GetComponent<TowerPlacementSpot>();
            if (spot != null && !spot.IsOccupied)
            {
                if (_lastHighlightedSpot != spot)
                {
                    if (_lastHighlightedSpot != null) _lastHighlightedSpot.Highlight(false, _placementSystem.CanPlaceTower(TowerType.Basic));
                    _lastHighlightedSpot = spot;
                    _lastHighlightedSpot.Highlight(true, _placementSystem.CanPlaceTower(TowerType.Basic));
                }
                return;
            }
        }
        if (_lastHighlightedSpot != null)
        {
            _lastHighlightedSpot.Highlight(false, _placementSystem.CanPlaceTower(TowerType.Basic));
            _lastHighlightedSpot = null;
        }
    }

    public void SetNewTowerType(int towerType)
    {
        switch (towerType)
        {
            case 0:
                typeInt = 0;
                break;
            case 1:
                typeInt = 1;
                break;
            case 2:
                typeInt = 2;
                break;
        }
    }

    private void HandlePlacement(int typeAsInt)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _placementLayer))
            {
                var spot = hit.collider.GetComponent<TowerPlacementSpot>();
                if (spot != null && !spot.IsOccupied)
                {
                    TowerType tp = TowerType.Basic;
                    switch (typeAsInt)
                    {
                        case 0:
                            tp = TowerType.Basic;
                            break;
                        case 1:
                            tp = TowerType.Fast;
                            break;
                        case 2:
                            tp = TowerType.Canonball;
                            break;
                    }
                    _placementSystem.PlaceTowerAt(spot,tp);
                }
            }
        }
    }
}