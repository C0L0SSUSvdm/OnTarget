using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Engine", menuName = "Inventory System/Items/Engine")]
public class Engine : Item
{
    public float motorMass;
    public float motorPower;
    //[Range(0.01f, 1.0f), SerializeField] float EngineEfficiency = 0.3f;
    //[Range(0.1f, 1.0f), SerializeField] float CamShaftRadius = 0.75f;// length = pi r
    //[Tooltip("Value assumed to be in inches")]
    //[Range(1.00f, 3.0f), SerializeField] float CrankShaftRadius = 2.2f; //CrankThrow distance is different from CrankShaft Radius
    //[SerializeField] float CylinderCount = 8;
    //[SerializeField] float CrankShaftAngle = 45; // 1 / (Number of Cylinders), can be other angles
    //[SerializeField] float RedLineRPM = 8000;
    //[SerializeField] float EngineCompressionRatio = 10.0f;
    //[SerializeField] float PistonDiameter = 4.0f;
}
