using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTooltipProvider : MonoBehaviour,ITooltip
{
    private Tower _tower;

    private void Awake()
    {
        _tower = GetComponent<Tower>();
    }

    public string GetTooltipText()
    {
        float attackSpeed = 1f / _tower.AttackInterval;
        return $"Радиус атаки: {_tower.AttackRadius}\nУрон: {_tower.Damage}\nСкорость атаки: {attackSpeed:F2} выстр/сек";
    }

    public float GetAttackRadius() => _tower.AttackRadius;

    public Vector3 GetPosition() => transform.position;
}
