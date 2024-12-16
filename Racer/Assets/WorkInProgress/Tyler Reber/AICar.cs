using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class AICar : baseVehicle
{   
    SphereCollider radarCollider;
    [SerializeField] LineRenderer lineRenderer;


    [SerializeField] float RunTimeSteeringLerpAngle = 0;

    [Range(20, 150), SerializeField] float RadarDistance;

    
    public List<FlockObject> otherFlockObjects;
    [SerializeField] Vector3 averageForward;
    [SerializeField] Vector3 averagePosition;
    [SerializeField] Vector3 trackAveragePosition;
    float AlignmentStrength = 5f;
    float CohesionStrength = 5f;
    float SeperationStrength = 50f;

    public Vector3 TestLogicPosition;

    public new void Start()
    {
        base.Start();
        radarCollider = gameObject.GetComponent<SphereCollider>();
        radarCollider.radius = RadarDistance;
        radarCollider.isTrigger = true;

        if (lineRenderer != null)
            InitializeFieldOfView();
    }

    // Update is called once per frame
    public new void FixedUpdate()
    {
        base.FixedUpdate();
        CalculateAverages();

        Vector3 seperation = CalculateSeperation();
        TestLogicPosition = seperation;

        float angle = Vector3.SignedAngle(transform.forward,trackAveragePosition - transform.position, Vector3.up);
        RunTimeSteeringLerpAngle = Mathf.Lerp(RunTimeSteeringLerpAngle, angle, Time.deltaTime * 2);
        float turnInput = Mathf.Clamp(angle, -maximumSteerAngle, maximumSteerAngle) / maximumSteerAngle;
        RunTimeSteeringLerpAngle = turnInput;
        

        UpdateSteeringAngle(turnInput);
        ApplyGasPedal(1);
        otherFlockObjects.Clear();

        if(lineRenderer != null)
        {
            UpdateFieldOfView();
        }
    }

    void CalculateAcceleration() {

    }

    Vector3 CalculateSeperation()
    {
        Vector3 sum = Vector3.zero;

        foreach(FlockObject flockObject in otherFlockObjects)
        {
            Vector3 direction = flockObject.transform.position - transform.position;
            float totalSafeDistance = myFlockObject.GetSafeRadius() + flockObject.GetSafeRadius();
            //Debug.Log($"distance: {direction.magnitude}, safe distance: {totalSafeDistance}");
            if(direction.magnitude < totalSafeDistance)
            {
                //Debug.Log("In Safe Distance");
                if(IsInFieldOfView(direction.normalized))
                { 
                    //Debug.Log("In Field of View");
                    float ratio = (totalSafeDistance - direction.magnitude) / totalSafeDistance;
                    
                    sum += (direction.normalized * ratio);
                }
                
            }
        }
        return sum * SeperationStrength;
    }

    void InitializeFieldOfView()
    {
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.positionCount = 12;
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;

        UpdateFieldOfView();

    }

    private void UpdateFieldOfView()
    {
        
        lineRenderer.SetPosition(0, transform.position);
        for (int i = 1, j = -100; i != lineRenderer.positionCount; i++, j += 20)
        {
            Vector3 rotation = Quaternion.Euler(0, j, 0) * (transform.forward * RadarDistance);
            lineRenderer.SetPosition(i, rotation + transform.position);
        }

    }

    void  CalculateAverages()
    {
        int vehicleCount = 1;
        int raceTrackNodeCount = 0;
        averageForward = transform.forward;
        averagePosition = transform.position;
        trackAveragePosition = Vector3.zero;
        foreach (FlockObject flockObject in otherFlockObjects)
        {
            
            if(flockObject.tag == "TrackNode")
            {
                trackAveragePosition += flockObject.GetTrackCenterPoint();
                raceTrackNodeCount++;
            }
            else
            {
                averagePosition += flockObject.transform.position;
                averageForward += flockObject.transform.forward;
                vehicleCount++;
            }
            
        }
        averageForward /= vehicleCount;
        averagePosition /= vehicleCount;
        if(raceTrackNodeCount > 0)
        {
            trackAveragePosition = ((trackAveragePosition / raceTrackNodeCount) + transform.position) * 0.5f;
        }
        else
        {
            Debug.Log("Fix 0 Node Case");
        }
        

        
    }

    private void OnDrawGizmos()
    {
        DrawLocationSphere();
    }

    void DrawLocationSphere()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(trackAveragePosition, 1.0f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(averagePosition, 1.0f);


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(TestLogicPosition + averagePosition, 2.0f);

    }

    public void OnTriggerStay(Collider other)
    {
       
        FlockObject flockObject = other.gameObject.GetComponent<FlockObject>();
        if (flockObject != null)
        {   
            Vector3 direction = (other.transform.position - transform.position).normalized;
            if (IsInFieldOfView(direction))
            {
                otherFlockObjects.Add(flockObject);
            }
            
        }
    }

    bool IsInFieldOfView(Vector3 direction)
    {
        float angle = Vector3.Angle(transform.forward, direction);
        return angle <= 100;
    }
}
