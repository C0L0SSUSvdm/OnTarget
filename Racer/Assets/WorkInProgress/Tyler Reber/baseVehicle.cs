using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    [SerializeField] protected int CurrentRPM = 0;
    //[SerializeField] float RunTimeCompressionRatio;
    [SerializeField] float RunTimeMeanRPM;
    [SerializeField] float RunTimeCurveDeviation_Inverse;
    [SerializeField] float RunTimeBackPressure;
    [SerializeField] float RunTimeCombustionForce;
    [SerializeField] float RunTimeCrankTorque;
    [SerializeField] float RunTimeHorsePower;
    [SerializeField] float RunTimeMotorPower;
    [SerializeField] int RedLineRPM;
    //[SerializeField] float ThrottleRepsonse = 1000.0f;

    float RunTimeWheelTrainRatio;

    //https://www.hagerty.com/media/maintenance-and-tech/10-factors-that-influence-an-engines-character/#:~:text=How%20an%20engine%20breathes%20is,and%20fuel%20into%20the%20cylinders.
    [Header("----- Engine Item Fields -----")]
    //[Range(0, 100000), SerializeField] float TempEnginePower = 10000; //Temporary Variable until Transmission scales properly
    [SerializeField] Engine Engine;
    [Range(1.1f, 4.0f), SerializeField] float CylinderStrokeLength = 1.8f; //2
    [Range(1, 12), SerializeField] int CylinderCount = 6; //8
    [Range(1, 10), SerializeField] float CylinderDiameter = 3.5f; //4
    [SerializeField] int NumberOfStrokes = 4;
    


    [Header("----- Cam Shaft Item Fields -----")]
    [Range(100, 400), SerializeField] float IgnitionsPerSecond = 175.0f; //200

    [Range(0.1f, 1.0f), SerializeField] float CamShaftRadius = 0.75f;// length = pi r  
    [Range(0.5f, 0.9f), SerializeField] float PerformanceEfficiency = 0.70f;
    [Header("----- CrankShaft Item Fields -----")]
    [SerializeField] float CrankShaftAngle = 90; 
    [Range(0.5f, 1.0f), SerializeField] float CamShaftProfileEfficiency = 0.9f;
    //[SerializeField] float EngineCompressionRatio = 10.0f;

    [Header("----- Fly Wheel -----")]
    [Range(20.0f, 50.0f), SerializeField] float FlyWheelMass = 25;
    [Range(2.0f, 10.0f), SerializeField] float FlyWheelRadius = 4;
    [SerializeField] float FlyWheelInertia;

    [Header ("---- Air Control Valve -----")]
    [Range(500, 1500), SerializeField] int IdleRPMs = 1500;
    [Range(100, 800), SerializeField] float AirFlowRate = 450.0f;
    [Header("Valve")]
    [Range(0.01f, 0.5f), SerializeField] float ValveMass = 0.12f;
    [Header("Valve Spring")]
    [Range(10000.0f, 50000.0f), SerializeField] float SpringStiffness = 24000; // N/m


    [Header("----- Piston Fields -----")]
    [Range(10, 100), SerializeField] float PistonMass = 0;
    [Range(-1.0f, 1.0f), SerializeField] float PistonCompressionOffset = 0;
    [Range(1000, 8000), SerializeField] float PistonSpeed = 2300; //Feet/Min
    [Header("----- Cylinder Header -----")] //Phase Out
    [Range(0.1f, 10.0f), SerializeField] float ChamberCompressionArea = 65.0f; //CC
    [Range(0.01f, 0.2f), SerializeField] float HeaderBackPressure = 0.1f;
    [Header("----- Transmission Fields -----")]
    [SerializeField] protected int GearIndex = 0;
    [SerializeField] float ShiftTime = 0.1f;
    [SerializeField] float[] GearRatio = new float[6] { 3.82f, 2.2f, 1.4f, 1.0f, 0.8f, 0.6f };

    [Header("----- Transmission Control Module -----")]
    [Range(500, 8000), SerializeField] int ShiftDownRPM = 2200;
    [Range(1000, 12000), SerializeField] int ShiftUpRPM = 5000;

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
    [Header("----- Air Mixture and Compression-----")]// Carorator, fuel injection, airflow etc
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


    private void Awake()
    {
        
    }

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

        FlyWheelInertia = 0.5f * FlyWheelMass * Mathf.Pow(FlyWheelRadius, 2);

        CurrentRPM = IdleRPMs;
        CalculateCombustionForce();
       
        RunTimeBackPressure = ExhaustHeaderBackPressure + ExhaustPipeBackPressure + HeaderBackPressure;
        InitializeNormalDistibutionCurve();
        
    }

    protected void FixedUpdate()
    {
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

    }

   

    private float ShiftTranmission(float WheelVelocity)
    {
        float WheelTrainRatio = DifferentialRatio * GearRatio[GearIndex];
        float WheelAngularVelocity = WheelVelocity / 0.5f;
        float WheelRPM = (int)(WheelAngularVelocity * 60 / (2 * Mathf.PI));


        if (CurrentRPM < ShiftDownRPM && GearIndex > 0)
        {
            GearIndex--;
            WheelTrainRatio = DifferentialRatio * GearRatio[GearIndex];
            WheelRPM = (int)(WheelAngularVelocity * 60 / (2 * Mathf.PI));
        }
        else if (CurrentRPM > ShiftUpRPM && GearIndex < GearRatio.Length - 1)
        {
            GearIndex++;
            WheelTrainRatio = DifferentialRatio * GearRatio[GearIndex];
            WheelRPM = (int)(WheelAngularVelocity * 60 / (2 * Mathf.PI));
        }

        // Clamp RPM to valid range
        int rpm = (int)(WheelRPM * WheelTrainRatio);
        CurrentRPM = Mathf.Clamp(rpm, IdleRPMs, CurrentRPM * 2);

        return WheelTrainRatio;
    }
    
    private void CalculateRunTimeCrankTorque()
    {
        RunTimeCrankTorque = CylinderStrokeLength * Mathf.Sin(CrankShaftAngle) * (RunTimeCombustionForce) * RunTimeBackPressure;
    }

    protected void ApplyGasPedal(float input)
    {
        if(engineSound != null)
        {
            gameManager.instance.SFXOneShot(engineSound);
        }
        
        float averageWheelVelocity = (rb.GetPointVelocity(WheelOBJ_BL.transform.position) + rb.GetPointVelocity(WheelOBJ_BR.transform.position)).magnitude * 0.5f;
        RunTimeWheelTrainRatio = ShiftTranmission(averageWheelVelocity);

        float wheelTorque_ftP = (RunTimeWheelTrainRatio * RunTimeCrankTorque) - FlyWheelInertia; //Convert from Foot pounds to Newton Meters with 1.35582

        float wheelAngularVelocity = CalculateWheelAngularVelocity();
        float deltaRPM = CalculateDeltaRPM(input * wheelTorque_ftP, wheelAngularVelocity);

        float wheelTorque_Nm = wheelTorque_ftP * 1.35582f * 2;

        //Normal DistributionCurve Used to simulate Engine's physical limitations, uses
        float exponent = CalculateCurveExponent(CurrentRPM);
        float NormalCurveRatio = Mathf.Pow(RunTimeWheelTrainRatio, exponent);
    
        //Debug.Log($"Torque:{wheelTorque_Nm}, {deltaRPM}, {NormalCurveRatio}");

        RunTimeMotorPower = ((deltaRPM + CurrentRPM) * NormalCurveRatio) + wheelTorque_Nm;

        float WheelAngularVelocity = averageWheelVelocity / (RearTireRadius);

        wheel_FL.DriveWheel(0, rb.GetPointVelocity(WheelOBJ_FL.transform.position), WheelAngularVelocity);
        wheel_FR.DriveWheel(0, rb.GetPointVelocity(WheelOBJ_FR.transform.position), WheelAngularVelocity);
        wheel_BL.DriveWheel(input * RunTimeMotorPower, rb.GetPointVelocity(WheelOBJ_BL.transform.position), WheelAngularVelocity);
        wheel_BR.DriveWheel(input * RunTimeMotorPower, rb.GetPointVelocity(WheelOBJ_BR.transform.position), WheelAngularVelocity);
    }

    protected void ApplyBrake(float input)
    {
        float brakePower = 40000.0f;
        Vector3 forward = -transform.forward * 1000 * rb.velocity.magnitude;

        rb.AddForce(forward);
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

    private void CalculateCombustionForce()
    {
        //float minimumCompression = ChamberVolume + PistonCC;
        // Total Volume when piston at lowest Point / Volume when Piston at highest Point
        float ConvertToCC = 16.387f;//cubic inches to Cubic Centimeter ratio

        float CylinderArea = Mathf.PI * Mathf.Pow((CylinderDiameter / 2), 2.0f) * CylinderStrokeLength * ConvertToCC;
        float compressionRatio = (CylinderArea + ChamberCompressionArea) / ChamberCompressionArea;

        RunTimeCombustionForce = CompressedAirPressure * compressionRatio;
    }

    //https://www.symbolab.com/graphing-calculator/bell-curve-graph
    private void InitializeNormalDistibutionCurve()
    {
        CalculateRunTimeCrankTorque();
        RedLineRPM = (int)((CalculateRPM_IgnitionControl() + CalculateRPM_MaxAirFlow() + CalculateRPM_MaxPistonSpeed() + CalculateRPM_ValveControl())) / 4;
        CalculateRunTimeHorsePower();

        float deviation = RedLineRPM * 0.25f;
        RunTimeCurveDeviation_Inverse = 1 / (2 * deviation * deviation);
    }

    private float CalculateCurveExponent(float RPM)
    {
        float xminusMean = (RPM - (RedLineRPM * PerformanceEfficiency));
        float Squared = xminusMean * xminusMean;
        float exponent = (Squared * RunTimeCurveDeviation_Inverse);
        //Make Sure to return a negative value to make the bell curve
        
        return -exponent;
    }

    private void CalculateRunTimeHorsePower()
    {
        RunTimeHorsePower = (RunTimeCrankTorque * RedLineRPM) / 5252;
    }

    private float CalculateDeltaRPM(float TorqueInput, float wheelAngularVelocity)
    {
        // Calculate load based on wheel speed
        float load = wheelAngularVelocity * WheelLoadFactor;
        float deltaRPM = 0;
        // Update RPM based on gas pedal input and load
        if (TorqueInput != 0)
        {
            deltaRPM = (RunTimeHorsePower / (TorqueInput * Helper_ConstantInverse_5252));
        }
        deltaRPM -= load;
        return deltaRPM;
    }

    private float CalculateWheelAngularVelocity()
    {
        //RunTimeWheelTrainRatio = DifferentialRatio * GearRatio[GearIndex];
        return (CurrentRPM * 2 * Mathf.PI) / (60 * RunTimeWheelTrainRatio);
    }

    private float CalculateRPM_MaxAirFlow()
    {
        float CylinderRadiusFeet = (CylinderDiameter / 2.0f) / 12;
        float CrossSectionArea = Mathf.PI * Mathf.Pow(CylinderRadiusFeet, 2);
        return (AirFlowRate * CamShaftProfileEfficiency) / CrossSectionArea; //6500
    }

    private float CalculateRPM_MaxPistonSpeed()
    {
        float CylinderFeetRadius = CylinderStrokeLength / 12;
        return PistonSpeed / (2.0f * CylinderFeetRadius);
    }

    private float CalculateRPM_ValveControl()
    {
        float SpringFrequency = (1 / (2 * Mathf.PI)) * Mathf.Sqrt(SpringStiffness / ValveMass);
        float SafteyFactor = 1.5f; //Scale down below spring frequency
        return (SpringFrequency * 120) / SafteyFactor;
    }

    private float CalculateRPM_IgnitionControl()
    {
        return (IgnitionsPerSecond * 120 * (NumberOfStrokes * 0.5f)) / CylinderCount;
    }
}

