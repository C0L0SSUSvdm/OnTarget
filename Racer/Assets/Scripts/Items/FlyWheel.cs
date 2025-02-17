using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FlyWheel", menuName = "Inventory System/Items/FlyWheel")]
public class FlyWheel : Item
{
    [Range(20.0f, 50.0f)] public float FlyWheelMass;
    [Range(2.0f, 10.0f)] public float FlyWheelRadius;

}
