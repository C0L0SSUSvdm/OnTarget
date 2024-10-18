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
    [SerializeField] Vector3 ReadAxel_forcePoint;
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
    [SerializeField] float RunTimeMotorPower;
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
    Transform FrontAxelCenterPoint;
    Transform AckermanCenterPoint;
    float RearWheelOffset;
    public float AckermanOppositeDistance;
    public float AckermanAdjacentDistance;
    //[SerializeField] GameObject Chasis;
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


        rb = gameObject.GetComponent<Rigidbody>();
        //rb.useGravity = false;
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
        ReadAxel_forcePoint = new Vector3(0, -0.5f, (WheelOBJ_BL.transform.localPosition.z * 0.5f));

        wheel_FL.InitializeSuspension(rb);
        wheel_FR.InitializeSuspension(rb);
        wheel_BL.InitializeSuspension(rb);
        wheel_BR.InitializeSuspension(rb);

        FrontAxelCenterPoint = transform.Find("FrontAxelCenterPoint");
        AckermanCenterPoint = transform.Find("AckermanCenterPoint");
        RearWheelOffset = (WheelOBJ_BL.transform.localPosition.x - WheelOBJ_BR.transform.localPosition.x) * 0.5f;
        AckermanOppositeDistance = FrontAxelCenterPoint.localPosition.z - AckermanCenterPoint.localPosition.z;
        AckermanAdjacentDistance = AckermanOppositeDistance / Mathf.Sin(Mathf.Deg2Rad * maximumSteerAngle) * Mathf.Cos(Mathf.Deg2Rad * maximumSteerAngle);

        CurrentRPM = IdleRPMs;
        //TODO: Current BackPressure is linear, Calculate BackPressure on some kind of bell curve to find optimal value
        RunTimeBackPressure = HeaderBackPressure + ExhaustHeaderBackPressure + ExhaustPipeBackPressure;
        InitializeNormalDistibutionCurve();
        
    }

    protected new void FixedUpdate()
    {
        CalculateGravity();

        float sumofDistance = wheel_FL.COMDistance(COM) + wheel_FR.COMDistance(COM) + wheel_BL.COMDistance(COM) + wheel_BR.COMDistance(COM);
        float sumOfDistances_Inverse = sumofDistance != 0 ? 1 / sumofDistance : 0;
        wheel_FL.SetWeightOnWheel(sumOfDistances_Inverse, rb.mass * Physics.gravity.y);
        wheel_FR.SetWeightOnWheel(sumOfDistances_Inverse, rb.mass * Physics.gravity.y);
        wheel_BL.SetWeightOnWheel(sumOfDistances_Inverse, rb.mass * Physics.gravity.y);
        wheel_BR.SetWeightOnWheel(sumOfDistances_Inverse, rb.mass * Physics.gravity.y);

        float sumOfCompression = wheel_FL.RayCastWheelDistance() + wheel_FR.RayCastWheelDistance() + wheel_BL.RayCastWheelDistance() + wheel_BR.RayCastWheelDistance();
        float sumOfCompression_Inverse = sumOfCompression != 0 ? 1 / sumOfCompression: 0;
        wheel_FL.SetMassOnWheel(sumOfCompression_Inverse, rb.mass);
        wheel_FR.SetMassOnWheel(sumOfCompression_Inverse, rb.mass);
        wheel_BL.SetMassOnWheel(sumOfCompression_Inverse, rb.mass);
        wheel_BR.SetMassOnWheel(sumOfCompression_Inverse, rb.mass);

        wheel_FL.UpdateSpringPhysics();
        wheel_FR.UpdateSpringPhysics();
        wheel_BL.UpdateSpringPhysics();
        wheel_BR.UpdateSpringPhysics();

        //rb.AddForceAtPosition(wheel_FL.UpdateSpringPhysics(), WheelOBJ_FL.transform.position);
        //rb.AddForceAtPosition(wheel_FR.UpdateSpringPhysics(), WheelOBJ_FR.transform.position);
        //rb.AddForceAtPosition(wheel_BL.UpdateSpringPhysics(), WheelOBJ_BL.transform.position);
        //rb.AddForceAtPosition(wheel_BR.UpdateSpringPhysics(), WheelOBJ_BR.transform.position);

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
            float RateOfChange = CurveRatio * Torque * RunTimeBackPressure;

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
        RunTimeMotorPower = (WheelAngularVelocity) / Time.fixedDeltaTime + (Torque * CylinderCount * input);

        float power = RunTimeMotorPower * 0.25f;
        wheel_FL.DriveWheel(power, rb.GetPointVelocity(WheelOBJ_FL.transform.position), WheelAngularVelocity);
        wheel_FR.DriveWheel(power, rb.GetPointVelocity(WheelOBJ_FR.transform.position), WheelAngularVelocity);
        wheel_BL.DriveWheel(power, rb.GetPointVelocity(WheelOBJ_BL.transform.position), WheelAngularVelocity);
        wheel_BR.DriveWheel(power, rb.GetPointVelocity(WheelOBJ_BR.transform.position), WheelAngularVelocity);

        //ReadAxel_forcePoint.x = -(currentSteerAngle * WheelOBJ_BR.transform.localPosition.x / maximumSteerAngle);
        //Debug.Log(calculatedForcePosition);
        //rb.AddForceAtPosition(transform.forward * 20000 * input, transform.TransformPoint(ReadAxel_forcePoint), ForceMode.Force);
        //rb.AddForce(transform.forward * 20000 * input, ForceMode.Force);
    }

    protected void UpdateSteeringAngle(float input)
    {
        float FL_AckermanAngle = Mathf.Atan(AckermanOppositeDistance / (AckermanAdjacentDistance - (input * RearWheelOffset))) * Mathf.Rad2Deg * input;
        float FR_AckermanAngle = Mathf.Atan(AckermanOppositeDistance / (AckermanAdjacentDistance + (input * RearWheelOffset))) * Mathf.Rad2Deg * input;

        //currentSteerAngle = maximumSteerAngle * input;
        wheel_FL.UpdateWheelAngle(FL_AckermanAngle);
        wheel_FR.UpdateWheelAngle(FR_AckermanAngle);
        //WheelOBJ_FL.transform.localRotation = Quaternion.Euler(0, currentSteerAngle, 0);
        //WheelOBJ_FR.transform.localRotation = Quaternion.Euler(0, currentSteerAngle, 0);
    }

    protected void ApplySteerForce(float input)
    {

        //transform.RotateAround(SteerPoint.position, Vector3.up, currentSteerAngle * Time.deltaTime * rb.velocity.magnitude);
        //float leftTorque = wheel_FL.SteerVehicle(rb.GetPointVelocity(WheelOBJ_FL.transform.position));
        //float rightTorque = wheel_FR.SteerVehicle(rb.GetPointVelocity(WheelOBJ_FR.transform.position));
        
        //rb.AddTorque(transform.up * currentSteerAngle * 1400);
        //rb.AddForceAtPosition(transform.right * leftTorque * input, WheelOBJ_BL.transform.position);
        //rb.AddForceAtPosition(transform.right * rightTorque * input, WheelOBJ_BR.transform.position);
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
