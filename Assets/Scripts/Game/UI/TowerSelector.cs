using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class TowerSelector : MonoBehaviour
{
    [Inject] private TowerUpgradeUI _towerUpgradeUI;

    [SerializeField]private LayerMask _towerLayer;

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _towerLayer))
            {
                var tower = hit.collider.GetComponent<Tower>();
                if (tower != null)
                {
                    _towerUpgradeUI.ShowForTower(tower);
                    return;
                }
            }
            _towerUpgradeUI.Hide();
        }
    }
}
