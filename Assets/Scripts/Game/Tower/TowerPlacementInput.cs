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
        if (Input.GetKey(KeyCode.F))
        {
            typeInt = 0;
            Debug.Log("Set As Basic");
        }
        if (Input.GetKey(KeyCode.G))
        {
            typeInt = 1;
            Debug.Log("Set As Fast");
        }
        if (Input.GetKey(KeyCode.H))
        {
            typeInt = 2;
            Debug.Log("Set As Cannon");
        }
        
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