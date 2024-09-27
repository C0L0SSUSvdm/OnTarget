using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseCar : MonoBehaviour
{
    [Header("Car Parts")]
    [SerializeField] GameObject Chasis;

    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected float ForwardForce = 10.0f;

    [Tooltip("The Length of the Shock Absorber, Will likely be moved to item")]
    [Range(0, 2), SerializeField] float SpringLength = 1.2f;


    [SerializeField] protected Wheel wheelFL;
    [SerializeField] protected Wheel wheelFR;
    [SerializeField] protected Wheel wheelBL;
    [SerializeField] protected Wheel wheelBR;

    [Header("----- Environment Fields -----)")]
    [SerializeField] Vector3 windDirection;
    //[SerializeField] float windSpeed = 0.0f;

    [SerializeField] float airDensity = 1.838f; //Sea Level Air Density 1.225 kg/m^3
    [Tooltip("Drag Coefficients for a Car in Cartesian Cooridnates, (xyz) = (Sides, Top/bottom, front/back")]
    [SerializeField] Vector3 dragCoefficients = new Vector3(0.5f, 1, 0.3f); //Drag Coefficient of a Car
    [SerializeField] Vector3 CarAreas = new Vector3(3.0f, 5.0f, 2.2f); //Frontal Area of a Car

    void ApplyForces()
    {
        float y_force = rb.mass * Physics.gravity.y;
        Vector3 freefallResistance_y = CalculateResistance(Vector3.up, rb.velocity.y);
        Vector3 freefallResistance_x = CalculateResistance(Vector3.right, rb.velocity.x);
        Vector3 freefallResistance_z = CalculateResistance(Vector3.forward, rb.velocity.z);

        Debug.Log($"wind resistance: {new Vector3(freefallResistance_x.x, freefallResistance_y.y, freefallResistance_z.z)}, speed: {rb.velocity.y}");
   
        rb.AddForce(new Vector3(freefallResistance_y.x - freefallResistance_x.x, y_force + freefallResistance_y.y, freefallResistance_y.z - freefallResistance_z.z));
    }

    /// <summary>
    /// Pojects a wind direction onto the car car and calculates the area used from each side and returns the amount of force applied to the car in Cartesian Coordinates
    /// Formula used: 0.5f * airDensity * dragCoefficients * carAreaUsed * windSpeed * windSpeed
    /// </summary>
    /// <param name="NormalizedWindDirection"></param>
    /// <returns></returns>
    Vector3 CalculateResistance(Vector3 NormalizedWindDirection, float windSpeed)
    {
        //Project the Direction of the Wind on to the Car
        float front = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, transform.forward)) * CarAreas.z;
        float top = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, transform.up)) * CarAreas.y;
        float rightside = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, -transform.right)) * CarAreas.x;

        float backproject = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, -transform.forward)) * CarAreas.z;
        float bottomproject = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, -transform.up)) * CarAreas.y;
        float leftside = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, transform.right)) * CarAreas.x;

        float carAreaUsed = front + top + rightside + backproject + bottomproject + leftside;
        Vector3 ForceApplied = 0.5f * airDensity * dragCoefficients * carAreaUsed * windSpeed * windSpeed;

        return ForceApplied;
    }

    // Start is called before the first frame update
    void Start()
    {
       

        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 1000;
        rb.drag = 0;// 0.5f;
        rb.angularDrag = 0;// 0.5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        wheelFL.SetMaxTurnAngle(35);
        wheelFR.SetMaxTurnAngle(35);

        UpdateSprings();
        
    }

    // Update is called once per frame
    protected void Update()
    {
        //Chasis.transform.position = new Vector3(0, SpringLength, -3.0f);
        UpdateSprings();
        ApplyForces();
        
    }

    protected void TurnCar(float input)
    {
        
        transform.Rotate(0, wheelFL.GetTurnAngle() * Time.deltaTime * rb.velocity.magnitude, 0);   
    }

    protected void AddForcesToCar(Vector3 forces)
    {
        rb.AddForce(forces);
    }

    protected void UpdateWheelTurnAngle(float ratio)
    {
        wheelFL.UpdateWheelAngle(ratio);
        wheelFR.UpdateWheelAngle(ratio);
    }

     void UpdateSprings()
    {
        wheelFL.SetShockAbsorberLength(SpringLength);
        wheelFR.SetShockAbsorberLength(SpringLength);
        wheelBL.SetShockAbsorberLength(SpringLength);
        wheelBR.SetShockAbsorberLength(SpringLength);
    }
    
}
