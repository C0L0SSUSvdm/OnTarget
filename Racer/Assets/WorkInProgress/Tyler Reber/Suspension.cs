using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Suspension : MonoBehaviour
{
    [Header("----- Suspension Fields -----")]
    [Range(1000, 100000), SerializeField] float MaximumSpringForce;
    [Range(0, 2), SerializeField] float EffectiveSpringLength;
    [SerializeField] float WheelMass;    
    [SerializeField] float DampenerResistance;
    [SerializeField] int NumberOfCoils;
    [Header("RunTime Values -----")]
    float CenterOfMassDistance;
    Rigidbody rb;
    public bool isGrounded;
    [Header("----- Spring Spring Values -----")]
    [SerializeField] float SpringStrength;
    [SerializeField] Vector3 WheelVelocity;   
    [SerializeField] float weightOnWheel;
    [SerializeField] float massOnWheel;
    [SerializeField] float SpringRestPosition;
    [Header("----- Wheel Values -----")]
    [SerializeField] GameObject Wheel;
    [SerializeField] public float WheelRadius = 1.0f;
    [SerializeField] public float AngularVelocity; //w


    [Header("----- Visual Components -----")]    
    [SerializeField] LineRenderer SpringHelix;
    [SerializeField] float SpringThickness = 0.035f;   
    const int SpringCurvatureResolution = 30; //360 degree / 12 segements
    int NumberOfSpringSegements;
    [SerializeField] float SpringMechanicalOffset;
    [SerializeField] Vector3 SpringOffsets;
    [SerializeField] float SpringRadius;
    

    void Start()
    {        
        SpringStrength = MaximumSpringForce / EffectiveSpringLength;
        EffectiveSpringLength *= -1;

        NumberOfSpringSegements = NumberOfCoils * (360 / SpringCurvatureResolution);

        SpringHelix.startWidth = SpringThickness;
        SpringHelix.endWidth = SpringThickness;
        SpringHelix.positionCount = NumberOfSpringSegements + 1; //Add 1 for the Fence Post
        SpringHelix.useWorldSpace = false;

        SpringMechanicalOffset = NumberOfCoils * 2 * SpringThickness;

    }
    
    void Update()
    {

        float segmentLength = (transform.localPosition.y + -SpringMechanicalOffset) / 360;
        Vector3 start = SpringOffsets;// transform.localPosition + SpringOffsets;
        start.y += -SpringMechanicalOffset;

        for (int i = 0; i <= NumberOfSpringSegements; i++)
        {
            //float theta = 2 * Mathf.PI * (-i * 7);
            float theta = SpringCurvatureResolution * -i; // 12 Segements per Coil
            Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.Euler(0, theta, 0));

            Vector3 nextpoint = new Vector3(rotation.m20, rotation.m21, rotation.m22) * SpringRadius + start;
            nextpoint.y = segmentLength * (-i * (SpringCurvatureResolution / (2 * Mathf.PI)));
            SpringHelix.SetPosition(i, nextpoint);
            start = nextpoint;
            
        }
    }



    public void InitializeSuspension(Rigidbody rigidBody)
    {
        rb = rigidBody;
    }

    public void DriveWheel(float RPMs, float torque)
    {
        torque *= WheelRadius;
        float theta = Wheel.transform.localRotation.eulerAngles.x;
        if (isGrounded)
        {
            //Calculate How much work is being done, Amount of force actually turning the wheel
            //Torque = Sum of Ratios * Crank Shaft Angle
            float currentSpeed = WheelVelocity.magnitude * (WheelRadius * 2 * Mathf.PI);
            float wheelRPM = currentSpeed / torque;


            //rb.AddForceAtPosition(transform.forward * power, transform.position);          
        }
        else
        {

        }
        Wheel.transform.localRotation = Quaternion.Euler(theta, 0, 0);
    }

    public void SteerVehicle()
    {
        //transform.Rotate(0, wheelFL_data.GetTurnAngle() * Time.deltaTime * rb.velocity.magnitude, 0);   
        Vector3 steerDirection = transform.right;
        Vector3 wheelWorldVelocity = rb.GetPointVelocity(transform.position);

        float SteerVelocity = Vector3.Dot(steerDirection, wheelWorldVelocity);

        float desiredSteerVelocity = -SteerVelocity * 0.9f; //Friction or slippage
        float acceleration = desiredSteerVelocity / Time.deltaTime;
        Debug.Log($"Steer Dir: {steerDirection}, acceleration: {acceleration}, weight: {massOnWheel}, total: {steerDirection * acceleration * massOnWheel}");
        rb.AddForceAtPosition(steerDirection * acceleration * 75, transform.position);
    }

    public float COMDistance(Transform CenterOfMass)
    {
        CenterOfMassDistance = 0;
        if (isGrounded)
        {
            CenterOfMassDistance = Vector3.Distance(transform.localPosition, CenterOfMass.localPosition);
        }
        return CenterOfMassDistance;
    }

    public void SetWeightOnWheel(float sumOfDistances_Inverse, float totalWeight)
    {
        weightOnWheel = CenterOfMassDistance * sumOfDistances_Inverse * totalWeight;
    }

    public float SuspensionDistance()
    {
        //float delta = 0;
        //if(isGrounded)
        //{
        //    delta = EffectiveSpringLength - transform.localPosition.y;
        //}
        return EffectiveSpringLength - transform.localPosition.y;
    }

    public void SetMassOnWheel(float sumOfCompression_Inverse, float totalMass)
    {
        float delta = EffectiveSpringLength - transform.localPosition.y;
        if (isGrounded && delta != 0)
        {
            massOnWheel = delta * sumOfCompression_Inverse * totalMass;
        }
        else
        {
            massOnWheel = WheelMass;
        }
    }

    public Vector3 UpdateSpringPhysics(Vector3 WheelVelocity)
    {
        Vector3 returnedForce = Vector3.zero;
        Vector3 newPhysicalWheelPosition = transform.localPosition;
        float inverseMass = massOnWheel != 0 ? 1 / massOnWheel : WheelMass;


        //If car is moving up, Spring decompresses the distance
        if (WheelVelocity.y > 0)
        {
            newPhysicalWheelPosition.y -= WheelVelocity.y * Time.deltaTime;
        }


        SpringRestPosition = isGrounded ? EffectiveSpringLength - (weightOnWheel / SpringStrength) : EffectiveSpringLength;
        SpringRestPosition = Mathf.Clamp(SpringRestPosition, EffectiveSpringLength, 0);

        float ConstanceForceOnWheel = (WheelVelocity.y / Time.deltaTime) * massOnWheel;

        float delta = transform.localPosition.y - SpringRestPosition;

        //Step: ? Calculate the velocity of the real spring movement
        Vector3 SpringCompressionVelocity = Vector3.up * (delta / Time.deltaTime);
        float CompressionForceDelta = Vector3.Dot(transform.up, SpringCompressionVelocity);

        float springForce = (delta * SpringStrength) - ConstanceForceOnWheel + CompressionForceDelta;
        float averageSpringDistance = (springForce * inverseMass) * Time.deltaTime * Time.deltaTime * 0.5f;
        
        newPhysicalWheelPosition.y -= averageSpringDistance;


        if (newPhysicalWheelPosition.y <= EffectiveSpringLength) //Stretched to the Max
        {
            newPhysicalWheelPosition.y = EffectiveSpringLength;
        }
        else if (newPhysicalWheelPosition.y >= 0)
        {
            newPhysicalWheelPosition.y = 0;
            returnedForce.y = (newPhysicalWheelPosition.y + EffectiveSpringLength) * SpringStrength;
        }
        
        transform.localPosition = newPhysicalWheelPosition;


        return returnedForce;
    }

    public void OnTriggerEnter(Collider other)
    {
        isGrounded = true;
    }

    public void OnTriggerExit(Collider other)
    {

        isGrounded = false;

        //if(transform.localPosition.y <= EffectiveSpringLength)
        //{
        //    RaycastHit hit;
        //    if (Physics.Raycast(transform.position, -transform.up, out hit, 1))
        //    {
        //        if(other.gameObject != this){

        //        }
        //        float position = Mathf.Clamp
        //        isGrounded = true;
        //    }
        //}
        //else
        //{
        //    isGrounded = false;
        //}

    }
}
