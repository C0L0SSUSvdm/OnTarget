using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using UnityEditor.ShaderGraph.Internal;
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

    [Header("----- RunTime Parameters -----")]
    [SerializeField] protected int CurrentRPM = 0;
    //[SerializeField] float RunTimeCompressionRatio;
    [SerializeField] float RunTimePistonDisplacement;
    [SerializeField] float RunTimeCompressionRatio;
    [SerializeField] float RunTimeCombustionPressure; //psi
    [SerializeField] float RunTimeCombustionForce;
    [SerializeField] float RunTimeAirFlow;
    [SerializeField] float RunTimeMeanRPM;
    [SerializeField] float RunTimeCurveDeviation_Inverse;
    [SerializeField] float RunTimeBackPressure;

    [SerializeField] float RunTimeCrankTorque;
    [SerializeField] float RunTimeHorsePower;
    [SerializeField] float RunTimeWheelTorque;
    [SerializeField] float RunTimeMotorPower;
    [SerializeField] int RedLineRPM;
    //[SerializeField] float ThrottleRepsonse = 1000.0f;
    [SerializeField] float FlyWheelInertia;
    [SerializeField] float RunTimeWheelTrainRatio;
    [Header("----- Load Factors and Reductions -----")]
    [SerializeField] float RunTImeLoadPistonFriction;
    [SerializeField] float RunTimeIgnitionTimingLoss;
    [SerializeField] float RunTimeThermalLoss;

    [Header("----- Car Parts -----")]
    [SerializeField] BaseCar Car;
    [SerializeField] Engine Engine;
    [SerializeField] CamShaft CamShaft;
    [SerializeField] CrankShaft CrankShaft;
    [SerializeField] FlyWheel FlyWheel;
    [SerializeField] AirController AirController;


    [Header("Valve")]
    public float ValveMass = 0.12f;
    [Header("Valve Spring")]
    [Range(10000.0f, 50000.0f), SerializeField] float SpringStiffness = 24000; // N/m
    [Header("----- Piston Fields -----")]
    [Range(10, 100), SerializeField] float PistonMass = 0;
    [Range(0, 0.5f), SerializeField] float PistonSize = 0.24f;
    [Range(0, 0.5f), SerializeField] float PistonRodLength = 0.25f;
    [Range(1000, 8000), SerializeField] float PistonSpeed = 2300; //Feet/Min
    [Range(0.01f, 0.2f), SerializeField] float FrictionCoefficient = 0.07f;
    [Range(50, 200), SerializeField] float PistonRingForce = 100; //Newtons
    [Header("----- Cylinder Header -----")] //Phase Out
    [Range(0.1f, 100.0f), SerializeField] float ChamberCompressionArea = 65.0f; //CC
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
        //Engine = gameManager.instance.DataManager().Engines().GetItem(Car.engineID) as Engine;
        //CamShaft = gameManager.instance.DataManager().CamShafts().GetItem(Car.camshaftID) as CamShaft;
        //CrankShaft = gameManager.instance.DataManager().CrankShafts().GetItem(Car.crankShaftID) as CrankShaft;
        //FlyWheel = gameManager.instance.DataManager().FlyWheel().GetItem(Car.flyWheelID) as FlyWheel;
        //AirController = gameManager.instance.DataManager().AirController().GetItem(Car.airControlerID) as AirController;
    }

    protected void Start()
    {
        
        myFlockObject = gameObject.transform.Find("Chasis").gameObject.GetComponent<FlockObject>();

        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        //rb.mass = 1000;
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

        FlyWheelInertia = 0.5f * FlyWheel.FlyWheelMass * Mathf.Pow(FlyWheel.FlyWheelRadius, 2);

        CurrentRPM = AirController.IdleRPMs;

       
        RunTimeBackPressure = ExhaustHeaderBackPressure + ExhaustPipeBackPressure + HeaderBackPressure;
        InitializeNormalDistibutionCurve();
        CalculateCylinderDisplacement();
        CalculateAirFlow();

        float sumofDistance = wheel_FL.COMDistance(COM) + wheel_FR.COMDistance(COM) + wheel_BL.COMDistance(COM) + wheel_BR.COMDistance(COM);
        float sumOfDistances_Inverse = sumofDistance != 0 ? 1 / sumofDistance : 0;
        wheel_FL.SetWeightOnWheel(sumOfDistances_Inverse, rb.mass * Physics.gravity.y);
        wheel_FR.SetWeightOnWheel(sumOfDistances_Inverse, rb.mass * Physics.gravity.y);
        wheel_BL.SetWeightOnWheel(sumOfDistances_Inverse, rb.mass * Physics.gravity.y);
        wheel_BR.SetWeightOnWheel(sumOfDistances_Inverse, rb.mass * Physics.gravity.y);

    }

    protected void FixedUpdate()
    {
        float sumOfCompression = wheel_FL.SphereCastWheelDistance() + wheel_FR.SphereCastWheelDistance() + wheel_BL.SphereCastWheelDistance() + wheel_BR.SphereCastWheelDistance();
        float sumOfCompression_Inverse = sumOfCompression != 0 ? 1 / sumOfCompression : 0;

        Vector3 carUp = transform.up;
        float rollAngle = Vector3.SignedAngle(carUp, Vector3.up, transform.forward);
        float pitchAngle = Vector3.SignedAngle(carUp, Vector3.up, transform.right);

        // Calculate weight shift factors
        float rollFactor = Mathf.Sin(rollAngle * Mathf.Deg2Rad);
        float pitchFactor = Mathf.Sin(pitchAngle * Mathf.Deg2Rad);

        float leftWeightFactor = 0.5f - rollFactor * 0.5f;
        float rightWeightFactor = 0.5f + rollFactor * 0.5f;
        float frontWeightFactor = 0.5f - pitchFactor * 0.5f;
        float rearWeightFactor = 0.5f + pitchFactor * 0.5f;

        // Distribute mass
        float FL = (frontWeightFactor * leftWeightFactor) * rb.mass;
        float FR = (frontWeightFactor * rightWeightFactor) * rb.mass;
        float BL = (rearWeightFactor * leftWeightFactor) * rb.mass;
        float BR = (rearWeightFactor * rightWeightFactor) * rb.mass;

        float depressionmass = sumOfCompression_Inverse * rb.mass;
        wheel_FL.SetMassOnWheel(depressionmass, FL);
        wheel_FR.SetMassOnWheel(depressionmass, FR);
        wheel_BL.SetMassOnWheel(depressionmass, BL);
        wheel_BR.SetMassOnWheel(depressionmass, BR);

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
        CurrentRPM = Mathf.Clamp(rpm, AirController.IdleRPMs, CurrentRPM * 2);

        return WheelTrainRatio;
    }
    
    private float CalculateRunTimeCrankTorque()
    {
        return (Engine.CylinderStrokeLength * 0.0254f / 2) * Mathf.Sin(Mathf.Deg2Rad * CrankShaft.CrankShaftAngle) * (RunTimeCombustionForce);
    }

    private void CalculateRunTimeHorsePower()
    {
        RunTimeHorsePower = (RunTimeCrankTorque * RedLineRPM) / 5252;
    }

    protected void ApplyGasPedal(float input)
    {
        
        //Normal DistributionCurve Used to simulate Engine's physical limitations, uses
        float exponent = CalculateCurveExponent(CurrentRPM);
        float NormalCurveRatio = Mathf.Pow(RunTimeWheelTrainRatio, exponent);

       
        float test = 0.5f;

        float CurrentAirFlow = (input == 0 ? 0.05f : input) * RunTimeAirFlow * CalculateRunTimeCrankTorque();
        RunTimeCrankTorque = CurrentAirFlow * NormalCurveRatio;

        CalculateRunTimeHorsePower();
        
        if(engineSound != null)
        {
            gameManager.instance.SFXOneShot(engineSound);
        }
        
        float averageWheelVelocity = (rb.GetPointVelocity(WheelOBJ_BL.transform.position) + rb.GetPointVelocity(WheelOBJ_BR.transform.position)).magnitude * 0.5f;
        RunTimeWheelTrainRatio = ShiftTranmission(averageWheelVelocity);

        float wheelTorque_ftP = (RunTimeWheelTrainRatio * RunTimeCrankTorque);// - FlyWheelInertia; //Convert from Foot pounds to Newton Meters with 1.35582

        float wheelAngularVelocity = CalculateWheelAngularVelocity();

        float peekPower = CalculatePeekPower(input, wheelAngularVelocity) * 0.5f;


        float wheelTorque_Nm = wheelTorque_ftP; //Divide by Wheel
        RunTimeWheelTorque = wheelTorque_Nm;


        //RunTimeMotorPower = (peekPower * NormalCurveRatio) + wheelTorque_Nm;
        RunTimeMotorPower = wheelTorque_Nm; // / 0.5f for the wheel radius
                                                    // float WheelAngularVelocity = averageWheelVelocity / (RearTireRadius);

        wheel_FL.DriveWheel(0, rb.GetPointVelocity(WheelOBJ_FL.transform.position), wheelAngularVelocity);
        wheel_FR.DriveWheel(0, rb.GetPointVelocity(WheelOBJ_FR.transform.position), wheelAngularVelocity);
        wheel_BL.DriveWheel(input * (RunTimeMotorPower), rb.GetPointVelocity(WheelOBJ_BL.transform.position), wheelAngularVelocity);
        wheel_BR.DriveWheel(input * (RunTimeMotorPower), rb.GetPointVelocity(WheelOBJ_BR.transform.position), wheelAngularVelocity);
    }

    protected void ApplyBrake(float input)
    {
        if(input != 0)
        {
            wheel_BL.ApplyBrakesWheel(input);
            wheel_BR.ApplyBrakesWheel(input);
        }
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

    private void CalculateCylinderDisplacement()
    {
        float boreRadius_cm = Engine.CylinderDiameter * 2.54f / 2f;
        float strokeLength_cm = Engine.CylinderStrokeLength * 0.75f * 2.54f;

        RunTimePistonDisplacement = (Mathf.PI * Mathf.Pow(boreRadius_cm, 2.0f) * strokeLength_cm);
        RunTimeCompressionRatio = (RunTimePistonDisplacement + ChamberCompressionArea) / ChamberCompressionArea;

        RunTimeCombustionPressure = CompressedAirPressure * RunTimeCompressionRatio;
        CalculateCombustionForce(boreRadius_cm);
    }

    private void CalculateCombustionForce(float boreRadius)
    {
        //After Combustion, 10 is a static temporary static value, Convert to pascals, Multiply by cubic meters
        float CylinderForce = RunTimeCombustionPressure * 10.0f * 6894.76f * (boreRadius / 10000);


        float fuelEfficiency = 0.9f;
        //Loads and Frictions
        RunTImeLoadPistonFriction = ((RunTimeCompressionRatio * RunTimePistonDisplacement) + PistonRingForce) * FrictionCoefficient * Engine.CylinderCount;
        RunTimeCombustionForce = (CylinderForce / (Engine.NumberOfStrokes / 2) * fuelEfficiency) - RunTImeLoadPistonFriction;
    }

    //https://www.symbolab.com/graphing-calculator/bell-curve-graph
    private void InitializeNormalDistibutionCurve()
    {
        //CalculateRunTimeCrankTorque();
        RedLineRPM = (int)((CalculateRPM_IgnitionControl() + CalculateRPM_MaxPistonSpeed() + CalculateRPM_ValveControl())) / 3;
        CalculateAirFlow();

        //Use Log to scale better with high RPM engine builds
        float deviationScalingFactor = 0.15f * Mathf.Log(RedLineRPM / 1000f + 1);
        //Debug.Log(deviationScalingFactor);
        float deviation = RedLineRPM * deviationScalingFactor;
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



    private float CalculatePeekPower(float Input, float wheelAngularVelocity)
    {
        // Calculate load based on wheel speed
        float load = wheelAngularVelocity * WheelLoadFactor;

        float power = (RedLineRPM / RunTimeWheelTrainRatio * Input - load) * Time.fixedDeltaTime * RunTimeHorsePower;


        return power;
    }

    private float CalculateWheelAngularVelocity()
    {
        //RunTimeWheelTrainRatio = DifferentialRatio * GearRatio[GearIndex];
        return (CurrentRPM * 2 * Mathf.PI) / (60 * RunTimeWheelTrainRatio);
    }

    private void CalculateAirFlow()
    {
        //Super Charger coefficient
        float volumetricEfficiency = 1.0f;

        RunTimeAirFlow = ((RunTimePistonDisplacement * Engine.CylinderCount * RedLineRPM) / 14158.4f) * volumetricEfficiency;

    }

    private float CalculateRPM_MaxPistonSpeed()
    {
        float CylinderFeetRadius = Engine.CylinderStrokeLength / 12;
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
        // 120 = second in per minute * 2
        return (CamShaft.IgnitionsPerSecond * 120 * (Engine.NumberOfStrokes * 0.5f)) / Engine.CylinderCount;
    }
}

