using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AirController", menuName = "Inventory System/Items/AirController")]
public class AirController : Item
{
    [Range(500, 1500)] public int IdleRPMs;
    [Range(200, 800)] public float AirFlowRate; //FLow Rate Cubic Feet per minute
}
