using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chasis", menuName = "Inventory System/Items/Chasis")]
public class Chasis : Item
{
    [Range(0, 1)] public float RepairEfficiency;
    [Range(0, 1)] public float DragCoefficient;
    public int Durability;
    public int Mass;
    public float Drag;
    public float AngularDrag;
}
