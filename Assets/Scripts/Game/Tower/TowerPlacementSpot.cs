using UnityEngine;

public class TowerPlacementSpot : MonoBehaviour
{
    private Renderer _renderer;
    private Color _defaultColor = Color.green;
    private Color _hoverColor = Color.yellow;
    private Color _disabledColor = Color.red;
    private ITower _placedTower;

    public bool IsOccupied { get; private set; }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null) _renderer.material.color = _defaultColor;
    }

    public void Highlight(bool isHighlighted, bool canPlace)
    {
        if (_renderer != null && !IsOccupied)
        {
            _renderer.material.color = isHighlighted ? (canPlace ? _hoverColor : _disabledColor) : _defaultColor;
        }
    }

    public void Occupy(ITower tower)
    {
        IsOccupied = true;
        _placedTower = tower;
        if (_renderer != null) _renderer.enabled = false;
    }

    public ITower GetTower()
    {
        return _placedTower;
    }
    
    public void Free()
    {
        IsOccupied = false;
        _placedTower = null;
        if (_renderer != null)
        {
            _renderer.enabled = true;
            _renderer.material.color = _defaultColor;
        }
    }
}