using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Engine", menuName = "Inventory System/Items/Engine")]
public class Engine : Item
{  
    [Range(7.62f, 15.24f)] public float CylinderMaxLength_CM; //inches
    [Range(7.62f, 15.24f)] public float CylinderDiameter_CM; //inches
    [Range(2.0f, 12.0f)] public int CylinderCount;

    [Tooltip("2 or 4 stroke Engine")]
    [Range(2, 4)] public int NumberOfStrokes;
}
