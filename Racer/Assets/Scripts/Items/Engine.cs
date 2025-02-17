using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Engine", menuName = "Inventory System/Items/Engine")]
public class Engine : Item
{  
    [Range(3.0f, 4.5f)] public float CylinderStrokeLength; //inches
    [Range(2.0f, 12.0f)] public int CylinderCount;
    [Range(3.0f, 4.5f)] public float CylinderDiameter; //inches
    [Tooltip("2 or 4 stroke Engine")]
    [Range(2, 4)] public int NumberOfStrokes;
}
