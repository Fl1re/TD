using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTooltip : MonoBehaviour,ITooltip
{
    private PlayerBase _playerBase;

    private void Awake()
    {
        _playerBase = GetComponent<PlayerBase>();
    }

    public string GetTooltipText()
    {
        return $"Здоровье башни: {_playerBase.Health}/{_playerBase.MaxHealth}";
    }

    public float GetAttackRadius()
    {
        return 0;
    }

    public Vector3 GetPosition()
    {
        return new Vector3();
    }
}
