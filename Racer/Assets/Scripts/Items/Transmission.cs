using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Transmission", menuName = "Inventory System/Items/Transmission")]
public class Transmission : Item
{
    public float ShiftTime = 0.1f;
    public float[] GearRatio;// = new float[6] { 3.82f, 2.2f, 1.4f, 1.0f, 0.8f, 0.6f };
}
