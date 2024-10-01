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
    [SerializeField] float WheelFriction = 0.95f;
    //[SerializeField] public float WheelRadius = 1.0f;
    [SerializeField] public float AngularVelocity; //w
    [SerializeField] public Vector3 WheelSlippage = new (0.8f, 0.0f, 0.05f); //Wheel Offset (x, y, z

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
        UpdateSpringGraphic();

    }

    void UpdateSpringGraphic()
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

    public float AddTorque(float EngineForce)
    {
        return Vector3.zero.y;
    }

    public void DriveWheel(float EngineForce, float WheelAngularVelocity)
    {
        if (isGrounded)
        {
            if(rb.velocity.magnitude > 0.3f) {
                float theta = rb.velocity.magnitude * 2 * Mathf.PI;
                //Wheel.transform.Rotate(theta, 0, 0);
            }
            //Quaternion rotation = Quaternion.Euler(0, transform.localRotation.y, 0);
            //Transfer weight into forward force to offset the friction from gravity since the wheels don't actually spin.
            Vector3 groundedFrictionReducer = (transform.forward * (massOnWheel * Mathf.Abs(Physics.gravity.y)) * WheelFriction);
            Vector3 ForceVector = (transform.forward) * EngineForce + groundedFrictionReducer;
            rb.AddForceAtPosition(ForceVector, transform.position - new Vector3(0, -EffectiveSpringLength, 0), ForceMode.Force);
        }
        else
        {
            AngularVelocity = WheelAngularVelocity;
            float theta = AngularVelocity;
            
        }



    }

    public float SteerVehicle(float angleStrength, Vector3 steerDirection)
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, angleStrength * Time.deltaTime, 0));

        Vector3 wheelWorldVelocity = rb.velocity;

        float SteerVelocity = Vector3.Dot(steerDirection, wheelWorldVelocity);

        float desiredSteerVelocity = -SteerVelocity * 0.9f; //Friction or slippage
        float acceleration = desiredSteerVelocity;
        //rb.AddTorque(transform.up * force);
        //rb.AddForceAtPosition(steerDirection * acceleration, transform.position, ForceMode.Acceleration);
        Debug.Log($"Steer Direction: {desiredSteerVelocity}, Acceleration: {acceleration}, force: {acceleration * massOnWheel}");
        return acceleration;
    }

    public float COMDistance(Transform CenterOfMass)
    {
        CenterOfMassDistance = 0;
        if (isGrounded)
        {
            CenterOfMassDistance = Vector3.Distance(transform.localPosition, CenterOfMass.localPosition);
        }
        return CenterOfMassDistance;
        //return Vector3.Distance(transform.localPosition, CenterOfMass.localPosition);
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

    public float UpdateSpringPhysics(Vector3 WheelVelocity)
    {
        //Power is Velocity * Mass
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
            //returnedForce.y = (newPhysicalWheelPosition.y + EffectiveSpringLength) * SpringStrength;
        }      
        transform.localPosition = newPhysicalWheelPosition;

        if(isGrounded == false && transform.localPosition.y > EffectiveSpringLength){
            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(SpringOffsets.x, 0, 0), -transform.up, out hit, 0.5f))
            {
                transform.localPosition += Vector3.up * Mathf.Lerp(transform.localPosition.y, transform.localPosition.y + hit.distance, Time.deltaTime * 2);
            }
        }



        return 0;
    }

    public void OnTriggerEnter(Collider other)
    {
        isGrounded = true;
        if (transform.localPosition.y <= EffectiveSpringLength)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.forward * 0.25f, -transform.up, out hit, EffectiveSpringLength))
            {
                if (other.gameObject != Wheel.gameObject)
                {
                    isGrounded = true;
                    transform.localPosition = new Vector3(transform.localPosition.x, hit.distance, transform.localPosition.z);
                }
                
                
            }
        }
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
