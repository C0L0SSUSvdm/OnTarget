using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Base Car", menuName = "Inventory System/Car")]
[System.Serializable]
public class BaseCar : ScriptableObject
{
    //Chasis
    [Header("Car Chasis")]
    [SerializeField] public uint chasisID;
    public Chasis chasis { get; set; }

    [Header("Car Engine")]
    [SerializeField] public uint engineID;
    public Engine engine { get; set; }

    [Header("Car Front Tires")]
    public uint FrontTiresID;
    public Tires frontTires { get; set; }

    [Header("Car Rear Tires")]
    public uint RearTiresID;
    public Tires rearTires { get; set; }

    [Header("Car Power Steering")]
    public uint powerSteeringID;
    public PowerSteering powerSteering { get; set; }

    [Header("Car Shock Absorbers")]
    public uint shockAbsorbersID;
    public ShockAbsorbers shockAbsorbers { get; set; }

    [Header("Car Brakes")]
    public uint brakesID;
    public Brakes brakes { get; set; }

    //[Header("Input Scalars")]
    //[SerializeField] public float motorPower;
    //[SerializeField] public float steerPower; // 50 is a Euler angle
    //[SerializeField] public float brakePower;
    //[Header("RigidBody Fields")]
    //[SerializeField] float basemass;
    //[SerializeField] float baseDrag;
    //[SerializeField] float baseAngularDrag;

    //[Header("Wheel Collider Fields")]
    //[SerializeField] float baseMass;
    //[SerializeField] float baseWheelDampingRate;
    //[SerializeField] float baseSuspensionDistance;
    //[SerializeField] float baseForceAppPointDistance;
    //[Header("Wheel Spring Fields")]
    //[SerializeField] float baseSpringForce;
    //[SerializeField] float baseSpringDamper;
    //[SerializeField] float baseSpringTargetPosition;
    //[Header("Foward Friction Curve")]
    //[SerializeField] float baseFwdExtremumSlip;
    //[SerializeField] float baseFwdExtremumValue;
    //[SerializeField] float baseFwdAsymptoteSlip;
    //[SerializeField] float baseFwdAsymptoteValue;
    //[SerializeField] float baseFwdStiffness;
    //[Header("Side Friction Curve")]
    //[SerializeField] float baseSideExtremumSlip;
    //[SerializeField] float baseSideExtremumValue;
    //[SerializeField] float baseSideAsymptoteSlip;
    //[SerializeField] float baseSideAsymptoteValue;
    //[SerializeField] float baseSideStiffness;

}
