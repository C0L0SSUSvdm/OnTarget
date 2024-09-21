using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class baseVehicle : GravityBody
{
    [Header("----- Vehicle Fields -----")]
    [SerializeField] float maximumSteerAngle;
    [SerializeField] float currentSteerAngle;
    [SerializeField] protected float MotorForce = 10.0f;
    //https://www.hagerty.com/media/maintenance-and-tech/10-factors-that-influence-an-engines-character/#:~:text=How%20an%20engine%20breathes%20is,and%20fuel%20into%20the%20cylinders.
    [Header("----- Engine Item Fields -----")]
    [Range(0.05f, 1.0f), SerializeField] float CamShaftRadius = 0.1f;// length = pi r
    [SerializeField] float CylinderCount = 8;
    [SerializeField] float CrankShaftAngle = 45; // 1 / (Number of Cylinders), can be other angles
    [SerializeField] float MaxRPMs = 8000;
    [SerializeField] float CurrentRPMs = 0;
    [SerializeField] float PistonDiameter = 4.5f;
    [SerializeField] float RunTimeCompressionRatio;
    [SerializeField] float RunTimeMeanRPM;
    [SerializeField] float RunTimeCurveDeviation_Inverse;
    [Header ("---- Air Control Valve -----")]
    [Range(500, 1500), SerializeField] float IdleRPMs = 1000;
    [Header("----- Piston Fields -----")]
    [Range(-20, 20), SerializeField] float PistonCC = 0;
    [Header("----- Engine Header -----")]
    [Range(10, 200), SerializeField] float ChamberVolume = 46.0f;
    [Header("----- Transmission Fields -----")]
    [SerializeField] int GearIndex = 0;
    [SerializeField] int ShiftUpRPM = 6000;
    [SerializeField] int ShiftDownRPM = 2000;
    [SerializeField] float ShiftTime = 0.1f;
    [SerializeField] float[] GearRatio = new float[6] { 0.50f, 0.72f, 1.0f, 1.4f, 2.2f, 3.82f };
    [Header("----- Differential Fields -----")]
    [SerializeField] float DifferentialRatio = 3.42f;
    [Header("----- Wheel Fields -----")]
    [SerializeField] float DriveWheelRadius = 1.0f;

    [Header("----- Collider Fields -----")]
    [SerializeField] Transform COM; //Center of Mass
    [SerializeField] Transform SteerPoint;
    GameObject WheelOBJ_FL;
    GameObject WheelOBJ_FR;
    GameObject WheelOBJ_BL;
    GameObject WheelOBJ_BR;
    Suspension wheel_FL;
    Suspension wheel_FR;
    Suspension wheel_BL;
    Suspension wheel_BR;

    protected void Start()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 1000;
        rb.drag = 0;// 0.5f;
        rb.angularDrag = 0;// 0.5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        WheelOBJ_FL = transform.Find("Wheel_Left Front").gameObject;
        WheelOBJ_FR = transform.Find("Wheel_Right Front").gameObject;
        WheelOBJ_BL = transform.Find("Wheel_Left Back").gameObject;
        WheelOBJ_BR = transform.Find("Wheel_Right Back").gameObject;

        wheel_FL = WheelOBJ_FL.GetComponent<Suspension>();
        wheel_FR = WheelOBJ_FR.GetComponent<Suspension>();
        wheel_BL = WheelOBJ_BL.GetComponent<Suspension>();
        wheel_BR = WheelOBJ_BR.GetComponent<Suspension>();

        wheel_FL.InitializeSuspension(rb);
        wheel_FR.InitializeSuspension(rb);
        wheel_BL.InitializeSuspension(rb);
        wheel_BR.InitializeSuspension(rb);

    }

    protected new void Update()
    {
        CalculateGravity();

        float sumofDistance = wheel_FL.COMDistance(COM) + wheel_FR.COMDistance(COM) + wheel_BL.COMDistance(COM) + wheel_BR.COMDistance(COM);
        float sumOfDistances_Inverse = sumofDistance != 0 ? 1 / sumofDistance : 0;
        wheel_FL.SetWeightOnWheel(sumOfDistances_Inverse, EnvironmentForces.y);
        wheel_FR.SetWeightOnWheel(sumOfDistances_Inverse, EnvironmentForces.y);
        wheel_BL.SetWeightOnWheel(sumOfDistances_Inverse, EnvironmentForces.y);
        wheel_BR.SetWeightOnWheel(sumOfDistances_Inverse, EnvironmentForces.y);

        float sumOfCompression = wheel_FL.SuspensionDistance() + wheel_FR.SuspensionDistance() + wheel_BL.SuspensionDistance() + wheel_BR.SuspensionDistance();
        float sumOfCompression_Inverse = sumOfCompression != 0 ? 1 / sumOfCompression: 0;
        wheel_FL.SetMassOnWheel(sumOfCompression_Inverse, rb.mass);
        wheel_FR.SetMassOnWheel(sumOfCompression_Inverse, rb.mass);
        wheel_BL.SetMassOnWheel(sumOfCompression_Inverse, rb.mass);
        wheel_BR.SetMassOnWheel(sumOfCompression_Inverse, rb.mass);

        wheel_FL.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_FL.transform.position));
        wheel_FR.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_FR.transform.position));
        wheel_BL.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_BL.transform.position));
        wheel_BR.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_BR.transform.position));



        ApplyGravity();
    }

    protected void ApplyGasPedal(float input)
    {
        if(CurrentRPMs < ShiftDownRPM && GearIndex > 0)
        {
            GearIndex--;
        }
        else if(CurrentRPMs > ShiftUpRPM && GearIndex < GearRatio.Length - 1)
        {
            GearIndex++;
        }

        float wheelTrainRatio = DifferentialRatio * GearRatio[GearIndex] * DriveWheelRadius * CrankShaftAngle;
        float RPM_fromKineticEnergy = wheel_BL.AngularVelocity / wheelTrainRatio;
        float deltaRPM = RPM_fromKineticEnergy - CurrentRPMs; //Convert deltaRPM to Force
        if (input != 0)
        {
            CurrentRPMs = Mathf.Lerp(CurrentRPMs, MaxRPMs, Time.deltaTime * (RPM_fromKineticEnergy - CurrentRPMs + 1));
        }
        else
        {
            CurrentRPMs = Mathf.Lerp(CurrentRPMs, IdleRPMs, Time.deltaTime * (RPM_fromKineticEnergy - CurrentRPMs + 1));
        }
        wheel_BL.DriveWheel(CurrentRPMs, wheelTrainRatio);
        wheel_BR.DriveWheel(CurrentRPMs, wheelTrainRatio);

    }

    protected void UpdateSteeringAngle(float input)
    {
        currentSteerAngle = maximumSteerAngle * input;
        WheelOBJ_FL.transform.localRotation = Quaternion.Euler(WheelOBJ_FL.transform.localRotation.x + rb.angularVelocity.magnitude, currentSteerAngle, 0);
        WheelOBJ_FR.transform.localRotation = Quaternion.Euler(WheelOBJ_FR.transform.localRotation.x + rb.angularVelocity.magnitude, currentSteerAngle, 0);
    }

    protected void ApplySteerForce()
    {
        
        //transform.RotateAround(SteerPoint.position, Vector3.up, currentSteerAngle * Time.deltaTime * rb.velocity.magnitude);
        //wheel_FL.SteerVehicle();
        //wheel_FR.SteerVehicle();
    }

    public float GetSteeringAngle()
    {
        return currentSteerAngle;
    }

    //https://www.symbolab.com/graphing-calculator/bell-curve-graph
    private void InitializeNormalDistibutionCurve()
    {
        float minimumCompression = ChamberVolume + PistonCC;
        // Total Volume when piston at lowest Point / Volume when Piston at highest Point
        float ConvertToCC = 16.387f;
        RunTimeCompressionRatio = (((Mathf.PI / PistonDiameter) * PistonDiameter * PistonDiameter * ConvertToCC) + minimumCompression) / minimumCompression;
        
        RunTimeMeanRPM = (MaxRPMs - IdleRPMs) * 0.5f;

        float deviation = MaxRPMs / (CylinderCount * CamShaftRadius);
        RunTimeCurveDeviation_Inverse = (1 / 2 * deviation * deviation);


    }

    private float CalculateCurveExponent()
    {
        float test = (CurrentRPMs - RunTimeMeanRPM);
        float test2 = test * test;
        float test3 = (test2 * RunTimeCurveDeviation_Inverse);
        //Make Sure to return a negative value to make the bell curve
        return -test3;
    }
}
