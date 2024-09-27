using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;

public class baseCar : GravityBody
{
    [Header("----- Temporary Fields -----")]
    [Range(1, 100000), SerializeField] float SpringForce;
    [Tooltip("The Length of the Shock Absorber, Will likely be moved to item")]
    [Range(1, 100), SerializeField] float wheelMass = 1;
    [Range(0, 2), SerializeField] float SpringLength = 1.2f;
    [Range(1, 100000), SerializeField] float SpringDamper = 1;
    [SerializeField] float SteerAngle;
    [SerializeField] protected float MotorForce = 10.0f;


    [Header("Car Parts")]
    [SerializeField] GameObject steeringColumn;
    [SerializeField] GameObject Chasis;
    [SerializeField] GameObject wheelFL;
    [SerializeField] GameObject wheelFR;
    [SerializeField] GameObject wheelBL;
    [SerializeField] GameObject wheelBR;

    [SerializeField] float wheelFL_distance;
    [SerializeField] float wheelFR_distance;
    [SerializeField] float wheelBL_distance;
    [SerializeField] float wheelBR_distance;

    [SerializeField] float wheelFL_mass = 0;
    [SerializeField] float wheelFR_mass = 0;
    [SerializeField] float wheelBL_mass = 0;
    [SerializeField] float wheelBR_mass = 0;

    Wheel wheelFL_data;
    Wheel wheelFR_data;
    Wheel wheelBL_data;
    Wheel wheelBR_data;


    [Header("----- Environment Fields -----)")]
    [SerializeField] Vector3 windDirection;
    [SerializeField] GameObject CenterOfMass;

    void Start()
    {
        wheelFL_data = wheelFL.GetComponent<Wheel>();
        wheelFR_data = wheelFR.GetComponent<Wheel>();
        wheelBL_data = wheelBL.GetComponent<Wheel>();
        wheelBR_data = wheelBR.GetComponent<Wheel>();

        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 1000;
        rb.drag = 0;// 0.5f;
        rb.angularDrag = 0;// 0.5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        wheelFL_data.SetMaxTurnAngle(35);
        wheelFR_data.SetMaxTurnAngle(35);

        UpdateSprings();
        
    }


    // Update is called once per frame
    public new void Update()
    {
        //base.Update();
        CalculateGravity();




        //int groundedWheels = 0;
        float sumOfDistances = 0;
        float sumOfSuspension = 0;
       
        if (wheelFL_data.isGrounded)
        {
            //groundedWheels++;
            wheelFL_distance = Vector3.Distance(wheelFL_data.transform.localPosition, CenterOfMass.transform.localPosition);
            sumOfDistances += wheelFL_distance;
            sumOfSuspension += wheelFL_data.GetCompressedDistance();
            //Debug.Log($"{sumOfSuspension}, FL: {wheelFL_data.GetCompressedDistance()}");
        }
        if (wheelFR_data.isGrounded)
        {
            //groundedWheels++;
            wheelFR_distance = Vector3.Distance(wheelFR_data.transform.localPosition, CenterOfMass.transform.localPosition);
            sumOfDistances += wheelFR_distance;
            sumOfSuspension += wheelFR_data.GetCompressedDistance();
            //Debug.Log($"{sumOfSuspension}, FR: {wheelFR_data.GetCompressedDistance()}");
        }
        if (wheelBL_data.isGrounded)
        {
            //groundedWheels++;
            wheelBL_distance = Vector3.Distance(wheelBL_data.transform.localPosition, CenterOfMass.transform.localPosition);
            sumOfDistances += wheelBL_distance;
            sumOfSuspension += wheelBL_data.GetCompressedDistance();
            //Debug.Log($"{sumOfSuspension}, BL: {wheelBL_data.GetCompressedDistance()}");
        }
        if (wheelBR_data.isGrounded)
        {
            //groundedWheels++;
            wheelBR_distance = Vector3.Distance(wheelBR_data.transform.localPosition, CenterOfMass.transform.localPosition);
            sumOfDistances += wheelBR_distance;
            sumOfSuspension += wheelBR_data.GetCompressedDistance();
            //Debug.Log($"{sumOfSuspension}, BR: {wheelBR_data.GetCompressedDistance()}");
        }

        float inverseSumOfDistances = (sumOfDistances != 0 ? 1 / sumOfDistances : 0);
        float inverseSumOfSuspension = (sumOfSuspension != 0 ? (1 / sumOfSuspension) : 0);

        wheelFL_mass = wheelFL_data.isGrounded ? rb.mass * wheelFL_data.GetCompressedDistance() * inverseSumOfSuspension : 0;
        wheelFR_mass = wheelFR_data.isGrounded ? rb.mass * wheelFR_data.GetCompressedDistance() * inverseSumOfSuspension : 0;
        wheelBL_mass = wheelBL_data.isGrounded ? rb.mass * wheelBL_data.GetCompressedDistance() * inverseSumOfSuspension : 0;
        wheelBR_mass = wheelBR_data.isGrounded ? rb.mass * wheelBR_data.GetCompressedDistance() * inverseSumOfSuspension : 0;

        float weight = GetWeight().y * inverseSumOfDistances;
        float suspension = GetWeight().y * inverseSumOfSuspension;

        Vector3 fl = wheelFL_data.GetCurrentSpringForce(rb.GetPointVelocity(wheelFL.transform.position), wheelFL_mass, weight * wheelFL_distance);
        Vector3 fr = wheelFR_data.GetCurrentSpringForce(rb.GetPointVelocity(wheelFR.transform.position), wheelFR_mass, weight * wheelFR_distance);
        Vector3 bl = wheelBL_data.GetCurrentSpringForce(rb.GetPointVelocity(wheelBL.transform.position), wheelBL_mass, weight * wheelBL_distance);
        Vector3 br = wheelBR_data.GetCurrentSpringForce(rb.GetPointVelocity(wheelBR.transform.position), wheelBR_mass, weight * wheelBR_distance);

        
        //rb.AddForceAtPosition(fl, wheelFL.transform.position, ForceMode.Force);
        //rb.AddForceAtPosition(fr, wheelFR.transform.position, ForceMode.Force);
        //rb.AddForceAtPosition(bl, wheelBL.transform.position, ForceMode.Force);
        //rb.AddForceAtPosition(br, wheelBR.transform.position, ForceMode.Force);


        ApplyGravity();
    }



    protected void TurnCar(float input)
    {
       



        //transform.Rotate(0, wheelFL_data.GetTurnAngle() * Time.deltaTime * rb.velocity.magnitude, 0);   
    }

    protected void ApplyGasPedal(float GasPedal)
    {

        if(wheelBL_data.isGrounded)
        {
            rb.AddForceAtPosition(transform.forward * GasPedal * MotorForce, wheelBL.transform.position);
        }
        if (wheelBR_data.isGrounded)
        {
            rb.AddForceAtPosition(transform.forward * GasPedal * MotorForce, wheelBR.transform.position);
        }
    }

    protected void UpdateWheelTurnAngle(float ratio)
    {

        wheelFL_data.UpdateWheelAngle(ratio); //TODO:: Clean up with better steer angle communication
        wheelFR_data.UpdateWheelAngle(ratio);
    }

     void UpdateSprings()
    {
        wheelFL_data.InitializeShockAbsorber(SpringLength, SpringForce, SpringDamper, wheelMass);
        wheelFR_data.InitializeShockAbsorber(SpringLength, SpringForce, SpringDamper, wheelMass);
        wheelBR_data.InitializeShockAbsorber(SpringLength, SpringForce, SpringDamper, wheelMass);
        wheelBL_data.InitializeShockAbsorber(SpringLength, SpringForce, SpringDamper, wheelMass);
    }
    
    public float GetSteeringAngle()
    {
        return wheelFL_data.GetTurnAngle();
    }

}

