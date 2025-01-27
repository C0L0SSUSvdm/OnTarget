using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Suspension : MonoBehaviour
{
    public float test = 0;
    //[SerializeField] Vector3 SpringForces;
    [SerializeField] Vector3 DamperForces;
    //[SerializeField] Vector3 SteerForces;
    //[SerializeField] Vector3 MotorForces;
    [Header("----- Suspension Fields -----")]
    [Range(2000, 60000), SerializeField] float SpringStrength;
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
    //[SerializeField] float SpringStrength;
    [SerializeField] float weightOnWheel;
    [SerializeField] float massOnWheel;
    [SerializeField] float SpringRestPosition;
    [Header("----- Wheel Values -----")]
    [SerializeField] GameObject Wheel;
    [SerializeField] float WheelFriction = 0.95f;
    [SerializeField] public float WheelRadius = 1.0f;
    [SerializeField] public float AngularVelocity; //w
    [SerializeField] public Vector3 WheelSlippage = new(0.8f, 0.0f, 0.05f); //Wheel Offset (x, y, z

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
        //SpringStrength = MaximumSpringForce;
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

    public void UpdateWheelAngle(float eulerAngle_y)
    {
        transform.localRotation = Quaternion.Euler(0, eulerAngle_y, 0);
        //Debug.DrawRay(transform.position, transform.right * 25, Color.blue);
        //Debug.DrawRay(transform.position, -transform.right * 25, Color.red);
    }

    public void DriveWheel(float EngineForce, Vector3 wheelVelocity, float WheelAngularVelocity)
    {
        AngularVelocity = WheelAngularVelocity;// / wheelradius
        float theta = (AngularVelocity / (2 * Mathf.PI)) * 360 * Time.deltaTime;
        Wheel.transform.Rotate(theta, 0, 0);

        if (isGrounded)
        {
            rb.AddForceAtPosition(transform.forward * EngineForce, WheelHitPoint, ForceMode.Force);
        }

        SteerVehicle(wheelVelocity);
    }

    public float SteerVehicle(Vector3 wheelVelocity)
    {
        float angle = transform.transform.eulerAngles.y;

        Vector3 localVelocity = transform.InverseTransformDirection(wheelVelocity);

        float slipAngle = Mathf.Atan2(localVelocity.x, Mathf.Abs(localVelocity.z)) * Mathf.Rad2Deg;

        float tireCoefficient = 1.0f;
        float lateralForceMagnitude = weightOnWheel * tireCoefficient * slipAngle * Time.deltaTime;
        Vector3 lateralForce = transform.right * lateralForceMagnitude;


        rb.AddForceAtPosition(lateralForce, transform.position);



        //rb.AddForceAtPosition(localVelocity.x * -transform.right * massOnWheel, transform.position);
        Debug.DrawRay(transform.position, (localVelocity.x * -transform.right * massOnWheel) * 0.1f, Color.cyan);
        Debug.DrawRay(transform.position, (localVelocity.x * -transform.right * massOnWheel) * 0.1f, Color.red);

        Debug.Log(slipAngle);
        return slipAngle;
    }

    public float CalculateLateralForce(float SlipAngle)
    {
        float tireCoefficient = 1.0f;
        float maxforce = weightOnWheel * tireCoefficient;

        float slipRatio = SlipAngle / 15.0f;
        slipRatio = Mathf.Clamp(slipRatio, -1, 1);
        float lateralForce = maxforce * (1f - Mathf.Pow(slipRatio, 2f)) * Time.deltaTime;

        return lateralForce;
    }


    public float COMDistance(Transform CenterOfMass)
    {

        CenterOfMassDistance = Vector3.Distance(transform.localPosition, CenterOfMass.localPosition);
        return CenterOfMassDistance;

    }

    public float RayCastWheelDistance()
    {
        Vector3 RayCastPoint = transform.position + transform.up * -transform.localPosition.y;
        Debug.DrawRay(RayCastPoint, -transform.up * (WheelRadius + Mathf.Abs(EffectiveSpringLength)), Color.red);

        RaycastHit hit;
        
        if (Physics.Raycast(RayCastPoint, -transform.up, out hit, WheelRadius + Mathf.Abs(EffectiveSpringLength)))
        {
            hitDistance = Mathf.Clamp(-(hit.distance - WheelRadius), EffectiveSpringLength, 0);
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

    public void UpdateSpringPhysics(Vector3 WheelWorldSpaceVelocity)
    {
        Vector3 result = Vector3.zero;

        float deltaSpringVelocity = (hitDistance - transform.localPosition.y) / Time.fixedDeltaTime;
        transform.localPosition = new Vector3(transform.localPosition.x, hitDistance, transform.localPosition.z);
        if (isGrounded)
        {
            float DamperForce = deltaSpringVelocity * DampenerResistance;

            float massWeight = massOnWheel * Physics.gravity.y;
            float deltaMass = (massWeight - weightOnWheel);
            float averageWeight = (massWeight + weightOnWheel) * 0.5f;


            SpringRestPosition = EffectiveSpringLength - (averageWeight / SpringStrength);
            float upwardForce = (transform.localPosition.y - SpringRestPosition) * SpringStrength;

            result = (-averageWeight + upwardForce + DamperForce) * transform.up;
            //SpringForces = transform.up * upwardForce;
            DamperForces = transform.up * DamperForce;
            rb.AddForceAtPosition(result, WheelHitPoint, ForceMode.Force);
        }

    }



    public void OnTriggerEnter(Collider other)
    {


    }

    public void OnTriggerExit(Collider other)
    {


    }
}
