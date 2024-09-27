using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour
{
    [Header("----- Gravity Body Fields -----")]
    [SerializeField] protected Rigidbody rb;

    [SerializeField] float airDensity = 1.838f; //Sea Level Air Density 1.225 kg/m^3
    [Tooltip("Drag Coefficients for a Car in Cartesian Cooridnates, (xyz) = (Sides, Top/bottom, front/back")]
    [SerializeField] Vector3 dragCoefficients = new Vector3(0.5f, 1, 0.3f); //Drag Coefficient of a Car
    [SerializeField] Vector3 BodyAreas = new Vector3(3.0f, 5.0f, 2.2f); //Frontal Area of a Car

    [SerializeField] protected Vector3 EnvironmentForces = Vector3.zero;
    [SerializeField] Vector3 freefallResistance_y = Vector3.zero;
    [SerializeField] Vector3 freefallResistance_x = Vector3.zero;
    [SerializeField] Vector3 freefallResistance_z = Vector3.zero;

    protected Vector3 CalculateGravity()
    {
        float y_force = rb.mass * Physics.gravity.y;

        if (rb.velocity.magnitude > 0.1f)
        {
            freefallResistance_y = CalculateResistance(Vector3.up, rb.velocity.y);
            freefallResistance_x = CalculateResistance(transform.right, rb.velocity.magnitude);//I think I can use Angular Velocity for this
            freefallResistance_z = CalculateResistance(transform.forward, rb.velocity.magnitude);

            float sidewayMomentum = rb.mass * (Vector3.Dot(rb.velocity, transform.right) * Time.deltaTime);
            //Debug.Log(sidewayMomentum);

            //Vector3 test = new Vector3(freefallResistance_x.x, y_force + freefallResistance_y.y, freefallResistance_z.z);
            //Debug.Log(test.x);

            //Debug.Log($"wind resistance: {new Vector3(freefallResistance_x.x, freefallResistance_y.y, freefallResistance_z.z)}, {freefallResistance_z},  speed: {rb.velocity.y}");

            
        }
        else
        {
            freefallResistance_y = Vector3.zero;
            freefallResistance_x = Vector3.zero;
            freefallResistance_z = Vector3.zero;
        }

        //TODO, Figure out forward and side ways wind resistance
        EnvironmentForces = new Vector3(0, y_force + freefallResistance_y.y, -freefallResistance_z.z);
        //EnvironmentForces = new Vector3(0, y_force + freefallResistance_y.y, 0);
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
    Vector3 CalculateResistance(Vector3 NormalizedWindDirection, float windSpeed)
    {
        //Project the Direction of the Wind on to the Car
        float front = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, transform.forward)) * BodyAreas.z;
        float top = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, transform.up)) * BodyAreas.y;
        float rightside = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, -transform.right)) * BodyAreas.x;

        float backproject = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, -transform.forward)) * BodyAreas.z;
        float bottomproject = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, -transform.up)) * BodyAreas.y;
        float leftside = Mathf.Max(0, Vector3.Dot(NormalizedWindDirection, transform.right)) * BodyAreas.x;

        float carAreaUsed = front + top + rightside + backproject + bottomproject + leftside;
        Vector3 ForceApplied = 0.5f * airDensity * dragCoefficients * carAreaUsed * windSpeed * windSpeed;

        return ForceApplied;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
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
