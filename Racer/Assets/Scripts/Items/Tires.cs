using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tires", menuName = "Inventory System/Items/Tires")]
public class Tires : Item
{
    [Header("Wheel Collider Fields")]
    [Range(1, 100), SerializeField] float Mass;
    [Range(1, 100), SerializeField] float WheelDampingRate;
    [Range(1, 100), SerializeField] float SuspensionDistance;
    [Range(1, 100), SerializeField] float ForceAppPointDistance;

    [Header("Foward Friction Curve")]
    [Range(1, 100), SerializeField] float ForwardExtremumSlip;
    [Range(1, 100), SerializeField] float ForwardExtremumValue;
    [Range(1, 100), SerializeField] float ForwardAsymptoteSlip;
    [Range(1, 100), SerializeField] float ForwardAsymptoteValue;
    [Range(1, 100), SerializeField] float ForwardStiffness;
    [Header("Side Friction Curve")]
    [Range(1, 100), SerializeField] float SideExtremumSlip;
    [Range(1, 100), SerializeField] float SideExtremumValue;
    [Range(1, 100), SerializeField] float SideAsymptoteSlip;
    [Range(1, 100), SerializeField] float SideAsymptoteValue;
    [Range(1, 100), SerializeField] float SideStiffness;
}
