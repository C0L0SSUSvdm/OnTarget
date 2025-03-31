using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Suspension : MonoBehaviour
{
    public float weightForce = 0;
    public float compressedStrength;
    //[SerializeField] Vector3 SpringForces;
    [SerializeField] Vector3 DamperForces;
    //[SerializeField] Vector3 SteerForces;
    //[SerializeField] Vector3 MotorForces;
    [Header("----- Suspension Fields -----")]
    [Range(500, 10000), SerializeField] float SpringStrength;
    [Range(0, 1), SerializeField] float DampenerRate;
    [SerializeField] float Runtime_SpringStrength;
    [Range(0, 2), SerializeField] float EffectiveSpringLength;
    [SerializeField] float WheelMass;
    [SerializeField] int NumberOfCoils;
    [Header("RunTime Values -----")]
    float CenterOfMassDistance;
    Rigidbody rb;
    public bool isGrounded;

    [Header("----- Spring Spring Values -----")]
    //[SerializeField] float SpringrRestPosition = -0.25f;
    [SerializeField] float hitDistance;

    Vector3 WheelHitPoint;
    //[SerializeField] float SpringStrength;
    [SerializeField] float SpringRestPosition; //phaseout
    [SerializeField] float SpringRestMass;
    [SerializeField] float tiltDelta;
    [SerializeField] float deltaMass;
    [SerializeField] float weightOnWheel;
    [SerializeField] float massOnWheel;

    [Header("----- Wheel Values -----")]
    [SerializeField] GameObject Wheel;
    [SerializeField] float WheelFriction = 0.95f;
    [SerializeField] public float WheelRadius = 1.0f;
    [SerializeField] public float AngularVelocity; //w
    [SerializeField] public Vector3 WheelSlippage = new(0.8f, 0.0f, 0.05f); //Wheel Offset (x, y, z

    [Header("----- Visual Components -----")]
    [SerializeField] ParticleSystem WheelParticleSystem;
    [SerializeField] LineRenderer SpringHelix;
    [SerializeField] float SpringThickness = 0.035f;
    const int SpringCurvatureResolution = 30; //360 degree / 12 segements
    int NumberOfSpringSegements;
    [SerializeField] float SpringMechanicalOffset;
    [SerializeField] Vector3 SpringOffsets;
    [SerializeField] float SpringRadius;


    void Start()
    {

        //SpringStrength = MaximumSpringForce;
        EffectiveSpringLength *= -1;

        NumberOfSpringSegements = NumberOfCoils * (360 / SpringCurvatureResolution);

        SpringHelix.startWidth = SpringThickness;
        SpringHelix.endWidth = SpringThickness;
        SpringHelix.positionCount = NumberOfSpringSegements + 1; //Add 1 for the Fence Post
        SpringHelix.useWorldSpace = false;

        SpringMechanicalOffset = NumberOfCoils * 2 * SpringThickness;

        Runtime_SpringStrength = Mathf.Abs(SpringStrength * Physics.gravity.y);
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

    public void UpdateWheelAngle(float eulerAngle_y)
    {
        transform.localRotation = Quaternion.Euler(0, eulerAngle_y, 0);
        //Debug.DrawRay(transform.position, transform.right * 25, Color.blue);
        //Debug.DrawRay(transform.position, -transform.right * 25, Color.red);
    }

    public void DriveWheel(float TorqueForce, float EngineRpm, float WheelAngularVelocity)
    {
        

        if(TorqueForce != 0)
        {
            float EngineForce = 0;
            float max_friction_force = WheelFriction * Mathf.Abs(weightOnWheel);
            float Power_KE = 0.5f * massOnWheel * Mathf.Pow(rb.GetPointVelocity(gameObject.transform.position).magnitude, 2) * Time.fixedDeltaTime;
            if ((Mathf.Abs(TorqueForce) - Power_KE) > max_friction_force)
            {

                if (WheelParticleSystem.isPlaying == false)
                {
                    WheelParticleSystem.Play(); WheelParticleSystem.Play();
                }
                EngineForce = Mathf.Sign(TorqueForce) * max_friction_force;
            }
            else
            {
                if (WheelParticleSystem.isPlaying == true)
                {
                    WheelParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    
                    
                }

                EngineForce = TorqueForce;
            }

            if (isGrounded)
            {
                rb.AddForceAtPosition(transform.forward * EngineForce, WheelHitPoint, ForceMode.Force);
            }
            float theta = EngineRpm * 2 * Mathf.PI * Time.fixedDeltaTime;
            Wheel.transform.Rotate(theta, 0, 0);
        }
        else
        {
            AngularVelocity = WheelAngularVelocity;// / wheelradius
            float theta = (AngularVelocity / (2 * Mathf.PI)) * 360 * Time.fixedDeltaTime;
            Wheel.transform.Rotate(theta, 0, 0);
        }


        //SteerVehicle(EngineRpm);
    }

    public void ApplyBrakesWheel(float brakeInput)
    {
        if (isGrounded)
        {
            float maxBrakeForce = 20000;

            //float forwardVelocity = Vector3.Dot(rb.velocity.normalized, transform.right);

            // Calculate the brake force magnitude based on brake input and velocity
            float brakeForceMagnitude = brakeInput * maxBrakeForce;

            // Reduce rear wheel traction during braking to simulate weight transfer
            float rearTraction = Mathf.Clamp(1.0f - brakeInput, 0.5f, 1.0f); // Adjust as needed
            brakeForceMagnitude *= rearTraction;

            // Calculate the brake force vector (opposite to the wheel's velocity)
            Vector3 brakeForce = -rb.velocity.normalized * brakeForceMagnitude;

            // Apply the brake force at the wheel's contact point
            rb.AddForceAtPosition(brakeForce, WheelHitPoint, ForceMode.Force);


            if (rearTraction < 0.6f)
            {

                Vector3 lateralForce = transform.right * (rb.velocity.magnitude * 0.1f);
                rb.AddForceAtPosition(lateralForce, WheelHitPoint, ForceMode.Force);
            }
        }
    }

    public float SteerVehicle(Vector3 wheelVelocity)
    {
        float slipAngle = 0;

        //float angle = transform.transform.eulerAngles.y;
        if (isGrounded)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(wheelVelocity);

            slipAngle = Mathf.Atan2(localVelocity.x, Mathf.Abs(localVelocity.z)) * Mathf.Rad2Deg;

            float tireCoefficient = 0.3f;
            float lateralForceMagnitude = weightOnWheel * tireCoefficient * slipAngle * Time.deltaTime;

            Vector3 lateralSlipForce = transform.right * lateralForceMagnitude;

            Vector3 tireGripForce = -transform.right * (localVelocity.x * massOnWheel * 0.88f);

            rb.AddForceAtPosition(lateralSlipForce + tireGripForce, transform.position);
        }



        return slipAngle;
    }



    public float COMDistance(Transform CenterOfMass)
    {

        CenterOfMassDistance = Vector3.Distance(transform.localPosition, CenterOfMass.localPosition);
        return CenterOfMassDistance;

    }

    public float SphereCastWheelDistance()
    {
        Vector3 sphereCastOrigin = transform.position + transform.up * -transform.localPosition.y;
        float sphereRadius = WheelRadius * 0.5f;
        float maxDistance = WheelRadius + Mathf.Abs(EffectiveSpringLength);

        Debug.DrawRay(sphereCastOrigin, -transform.up * maxDistance, Color.red);

        RaycastHit hit;

        if (Physics.SphereCast(sphereCastOrigin, sphereRadius, -transform.up, out hit, maxDistance))
        {
            //Debug.Log(hit.transform.name);
            //if (hit.collider.CompareTag("Ground"))
            //{

            //}

            hitDistance = Mathf.Clamp(-(hit.distance - sphereRadius), EffectiveSpringLength, 0);
            WheelHitPoint = hit.point;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            hitDistance = EffectiveSpringLength;
        }

        return EffectiveSpringLength - hitDistance;
    }



    public void SetWeightOnWheel(float sumOfDistances_Inverse, float totalWeight)
    {
        weightOnWheel = CenterOfMassDistance * sumOfDistances_Inverse * totalWeight;
        SpringRestPosition = EffectiveSpringLength - (weightOnWheel / SpringStrength);
        SpringRestMass = weightOnWheel / Physics.gravity.y;
    }

    public void SetMassOnWheel(float sumOfCompression_Inverse_Mass, float ShiftedMass)
    {

        float delta = EffectiveSpringLength - hitDistance;
        if (isGrounded)
        {
            //tiltDelta = SpringRestMass - delta;
            massOnWheel = (delta * sumOfCompression_Inverse_Mass);

            //Debug.Log(ShiftedMass);
        }
        else
        {
            massOnWheel = WheelMass;
        }
    }

    public void UpdateSpringPhysics(Vector3 WheelWorldSpaceVelocity)
    {
        //Vector3 result = Vector3.zero;

        //float deltaSpringVelocity = (hitDistance - transform.localPosition.y) / Time.fixedDeltaTime;
        //transform.localPosition = new Vector3(transform.localPosition.x, hitDistance, transform.localPosition.z);
        //if (isGrounded)
        //{
        //    float DamperForce = deltaSpringVelocity * DampenerResistance;

        //    float massWeight = massOnWheel * Physics.gravity.y;
        //    //float deltaMass = (massWeight - weightOnWheel);
        //    float averageWeight = (massWeight + weightOnWheel) * 0.5f;


        //    SpringRestPosition = EffectiveSpringLength - (averageWeight / SpringStrength);
        //    float upwardForce = (transform.localPosition.y - SpringRestPosition) * SpringStrength;

        //    result = (-averageWeight + upwardForce + DamperForce) * transform.up;
        //    //SpringForces = transform.up * upwardForce;
        //    //DamperForces = transform.up * DamperForce;
        //    rb.AddForceAtPosition(result, WheelHitPoint, ForceMode.Force);
        //}
        Vector3 result = Vector3.zero;


        // Calculate the change in spring length over time
        float deltaDistance = hitDistance - transform.localPosition.y;
        float deltaSpringVelocity = deltaDistance / Time.fixedDeltaTime;
        transform.localPosition = new Vector3(transform.localPosition.x, hitDistance, transform.localPosition.z);

        if (isGrounded)
        {
            // Calculate damping force
            //Note: Calculated Damper Force is applied to Car, not the spring, therefor a decompressing spring doesn't dampen car's position in world space.
            //float DamperForce = Mathf.Clamp(deltaSpringVelocity * DampenerResistance, -1000, 99999);
            float DamperForce = deltaSpringVelocity * (SpringStrength * DampenerRate);
            float SpringForce = (EffectiveSpringLength - hitDistance) * Runtime_SpringStrength;

            compressedStrength = deltaDistance * Runtime_SpringStrength;
            weightForce = weightOnWheel - SpringForce;// + ((SpringRestMass - massOnWheel) * Physics.gravity.y);
            // Spring has to overcome the weigh on the spring,


            // Calculate the impact force based on the rigidbody's velocity
            float velocityAlongSpring = Vector3.Dot(WheelWorldSpaceVelocity, transform.up);
            float impactForce = velocityAlongSpring * massOnWheel; // F = m * v


            //Debug.Log($"{0}, force: {DamperForce}, ImpactForce: {impactForce}");
            // Combine all forces (spring, damping, and impact)
            result = (weightForce + DamperForce) * transform.up;

            // Apply the force to the rigidbody at the wheel's contact point
            rb.AddForceAtPosition(result, WheelHitPoint, ForceMode.Force);
        }
    }

}
