using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PowerSteering", menuName = "Inventory System/Items/PowerSteering")]
public class PowerSteering : Item
{
    [SerializeField] public float SteerPower = 35;
    [Tooltip("The speed at which the max power steering is reduced by the SteeringReduction")]
    [SerializeField] public int MaxSpeed;
    [Range(0, 1), SerializeField] public float SteeringReduction;
}
