using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITooltip
{
    string GetTooltipText();
    float GetAttackRadius();
    Vector3 GetPosition();
}
