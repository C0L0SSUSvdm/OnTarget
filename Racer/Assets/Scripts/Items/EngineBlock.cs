using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EngineBlock", menuName = "Inventory System/Items/EngineBlock")]
public class EngineBlock : Item
{
    [Range(1.1f, 4.0f)] public float CylinderStrokeLength; //2
    [Range(1, 12)] public int CylinderCount; //8
    [Range(1, 10)] public float CylinderDiameter; //4
    [Range(2, 4)] public int NumberOfStrokes;
}