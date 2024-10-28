using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GravityBody : MonoBehaviour
{
    [Header("----- Gravity Body Fields -----")]
    [SerializeField] protected Rigidbody rb;

    [SerializeField] float airDensity = 1.838f; //Sea Level Air Density 1.225 kg/m^3
    [Tooltip("Drag Coefficients for a Car in Cartesian Cooridnates, (xyz) = (Sides, Top/bottom, front/back")]

    [SerializeField] Vector3 BodyAreaCrossSection = new Vector3(3.0f, 5.0f, 2.2f); //Frontal Area of a Car
    [SerializeField] Vector3 ForwardCoefficients = new Vector3(0.5f, 1, 0.3f); //Drag Coefficient of a Car
    [SerializeField] Vector3 DragCoefficients = new Vector3(0.5f, 1, 0.3f); //Drag Coefficient of a Car
    [SerializeField] Vector3 SideCoefficients = new Vector3(0.8f, 1, 0.6f); //Drag Coefficient of a Car

    [SerializeField] protected Vector3 EnvironmentForces = Vector3.zero;
    [SerializeField] Vector3 freefallResistance_y = Vector3.zero;
    [SerializeField] Vector3 forwardAirResistance_z = Vector3.zero;
    [SerializeField] Vector3 dragAirResistance_z = Vector3.zero;
    [SerializeField] Vector3 AngularResistance_x = Vector3.zero;

    protected Vector3 CalculateGravity()
    {
        float y_force = rb.mass * Physics.gravity.y;

        if (rb.velocity.magnitude > 0.1f)
        {
            freefallResistance_y = CalculateResistance(Vector3.up, rb.velocity.y, ForwardCoefficients);

            //forwardAirResistance_z = CalculateResistance(transform.forward, rb.velocity.magnitude, ForwardCoefficients).z * -transform.forward;
            //dragAirResistance_z = CalculateResistance(transform.forward, rb.velocity.magnitude, DragCoefficients).z * -transform.forward;
            //AngularResistance_x = CalculateResistance(transform.right, rb.angularVelocity.magnitude, SideCoefficients).x * -transform.right;
        }
        else
        {
            freefallResistance_y = Vector3.zero;
            forwardAirResistance_z = Vector3.zero;
        }

        //TODO, side ways wind resistance
        EnvironmentForces = new Vector3(AngularResistance_x.x + forwardAirResistance_z.x + dragAirResistance_z.x,
            y_force + freefallResistance_y.y + forwardAirResistance_z.y + AngularResistance_x.y + dragAirResistance_z.y,
            forwardAirResistance_z.z + AngularResistance_x.z + dragAirResistance_z.z);
        //EnvironmentForces = new Vector3(forwardAirResistance_z.x + dragAirResistance_z.x,
        //freefallResistance_y.y + forwardAirResistance_z.y + dragAirResistance_z.y,
        //forwardAirResistance_z.z + dragAirResistance_z.z);
        //EnvironmentForces = new Vector3(0, freefallResistance_y.y, 0);
        return EnvironmentForces;
        //rb.AddForce(freefallResistance_x.x, y_force + freefallResistance_y.y, freefallResistance_z.z);
    }

    // Rigidbody is already applying interference forces from world objects
    // Only need to apply gravity because I'm not using unity's gravity physics
    protected void ApplyGravity()
    {
        rb.AddForce(EnvironmentForces);
    }


    /// <summary>
    /// Pojects a wind direction onto the car car and calculates the area used from each side and returns the amount of force applied to the car in Cartesian Coordinates
    /// Formula used: 0.5f * airDensity * dragCoefficients * carAreaUsed * windSpeed * windSpeed
    /// </summary>
    /// <param name="NormalizedWindDirection"></param>
    /// <returns></returns>
    Vector3 CalculateResistance(Vector3 NormalizedWindDirection, float windSpeed, Vector3 coefficient)
    {
        //Project the Direction of the Wind on to the Car
        float front = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, transform.forward)) * BodyAreaCrossSection.z;
        float top = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, transform.up)) * BodyAreaCrossSection.y;
        float rightside = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, -transform.right)) * BodyAreaCrossSection.x;

        float backproject = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, -transform.forward)) * BodyAreaCrossSection.z;
        float bottomproject = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, -transform.up)) * BodyAreaCrossSection.y;
        float leftside = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, transform.right)) * BodyAreaCrossSection.x;

        float carAreaUsed = front + top + rightside + backproject + bottomproject + leftside;
        Vector3 ForceApplied = 0.5f * airDensity * coefficient * carAreaUsed * windSpeed * windSpeed;

        return ForceApplied;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        CalculateGravity();
        ApplyGravity();

    }

    public Vector3 GetWeight()
    {
        return EnvironmentForces + (rb.velocity * rb.mass);
    }

    ////Find Forces from things acting on the car.
    private void OnCollisionStay(Collision collision)
    {



        //Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        //if(rb != null)
        //{
        //    collisionCounter++;
        //    Vector3 test = (rb.mass * rb.velocity) + (rb.mass * Physics.gravity);            
        //    ActingForces += test;
        //    if(ActingForces.y < -80000)
        //    {
        //        Debug.Log($"obj: {collision.gameObject.name}, mass: {rb.mass}, velocity: {rb.velocity}, this collision: {test}, total collistion: {ActingForces}");
        //    }
        //    else
        //    {
        //        Debug.Log($"velocity: {rb.velocity}, this collision: {test}, total collistion: {ActingForces}");
        //    }
        //}


    }

}
