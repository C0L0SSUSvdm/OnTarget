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
    public uint chasisID;
    public Chasis chasis { get; set; }

    [Header("Car Engine")]
    public uint engineID;
    public Engine engine { get; set; }

    [Header("Car Cam Shaft")]
    public uint camshaftID;
    public CamShaft camShaft { get; set; }

    [Header("Car Crank Shaft")]
    public uint crankShaftID;
    public CrankShaft crankShaft { get; set; }

    [Header("Car FlyWheel")]
    public uint flyWheelID;
    public FlyWheel flyWheel { get; set; }

    [Header("Car Air Controller")]
    public uint airControlerID;
    public AirController airController { get; set; }

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


}
