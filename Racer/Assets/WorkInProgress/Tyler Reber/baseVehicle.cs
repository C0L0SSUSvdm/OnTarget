using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

public class baseVehicle : MonoBehaviour
{
    [SerializeField] protected AudioClip engineSound;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected FlockObject myFlockObject;
    [Header("----- Vehicle Fields -----")]
    private float Helper_ConstantInverse_5252 = 1.0f / 5252.0f;
    //[SerializeField] CharacterController controller;
    [SerializeField] protected float maximumSteerAngle;
    [SerializeField] protected float currentSteerAngle;
    //[SerializeField] protected float MotorForce = 10.0f;
    [SerializeField] bool isClutchEngaged = true;

    [SerializeField] float CurrentRPM = 0;
    //[SerializeField] float RunTimeCompressionRatio;
    [SerializeField] float RunTimeMeanRPM;
    [SerializeField] float RunTimeCurveDeviation_Inverse;
    [SerializeField] float RunTimeBackPressure;
    [SerializeField] float RunTimeCombustionForce;
    [SerializeField] float RunTimeMotorPower;
    //[SerializeField] float ThrottleRepsonse = 1000.0f;
    
    float RunTimeWheelTrainRatio;

    //https://www.hagerty.com/media/maintenance-and-tech/10-factors-that-influence-an-engines-character/#:~:text=How%20an%20engine%20breathes%20is,and%20fuel%20into%20the%20cylinders.
    [Header("----- Engine Item Fields -----")]
    //[Range(0, 100000), SerializeField] float TempEnginePower = 10000; //Temporary Variable until Transmission scales properly
    [SerializeField] Engine Engine;
    [Range(0.01f, 1.0f), SerializeField] float EngineEfficiency = 0.3f;
    [Range(0.1f, 1.0f), SerializeField] float CamShaftRadius = 0.75f;// length = pi r
    [Tooltip("Value assumed to be in inches")]
    [Range(1.00f, 3.0f), SerializeField] float CrankShaftRadius = 2.2f; //CrankThrow Radius is different from CrankShaft Radius
    [SerializeField] float CylinderCount = 8;
    //[SerializeField] float CylinderVolume;
    [SerializeField] float CrankShaftAngle = 45; // 1 / (Number of Cylinders), can be other angles
    [SerializeField] float RedLineRPM = 8000;
    [SerializeField] float EngineCompressionRatio = 10.0f;
    //[SerializeField] float PistonDiameter = 4.0f;

    [Header ("---- Air Control Valve -----")]
    [Range(500, 1500), SerializeField] float IdleRPMs = 1500;
    [Header("----- Piston Fields -----")]
    [Range(-20, 20), SerializeField] float PistonCC = 0;
    [Header("----- Engine Header -----")] //Phase Out
    //[Range(50, 200), SerializeField] float ChamberVolume = 59.0f;
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
    [Range(0, 1), SerializeField] float WheelLoadFactor = 0.1f;
    [Header("----- Turbo Fields -----")]
    [Range(1.0f, 5.0f), SerializeField] float TurboBoost = 2.3f;
    [Header("----- Exhaust Header -----")]
    [Range(0.1f, 1.0f), SerializeField] float ExhaustHeaderBackPressure = 0.5f;
    [Header("----- Exhaust Pipes -----")] 
    [SerializeField] float ExhaustPipeBackPressure = 1.3f;
    [Header("----- Carborator-----")]
    [Range(14.7f, 25.0f), SerializeField] float CompressedAirPressure = 14.7f;//14.7 is atmospheric pressure

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
        myFlockObject = gameObject.transform.Find("Chasis").gameObject.GetComponent<FlockObject>();

        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.mass = 1000;
        rb.drag = 0.1f;// 0.5f;
        rb.angularDrag = 0.5f;// 0.5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.maxAngularVelocity = 7;

        WheelOBJ_FL = transform.Find("Suspension_Left_Front").gameObject;
        WheelOBJ_FR = transform.Find("Suspension_Right_Front").gameObject;
        WheelOBJ_BL = transform.Find("Suspension_Left_Back").gameObject;
        WheelOBJ_BR = transform.Find("Suspension_Right_Back").gameObject;

        wheel_FL = WheelOBJ_FL.GetComponent<Suspension>();
        wheel_FR = WheelOBJ_FR.GetComponent<Suspension>();
        wheel_BL = WheelOBJ_BL.GetComponent<Suspension>();
        wheel_BR = WheelOBJ_BR.GetComponent<Suspension>();
        //ReadAxel_forcePoint = new Vector3(0, -0.5f, (WheelOBJ_BL.transform.localPosition.z * 0.5f));

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

    protected void FixedUpdate()
    {
        //CalculateGravity();

        float sumofDistance = wheel_FL.COMDistance(COM) + wheel_FR.COMDistance(COM) + wheel_BL.COMDistance(COM) + wheel_BR.COMDistance(COM);
        float sumOfDistances_Inverse = sumofDistance != 0 ? 1 / sumofDistance : 0;
        wheel_FL.SetWeightOnWheel(sumOfDistances_Inverse, rb.mass * Physics.gravity.y);
        wheel_FR.SetWeightOnWheel(sumOfDistances_Inverse, rb.mass * Physics.gravity.y);
        wheel_BL.SetWeightOnWheel(sumOfDistances_Inverse, rb.mass * Physics.gravity.y);
        wheel_BR.SetWeightOnWheel(sumOfDistances_Inverse, rb.mass * Physics.gravity.y);

        float sumOfCompression = wheel_FL.SphereCastWheelDistance() + wheel_FR.SphereCastWheelDistance() + wheel_BL.SphereCastWheelDistance() + wheel_BR.SphereCastWheelDistance();
        float sumOfCompression_Inverse = sumOfCompression != 0 ? 1 / sumOfCompression: 0;
        wheel_FL.SetMassOnWheel(sumOfCompression_Inverse, rb.mass);
        wheel_FR.SetMassOnWheel(sumOfCompression_Inverse, rb.mass);
        wheel_BL.SetMassOnWheel(sumOfCompression_Inverse, rb.mass);
        wheel_BR.SetMassOnWheel(sumOfCompression_Inverse, rb.mass);

        wheel_FL.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_FL.transform.position));
        wheel_FR.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_FR.transform.position));
        wheel_BL.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_BL.transform.position));
        wheel_BR.UpdateSpringPhysics(rb.GetPointVelocity(WheelOBJ_BR.transform.position));

        //ApplyGravity();
    }
    
    private float ShiftTranmission(float WheelVelocity)
    {
        float WheelTrainRatio = DifferentialRatio * GearRatio[GearIndex];
        float WheelAngularVelocity = WheelVelocity / RearTireRadius;
        CurrentRPM = WheelAngularVelocity * WheelTrainRatio * (60 / (2 * Mathf.PI));


        if (CurrentRPM < ShiftDownRPM && GearIndex > 0)
        {
            GearIndex--;
            WheelTrainRatio = DifferentialRatio * GearRatio[GearIndex];
            CurrentRPM = (WheelVelocity / RearTireRadius) * WheelTrainRatio * (60 / (2 * Mathf.PI));
        }
        else if (CurrentRPM > ShiftUpRPM && GearIndex < GearRatio.Length - 1)
        {
            GearIndex++;
            WheelTrainRatio = DifferentialRatio * GearRatio[GearIndex];
            CurrentRPM = (WheelVelocity / RearTireRadius) * WheelTrainRatio * (60 / (2 * Mathf.PI));
        }

        // Clamp RPM to valid range
        CurrentRPM = Mathf.Clamp(CurrentRPM, IdleRPMs, RedLineRPM);

        return WheelTrainRatio;
    }


    protected void ApplyGasPedal(float input)
    {
        if(engineSound != null)
        {
            gameManager.instance.SFXOneShot(engineSound);
        }
        
        float averageWheelVelocity = (rb.GetPointVelocity(WheelOBJ_BL.transform.position) + rb.GetPointVelocity(WheelOBJ_BR.transform.position)).magnitude * 0.5f;
        RunTimeWheelTrainRatio = ShiftTranmission(averageWheelVelocity);
        float testPower = 800f;// EngineCompressionRatio * CylinderCount * backPressure;
        //TODO: Calculate MotorPower based on Car Components and use Torque
        float backPressure = ExhaustHeaderBackPressure + ExhaustPipeBackPressure;
        float CrankTorque = CrankShaftRadius * Mathf.Sin(CrankShaftAngle) * (RunTimeCombustionForce) * backPressure;// * RunTimeWheelTrainRatio;
        float wheelTorque = RunTimeWheelTrainRatio * CrankTorque;

        float wheelAngularVelocity = CalculateWheelAngularVelocity();
        float deltaRPM = CalculateDeltaRPM(input * wheelTorque, testPower, wheelAngularVelocity);
        
        //Normal DistributionCurve Used to simulate Engine's physical limitations, uses
        float exponent = CalculateCurveExponent(CurrentRPM);
        float NormalCurveRatio = Mathf.Pow(2.5f, exponent);

        


        //

        //Debug.Log($"Torque:{CrankTorque}, {deltaRPM}, {NormalCurveRatio}, {(deltaRPM + CurrentRPM) * NormalCurveRatio + wheelTorque} ");


        RunTimeMotorPower = (deltaRPM + CurrentRPM) * NormalCurveRatio + wheelTorque;

        float WheelAngularVelocity = averageWheelVelocity / (RearTireRadius);

        wheel_FL.DriveWheel(0, rb.GetPointVelocity(WheelOBJ_FL.transform.position), WheelAngularVelocity);
        wheel_FR.DriveWheel(0, rb.GetPointVelocity(WheelOBJ_FR.transform.position), WheelAngularVelocity);
        wheel_BL.DriveWheel(input * RunTimeMotorPower, rb.GetPointVelocity(WheelOBJ_BL.transform.position), WheelAngularVelocity);
        wheel_BR.DriveWheel(input * RunTimeMotorPower, rb.GetPointVelocity(WheelOBJ_BR.transform.position), WheelAngularVelocity);
    }

    protected void UpdateSteeringAngle(float input)
    {
        
        float FL_AckermanAngle = Mathf.Atan(AckermanOppositeDistance / (AckermanAdjacentDistance - (input * RearWheelOffset))) * Mathf.Rad2Deg * input;
        float FR_AckermanAngle = Mathf.Atan(AckermanOppositeDistance / (AckermanAdjacentDistance + (input * RearWheelOffset))) * Mathf.Rad2Deg * input;

        wheel_FL.UpdateWheelAngle(FL_AckermanAngle);
        wheel_FR.UpdateWheelAngle(FR_AckermanAngle);


        float xPosition = Mathf.Lerp(COM.transform.localPosition.x, -input * 10, Time.deltaTime * 2);
        COM.transform.localPosition = new Vector3(xPosition, COM.transform.localPosition.y, COM.transform.localPosition.z);

    }

    public float GetSteeringAngle()
    {
        return currentSteerAngle;
    }

    //https://www.symbolab.com/graphing-calculator/bell-curve-graph
    private void InitializeNormalDistibutionCurve()
    {
        //float minimumCompression = ChamberVolume + PistonCC;
        // Total Volume when piston at lowest Point / Volume when Piston at highest Point
        //float ConvertToCC = 16.387f;//cubic inches to Cubic Centimeter ratio
        //CylinderVolume = (PistonDiameter * PistonDiameter * 0.25f * Mathf.PI) * (12 * CrankShaftRadius * Mathf.PI); //Area * Height
        //float PistonArea = (PistonDiameter * PistonDiameter * 0.25f * Mathf.PI) * (12 * CrankShaftRadius * Mathf.PI) * 0.6f; //Area * Height * Material Coefficient
        //RunTimeCompressionRatio = ((PistonArea * ConvertToCC) + minimumCompression) / minimumCompression; //Switched to a static value for Compression Ratio
        RunTimeCombustionForce = CompressedAirPressure * EngineCompressionRatio;


        //Deviation for Curve
        //RunTimeMeanRPM = (RedLineRPM - IdleRPMs) * 0.5f;
        float deviation = RedLineRPM * 0.25f;
        RunTimeCurveDeviation_Inverse = 1 / (2 * deviation * deviation);
    }

    private float CalculateCurveExponent(float RPM)
    {
        float xminusMean = (RPM - (RedLineRPM * 0.5f));
        float Squared = xminusMean * xminusMean;
        float exponent = (Squared * RunTimeCurveDeviation_Inverse);
        //Make Sure to return a negative value to make the bell curve
        
        return -exponent;
    }



    private float CalculateDeltaRPM(float TorqueInput, float HorsePower, float wheelAngularVelocity)
    {
        // Calculate load based on wheel speed
        float load = wheelAngularVelocity * WheelLoadFactor;
        float deltaRPM = 0;
        // Update RPM based on gas pedal input and load
        if (TorqueInput != 0)
        {
            deltaRPM = (HorsePower / (TorqueInput * Helper_ConstantInverse_5252));
            //Debug.Log($"{400}, TorqueConstant = {TorqueInput} * {Helper_ConstantInverse_5252}");
        }

        deltaRPM -= load;
        //float deltaRPM = (TorqueInput - load) * Time.deltaTime;
        //CurrentRPM += deltaRPM;

        // Clamp RPM to valid range
        //CurrentRPM = Mathf.Clamp(CurrentRPM, IdleRPMs, RedLineRPM);
        return deltaRPM;
    }

    private float CalculateWheelAngularVelocity()
    {
        //RunTimeWheelTrainRatio = DifferentialRatio * GearRatio[GearIndex];
        return (CurrentRPM * 2 * Mathf.PI) / (60 * RunTimeWheelTrainRatio);
    }

}

//private float CalculateMotorForcePDF()
//{
//    //float exponent = -Mathf.Pow(rpm - RunTimeMeanRPM, 2) / (2 * Mathf.Pow(stdDev, 2));
//    float motorForce = CalculateCurveExponent();
//    return motorForce;
//}

//// Method to calculate motor force using CDF (cumulative distribution)
//private float CalculateMotorForceCDF()
//{
//    float cdfValue = 0.5f * (1 + Erf((CurrentRPM - RunTimeMeanRPM) / (RedLineRPM / (CylinderCount) * Mathf.Sqrt(2))));
//    float motorForce = cdfValue;
//    return cdfValue;
//}

//// Error function approximation (required for CDF)
//private float Erf(float x)
//{
//    // Abramowitz and Stegun approximation of the error function
//    float a1 = 0.254829592f;
//    float a2 = -0.284496736f;
//    float a3 = 1.421413741f;
//    float a4 = -1.453152027f;
//    float a5 = 1.061405429f;
//    float p = 0.3275911f;

//    int sign = x < 0 ? -1 : 1;
//    x = Mathf.Abs(x);

//    float t = 1f / (1f + p * x);
//    float y = 1f - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Mathf.Exp(-x * x);

//    return sign * y;
//}

//// Hybrid method: Combines CDF for low-to-mid RPM and PDF for high RPM
//public float CalculateMotorForceHybrid(float rpm)
//{
//    float optimalRPM = RunTimeMeanRPM;
//    float stdDev = RedLineRPM * 0.25f;
//    float crossoverRPM = optimalRPM + stdDev; // Example crossover point
//    if (rpm <= crossoverRPM)
//    {
//        return CalculateMotorForceCDF();
//    }
//    else
//    {
//        return CalculateMotorForcePDF();
//    }
//}