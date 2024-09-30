using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

public class baseVehicle : GravityBody
{
    //[SerializeField] public float TestTurnStrength;
    [Header("----- Vehicle Fields -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] float maximumSteerAngle;
    [SerializeField] protected float currentSteerAngle;
    //[SerializeField] protected float MotorForce = 10.0f;
    [SerializeField] bool isClutchEngaged = true;
    [SerializeField] float AeroDynamicDrag = 0.5f;
    //https://www.hagerty.com/media/maintenance-and-tech/10-factors-that-influence-an-engines-character/#:~:text=How%20an%20engine%20breathes%20is,and%20fuel%20into%20the%20cylinders.
    [Header("----- Engine Item Fields -----")]
    [SerializeField] Engine Engine;
    [Range(0.01f, 1.0f), SerializeField] float EngineEfficiency = 0.3f;
    [Range(0.1f, 1.0f), SerializeField] float CamShaftRadius = 0.75f;// length = pi r
    [Tooltip("Value assumed to be in inches")]
    [Range(1.00f, 3.0f), SerializeField] float CrankShaftRadius = 2.2f; //CrankThrow Radius is different from CrankShaft Radius
    [SerializeField] float CylinderCount = 8;
    [SerializeField] float CrankShaftAngle = 45; // 1 / (Number of Cylinders), can be other angles
    [SerializeField] float RedLineRPM = 8000;
    [SerializeField] float EngineCompressionRatio = 10.0f;
    [SerializeField] float PistonDiameter = 4.0f;
    [SerializeField] float CurrentRPM = 0;   
    [SerializeField] float RunTimeCompressionRatio;
    [SerializeField] float RunTimeMeanRPM;
    [SerializeField] float RunTimeCurveDeviation_Inverse;
    [SerializeField] float RunTimeBackPressure;
    [SerializeField] float RunTimeCombustionForce;
    [Header ("---- Air Control Valve -----")]
    [Range(500, 1500), SerializeField] float IdleRPMs = 1500;
    [Header("----- Piston Fields -----")]
    [Range(-20, 20), SerializeField] float PistonCC = 0;
    [Header("----- Engine Header -----")] //Phase Out
    [Range(50, 200), SerializeField] float ChamberVolume = 59.0f;
    [Range(0.01f, 0.2f), SerializeField] float HeaderBackPressure = 0.1f;
    [Header("----- Transmission Fields -----")]
    [SerializeField] int GearIndex = 0;
    [SerializeField] int ShiftUpRPM = 6000;
    [SerializeField] int ShiftDownRPM = 2000;
    [SerializeField] float ShiftTime = 0.1f;
    [SerializeField] float[] GearRatio = new float[6] { 3.82f, 2.2f, 1.4f, 1.0f, 0.8f, 0.6f };
    [Header("----- Differential Fields -----")]
    [SerializeField] float DifferentialRatio = 3.42f;
    [Header("----- Wheel Fields -----")]
    [SerializeField] float RearTireRadius = 1.0f;
    [Header("----- Turbo Fields -----")]
    [Range(1.0f, 5.0f), SerializeField] float TurboBoost = 2.3f;
    [Header("----- Exhaust Header -----")]
    [Range(0.1f, 1.0f), SerializeField] float ExhaustHeaderBackPressure = 0.5f;
    [Header("----- Exhaust Pipes -----")] 
    [SerializeField] float ExhaustPipeBackPressure = 1.3f;
    [Header("----- Carborator-----")]
    [Range(14.7f, 25.0f), SerializeField] float CompressedAirPressure = 14.7f;

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
        rb.angularDrag = 0.2f;// 0.5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.maxAngularVelocity = 7;

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

        CurrentRPM = IdleRPMs;
        //TODO: Current BackPressure is linear, Calculate BackPressure on some kind of bell curve to find optimal value
        RunTimeBackPressure = HeaderBackPressure + ExhaustHeaderBackPressure + ExhaustPipeBackPressure;
        InitializeNormalDistibutionCurve();
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

        //rb.AddForceAtPosition(wheel_FL.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_FL.transform.position)), WheelOBJ_FL.transform.position, ForceMode.Force);
        //rb.AddForceAtPosition(wheel_FR.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_FR.transform.position)), WheelOBJ_FR.transform.position, ForceMode.Force);
        //rb.AddForceAtPosition(wheel_BL.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_BL.transform.position)), WheelOBJ_BL.transform.position, ForceMode.Force);
        //rb.AddForceAtPosition(wheel_BR.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_BR.transform.position)), WheelOBJ_BR.transform.position, ForceMode.Force);
        float groundedWeight = wheel_FL.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_FL.transform.position)) +
        wheel_FR.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_FR.transform.position)) +
        wheel_BL.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_BL.transform.position)) +
        wheel_BR.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_BR.transform.position));

        //rb.AddForce(Vector3.up * groundedWeight, ForceMode.Force);

        ApplyGravity();
    }
    
    private float ShiftTranmission()
    {
        float WheelTrainRatio = DifferentialRatio * GearRatio[GearIndex] * RearTireRadius;
        if (CurrentRPM < ShiftDownRPM && GearIndex > 0)
        {
            float wheelAV = CurrentRPM * 2 * Mathf.PI * RearTireRadius / WheelTrainRatio;
            GearIndex--;
            WheelTrainRatio = DifferentialRatio * GearRatio[GearIndex] * RearTireRadius;
            CurrentRPM = (wheelAV * WheelTrainRatio) / (2 * Mathf.PI * RearTireRadius);
        }
        else if (CurrentRPM > ShiftUpRPM && GearIndex < GearRatio.Length - 1)
        {
            float wheelAV = CurrentRPM * 2 * Mathf.PI * RearTireRadius / WheelTrainRatio;
            GearIndex++;
            WheelTrainRatio = DifferentialRatio * GearRatio[GearIndex] * RearTireRadius;
            CurrentRPM = (wheelAV * WheelTrainRatio) / (2 * Mathf.PI * RearTireRadius);

        }
        else
        {
            WheelTrainRatio = DifferentialRatio * GearRatio[GearIndex] * RearTireRadius;
        }

        return WheelTrainRatio;
    }

    protected void ApplyGasPedal(float input)
    {
        float WheelTrainRatio = ShiftTranmission();
        float Torque = CrankShaftRadius * Mathf.Sin(CrankShaftAngle) * (RunTimeCombustionForce * EngineEfficiency) * WheelTrainRatio;

        if (input != 0) //Exponential Increase of RPM
        {
            float exponent = CalculateCurveExponent();
            float CurveRatio = Mathf.Pow(PistonDiameter, exponent);

            //TODO: Add TurboBoose, BackPressure, 
            float RateOfChange = CurveRatio * RunTimeBackPressure * Torque * CylinderCount;

            ///Debug.Log($"Piston: {PistonDiameter}^{CalculateCurveExponent()} = {CurveRatio} : RATE OF CHANGE = {RateOfChange}: TORQUE Multiplier: {TorqueScalar}");
            CurrentRPM += RateOfChange * Time.deltaTime;

        }
        else //Linear Reduction of RPM
        {
            //TODO:: Calculate Better RPM Reduction Method
            CurrentRPM = Mathf.Lerp(CurrentRPM, ShiftDownRPM * 0.5f, 200 * Time.deltaTime * Time.deltaTime * 0.5f);
        }

        //CurrentHorsePower = (CurrentRPMs * Torque) / 5252;

        float WheelAngularVelocity = CurrentRPM * 2 * Mathf.PI * RearTireRadius / (60 * WheelTrainRatio);
        float acceleration = (WheelAngularVelocity) / Time.fixedDeltaTime + (Torque * input);

        wheel_FL.DriveWheel(0, WheelAngularVelocity);
        wheel_FR.DriveWheel(0, WheelAngularVelocity);
        wheel_BL.DriveWheel(acceleration, WheelAngularVelocity);
        wheel_BR.DriveWheel(acceleration, WheelAngularVelocity);


    }

    protected void UpdateSteeringAngle(float input)
    {
        currentSteerAngle = maximumSteerAngle * input;
        WheelOBJ_FL.transform.localRotation = Quaternion.Euler(WheelOBJ_FL.transform.localRotation.x + rb.angularVelocity.magnitude, currentSteerAngle, 0);
        WheelOBJ_FR.transform.localRotation = Quaternion.Euler(WheelOBJ_FR.transform.localRotation.x + rb.angularVelocity.magnitude, currentSteerAngle, 0);
    }

    protected void ApplySteerForce(float input)
    {

        //transform.RotateAround(SteerPoint.position, Vector3.up, currentSteerAngle * Time.deltaTime * rb.velocity.magnitude);
        float leftTorque = wheel_FL.SteerVehicle(currentSteerAngle, transform.right);
        float rightTorque = wheel_FR.SteerVehicle(currentSteerAngle, transform.right);
        
        //rb.AddTorque(transform.up * (leftTorque + rightTorque));
        //rb.AddForceAtPosition(transform.right * leftTorque * input, WheelOBJ_FL.transform.position, ForceMode.Acceleration);
        //rb.AddForceAtPosition(transform.right * rightTorque * input, WheelOBJ_FR.transform.position, ForceMode.Acceleration);
        //rb.AddForceAtPosition(transform.right * GetSteeringAngle() * rb.velocity.magnitude * Time.deltaTime, SteerPoint.position, ForceMode.Acceleration);
        //transform.RotateAround(SteerPoint.position, Vector3.up, currentSteerAngle * Time.deltaTime * rb.velocity.magnitude);
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
        float ConvertToCC = 16.387f;//cubic inches to Cubic Centimeter ratio
        float PistonArea = (PistonDiameter * PistonDiameter * 0.25f * Mathf.PI) * (12 * CrankShaftRadius * Mathf.PI) * 0.6f; //Area * Height * Material Coefficient
        RunTimeCompressionRatio = ((PistonArea * ConvertToCC) + minimumCompression) / minimumCompression; //Switched to a static value for Compression Ratio
        RunTimeCombustionForce = CompressedAirPressure * EngineCompressionRatio;

        RunTimeMeanRPM = (RedLineRPM - IdleRPMs) * 0.5f;

        float deviation = RedLineRPM / (CylinderCount * CamShaftRadius);

       RunTimeCurveDeviation_Inverse = 1 / (2 * deviation * deviation);


    }

    private float CalculateCurveExponent()
    {
        float xminusMean = (CurrentRPM - RunTimeMeanRPM);
        float Squared = xminusMean * xminusMean;
        float exponent = (Squared * RunTimeCurveDeviation_Inverse);
        //Make Sure to return a negative value to make the bell curve
        
        return -exponent;
    }

}
