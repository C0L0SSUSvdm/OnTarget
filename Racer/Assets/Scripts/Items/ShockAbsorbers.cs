using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shocks", menuName = "Inventory System/Items/ShockAbsorbers")]
public class ShockAbsorbers : Item
{
    [Header("Wheel Spring Fields")]
    public float SpringForce;
    public float SpringDamper;
    public float SpringTargetPosition;
}
