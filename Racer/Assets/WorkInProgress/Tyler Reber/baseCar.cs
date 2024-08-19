using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class baseCar : GravityBody
{
    [Header("----- Temporary Fields -----")]
    [Range(1, 100000), SerializeField] float SpringForce;
    [Tooltip("The Length of the Shock Absorber, Will likely be moved to item")]
    [Range(0, 2), SerializeField] float SpringLength = 1.2f;
    [SerializeField] float SteerAngle;
    [SerializeField] protected float ForwardForce = 10.0f;


    [Header("Car Parts")]
    [SerializeField] GameObject steeringColumn;
    [SerializeField] GameObject Chasis;
    [SerializeField] GameObject wheelFL;
    [SerializeField] GameObject wheelFR;
    [SerializeField] GameObject wheelBL;
    [SerializeField] GameObject wheelBR;

    Wheel wheelFL_data;
    Wheel wheelFR_data;
    Wheel wheelBL_data;
    Wheel wheelBR_data;

    [Header("----- Environment Fields -----)")]
    [SerializeField] Vector3 windDirection;

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

        wheelFL_data.SetMaxTurnAngle(35);
        wheelFR_data.SetMaxTurnAngle(35);

        UpdateSprings();
        
    }


    // Update is called once per frame
    public new void Update()
    {
        //base.Update();
        CalculateGravity();


        //Chasis.transform.position = new Vector3(0, SpringLength, -3.0f);
        //UpdateSprings();

        //Upward Force Produced by the Springs at start
        Vector3 FL = wheelFL_data.ReturnProjectedSpringForce(rb.GetPointVelocity(wheelFL.transform.position));
        Vector3 FR = wheelFR_data.ReturnProjectedSpringForce(rb.GetPointVelocity(wheelFR.transform.position));
        Vector3 BL = wheelBL_data.ReturnProjectedSpringForce(rb.GetPointVelocity(wheelBL.transform.position));
        Vector3 BR = wheelBR_data.ReturnProjectedSpringForce(rb.GetPointVelocity(wheelBR.transform.position));







        //rb.AddForceAtPosition(wheelFL_data.ReturnProjectedSpringForce(), wheelFL.transform.position, ForceMode.Force); //TODO Dot product return vector
        //rb.AddForceAtPosition(wheelFR_data.ReturnProjectedSpringForce(), wheelFR.transform.position, ForceMode.Force);
        //rb.AddForceAtPosition(wheelBL_data.ReturnProjectedSpringForce(), wheelBL.transform.position, ForceMode.Force);
        //rb.AddForceAtPosition(wheelBR_data.ReturnProjectedSpringForce(), wheelBR.transform.position, ForceMode.Force);


        //int groundedWheels = 0;
        //if (wheelFL_data.isGrounded)
        //{
        //    groundedWheels++;     
        //}
        //if (wheelFR_data.isGrounded)
        //{
        //    groundedWheels++;          
        //}
        //if (wheelBL_data.isGrounded)
        //{
        //    groundedWheels++;            
        //}
        //if (wheelBR_data.isGrounded)
        //{
        //    groundedWheels++;           
        //}

        //if (wheelFL_data.isGrounded)
        //{
        //    float test = wheelFL_data.SetSpringDistance(WorldForces.y / groundedWheels);
        //    rb.AddForceAtPosition(new Vector3(0, test, 0), wheelFL.transform.position, ForceMode.Force);
        //}

        //if (wheelFR_data.isGrounded)
        //{
        //    float test = wheelFR_data.SetSpringDistance(WorldForces.y / groundedWheels);
        //    rb.AddForceAtPosition(new Vector3(0, test, 0), wheelFR.transform.position, ForceMode.Force);
        //}
        //if (wheelBL_data.isGrounded)
        //{
        //    float test = wheelBL_data.SetSpringDistance(WorldForces.y / groundedWheels);
        //    rb.AddForceAtPosition(new Vector3(0, test, 0), wheelBL.transform.position, ForceMode.Force);
        //}
        //if (wheelBR_data.isGrounded)
        //{
        //    float test = wheelBR_data.SetSpringDistance(WorldForces.y / groundedWheels);
        //    rb.AddForceAtPosition(new Vector3(0, test, 0), wheelBR.transform.position, ForceMode.Force);
        //}


        ApplyGravity();
    }

    protected void TurnCar(float input)
    {
        
        transform.Rotate(0, wheelFL_data.GetTurnAngle() * Time.deltaTime * rb.velocity.magnitude, 0);   
    }

    protected void AddForcesToCar(Vector3 forces)
    {
        //Debug.Log(forces);
        rb.AddForce(forces);
    }

    protected void UpdateWheelTurnAngle(float ratio)
    {

        wheelFL_data.UpdateWheelAngle(ratio); //TODO:: Clean up with better steer angle communication
        wheelFR_data.UpdateWheelAngle(ratio);
    }

     void UpdateSprings()
    {
        wheelFL_data.SetShockAbsorberLength(SpringLength, SpringForce);
        wheelFR_data.SetShockAbsorberLength(SpringLength, SpringForce);
        wheelBL_data.SetShockAbsorberLength(SpringLength, SpringForce);
        wheelBR_data.SetShockAbsorberLength(SpringLength, SpringForce);
    }
    
    public float GetSteeringAngle()
    {
        return wheelFL_data.GetTurnAngle();
    }
}


//[SerializeField] float windSpeed = 0.0f;

//[SerializeField] float airDensity = 1.838f; //Sea Level Air Density 1.225 kg/m^3
//[Tooltip("Drag Coefficients for a Car in Cartesian Cooridnates, (xyz) = (Sides, Top/bottom, front/back")]
//[SerializeField] Vector3 dragCoefficients = new Vector3(0.5f, 1, 0.3f); //Drag Coefficient of a Car
//[SerializeField] Vector3 CarAreas = new Vector3(3.0f, 5.0f, 2.2f); //Frontal Area of a Car

//void ApplyForces()
//{
//    float y_force = rb.mass * Physics.gravity.y;
//    Vector3 freefallResistance_y = CalculateResistance(Vector3.up, rb.velocity.y);
//    Vector3 freefallResistance_x = CalculateResistance(Vector3.right, rb.velocity.x);
//    Vector3 freefallResistance_z = CalculateResistance(Vector3.forward, rb.velocity.z);

//    float sidewayMomentum = rb.mass * (Vector3.Dot(rb.velocity, transform.right) * Time.deltaTime);
//    Debug.Log(sidewayMomentum);

//    Vector3 test = new Vector3(freefallResistance_x.x, y_force + freefallResistance_y.y, freefallResistance_z.z);
//    Debug.Log(test.x);

//    //Debug.Log($"wind resistance: {new Vector3(freefallResistance_x.x, freefallResistance_y.y, freefallResistance_z.z)}, {freefallResistance_z},  speed: {rb.velocity.y}");

//    rb.AddForce(test);
//}

///// <summary>
///// Pojects a wind direction onto the car car and calculates the area used from each side and returns the amount of force applied to the car in Cartesian Coordinates
///// Formula used: 0.5f * airDensity * dragCoefficients * carAreaUsed * windSpeed * windSpeed
///// </summary>
///// <param name="NormalizedWindDirection"></param>
///// <returns></returns>
//Vector3 CalculateResistance(Vector3 NormalizedWindDirection, float windSpeed)
//{
//    //Project the Direction of the Wind on to the Car
//    float front = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, transform.forward)) * CarAreas.z;
//    float top = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, transform.up)) * CarAreas.y;
//    float rightside = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, -transform.right)) * CarAreas.x;

//    float backproject = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, -transform.forward)) * CarAreas.z;
//    float bottomproject = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, -transform.up)) * CarAreas.y;
//    float leftside = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, transform.right)) * CarAreas.x;

//    float carAreaUsed = front + top + rightside + backproject + bottomproject + leftside;
//    Vector3 ForceApplied = 0.5f * airDensity * dragCoefficients * carAreaUsed * windSpeed * windSpeed;

//    return ForceApplied;
//}

// Start is called before the first frame update