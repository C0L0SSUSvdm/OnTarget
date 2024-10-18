using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Suspension : MonoBehaviour
{
    
    [SerializeField] Vector3 SpringForces;
    [SerializeField] Vector3 DamperForces;
    [SerializeField] Vector3 SteerForces;
    [SerializeField] Vector3 MotorForces;
    [Header("----- Suspension Fields -----")]
    [Range(1000, 20000), SerializeField] float MaximumSpringForce;
    [Range(0, 2), SerializeField] float EffectiveSpringLength;
    [SerializeField] float WheelMass;    
    [SerializeField] float DampenerResistance;
    [SerializeField] int NumberOfCoils;
    [Header("RunTime Values -----")]
    float CenterOfMassDistance;
    Rigidbody rb;
    public bool isGrounded;

    [Header("----- Spring Spring Values -----")]
    //[SerializeField] float SpringrRestPosition = -0.25f;
    [SerializeField] float hitDistance;
    Vector3 WheelHitPoint;
    [SerializeField] float SpringStrength;
    [SerializeField] float weightOnWheel;
    [SerializeField] float massOnWheel;
    [SerializeField] float SpringRestPosition;
    [Header("----- Wheel Values -----")]
    [SerializeField] GameObject Wheel;
    [SerializeField] float WheelFriction = 0.95f;
    [SerializeField] public float WheelRadius = 1.0f;
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
        SpringStrength = MaximumSpringForce;
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

    //public float AddTorque(float EngineForce)
    //{
    //    return Vector3.zero.y;
    //}

    public void DriveWheel(float EngineForce, Vector3 wheelVelocity, float WheelAngularVelocity)
    {
        Vector3 ForceVector = Vector3.zero;
        if (isGrounded)
        {
            if(rb.velocity.magnitude > 0.3f) {
                float theta = rb.velocity.magnitude * 2 * Mathf.PI;
                Wheel.transform.Rotate(theta, 0, 0);
            }

            Vector3 test = transform.InverseTransformDirection(wheelVelocity);
            float right = EngineForce * test.x;
            ForceVector = (transform.forward * EngineForce) + (-transform.right * right);

            rb.AddForceAtPosition(ForceVector, WheelHitPoint, ForceMode.Force);
            
        }
        else
        {
            AngularVelocity = WheelAngularVelocity;
            float theta = AngularVelocity;
            Wheel.transform.Rotate(theta, 0, 0);
        }
        
    }

    public void UpdateWheelAngle(float eulerAngle_y)
    {
        transform.localRotation = Quaternion.Euler(0, eulerAngle_y, 0);
        Debug.DrawRay(transform.position, transform.right * 25, Color.blue);
        Debug.DrawRay(transform.position, -transform.right * 25, Color.red);
    }

    public float SteerVehicle(Vector3 wheelVelocity)
    {
        
        //rb.MoveRotation(rb.rotation * Quaternion.Euler(0, angleStrength * Time.deltaTime, 0));

        Vector3 test = transform.InverseTransformDirection(wheelVelocity);
        Vector3 right = transform.right * test.x;

        rb.AddForceAtPosition(right, transform.position, ForceMode.Force);

        float SteerVelocity = Vector3.Dot(transform.right, wheelVelocity);

        float desiredSteerVelocity = -SteerVelocity * 0.9f; //Friction or slippage
        float acceleration = desiredSteerVelocity;
        //rb.AddTorque(transform.up * force);
        //rb.AddForceAtPosition(steerDirection * acceleration, transform.position, ForceMode.Acceleration);
        //Debug.Log($"Steer Direction: {desiredSteerVelocity}, Acceleration: {acceleration}, force: {acceleration * massOnWheel}");
        return acceleration;
    }

    public float COMDistance(Transform CenterOfMass)
    {
        
        CenterOfMassDistance = Vector3.Distance(transform.localPosition, CenterOfMass.localPosition);
        return CenterOfMassDistance;

    }

    public float RayCastWheelDistance()
    {
        Vector3 RayCastPoint = transform.position + transform.right * SpringOffsets.x + transform.up * -transform.localPosition.y;
        Debug.DrawRay(RayCastPoint, -transform.up * (WheelRadius + Mathf.Abs(EffectiveSpringLength)), Color.red);
        RaycastHit hit;
        if(Physics.Raycast(RayCastPoint, -transform.up, out hit, WheelRadius + Mathf.Abs(EffectiveSpringLength)))   
        {
            //Debug.Log($"Name: {transform.name}, HitObj: {hit.transform.gameObject.name}");
            isGrounded = true;
            //Debug.Log($"{hit.transform.name}, distance: {hit.distance}, wheel rad: {WheelRadius}");
            //hitDistance = -(hit.distance - WheelRadius);
            hitDistance = Mathf.Clamp(-(hit.distance - WheelRadius), EffectiveSpringLength, 0);
            WheelHitPoint = hit.point;
        }
        else
        {
            isGrounded = false;
            hitDistance = EffectiveSpringLength;
        }



        return EffectiveSpringLength;// - hitDistance;
    }

    public void SetWeightOnWheel(float sumOfDistances_Inverse, float totalWeight)
    {
        weightOnWheel = CenterOfMassDistance * sumOfDistances_Inverse * totalWeight;
    }

    public float SuspensionDistance()
    {
        return EffectiveSpringLength - hitDistance;
    }

    public void SetMassOnWheel(float sumOfCompression_Inverse, float totalMass)
    {

        float delta = EffectiveSpringLength - hitDistance;
        if (isGrounded)
        {
            massOnWheel = delta * sumOfCompression_Inverse * totalMass;
        }
        else
        {
            massOnWheel = WheelMass;
        }
    }

    public Vector3 UpdateSpringPhysics()
    {
        float result = 0;
        
        float deltaSpringVelocity = (hitDistance - transform.localPosition.y) / Time.fixedDeltaTime;
        transform.localPosition = new Vector3(transform.localPosition.x, hitDistance, transform.localPosition.z);
        if (isGrounded)
        {
            float DamperForce = deltaSpringVelocity * DampenerResistance;

            float averageWeight = (massOnWheel * Physics.gravity.y + weightOnWheel) * 0.5f;

            float restDistance = averageWeight / SpringStrength;
            float upwardForce = (EffectiveSpringLength - restDistance) * SpringStrength;
          
            result = (upwardForce - averageWeight) + DamperForce;
            SpringForces = transform.up * upwardForce;
            DamperForces = transform.up * DamperForce;
            rb.AddForceAtPosition(transform.up * result, WheelHitPoint, ForceMode.Force);
        }

        return transform.up * result;
    }

    public void OnTriggerEnter(Collider other)
    {
        

    }

    public void OnTriggerExit(Collider other)
    {


    }
}
