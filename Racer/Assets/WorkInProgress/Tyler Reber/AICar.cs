using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AICar : baseVehicle
{   
    SphereCollider radarCollider;
    [SerializeField] float RunTimeSteeringLerpAngle = 0;

    [Range(5, 25)] float RadarDistance = 15.0f;

    public List<GameObject> FlockObjects;
    Vector3 averageForward;
    Vector3 AveragePosition;
    float AlignmentStrength = 0.5f;
    float CohesionStrength = 0.5f;
    float SeperationStrength = 0.5f;

    public new void Start()
    {
        base.Start();
        radarCollider = gameObject.GetComponent<SphereCollider>();
        radarCollider.radius = RadarDistance;
        radarCollider.isTrigger = true;
    }

    // Update is called once per frame
    public new void FixedUpdate()
    {
        base.FixedUpdate();
        CalculateAverages();

        Vector3 nodeDistance = nextNode.transform.position - transform.position;
        float targetDirection = Vector3.SignedAngle(nodeDistance, transform.forward, Vector3.up);


        float steerStrength = 0;

        if (Mathf.Abs(targetDirection) < maximumSteerAngle)
        {
            if (targetDirection > 2.0f)
            {

                steerStrength = targetDirection / maximumSteerAngle;
            }

        }
        else
        {
            steerStrength = 1;
        }
        steerStrength = targetDirection > 0 ? -steerStrength : steerStrength;

        //Debug.Log($"Ratio: {steerStrength}, Angle: {targetDirection}");
        RunTimeSteeringLerpAngle = Mathf.Lerp(RunTimeSteeringLerpAngle, steerStrength, Time.deltaTime * 5);
        UpdateSteeringAngle(RunTimeSteeringLerpAngle);
        ApplyGasPedal(1);
        FlockObjects.Clear();
    }

    void  CalculateAverages()
    {
        int vehicleCount = 0;
        averageForward = transform.forward;
        AveragePosition = transform.position; 
        foreach (GameObject flockObject in FlockObjects)
        {
            
            if(flockObject.tag == "Vehicle")
            {
                averageForward += flockObject.transform.forward;
                vehicleCount++;
            }
            AveragePosition += flockObject.transform.position;
        }
        averageForward /= vehicleCount;
        AveragePosition /= FlockObjects.Count;
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Triggered");
    }

    public void SetNextNode(GameObject _nextNode)
    {
        
        nextNode = _nextNode;
    }

    public void OnTriggerStay(Collider other)
    {
       
        FlockObject flockObject = other.gameObject.GetComponent<FlockObject>();
        if (flockObject != null)
        {           
            FlockObjects.Add(other.gameObject);
        }
    }
}
