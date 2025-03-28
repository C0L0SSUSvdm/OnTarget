using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EngineBlock", menuName = "Inventory System/Items/CrankShaft")]
public class CrankShaft : Item
{
    [Tooltip("Usually 60 or 90")]
    [Range(30, 120)] public float CrankShaftAngle;
    [Range(0.5f, 1.0f)] public float CrankShaftProfileEfficiency;
    [Range(0.5f, 0.9f)] public float ThrowLengthCoefficient;
}
