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
    [Range(90, 150), SerializeField] float spatialAwarenessAngle = 130;
    [Range(20, 100), SerializeField] float perifialVisionAngle = 100;

    public List<FlockObject> otherFlockObjects;
    Vector3 averageForward;
    Vector3 averagePosition;
    Vector3 trackAveragePosition;


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

        float cohesion = 0;
        Vector3 seperation = Vector3.zero;

        foreach (FlockObject flockObject in otherFlockObjects)
        {
            cohesion += CalculateCohesion(flockObject);
            seperation += CalculateSwidewaysSeperation(flockObject);
        }



        //TestLogicPosition = seperation;

        Vector3 averageOffset = (transform.position - averagePosition);
        float angle = Vector3.SignedAngle(transform.forward, (trackAveragePosition + averageOffset - seperation) - transform.position, Vector3.up);

        float adjustedAngle = angle;// + (cohesion * 0.5f) + (seperation * 0.5f);// + seperation;// + cohesion;
        
        RunTimeSteeringLerpAngle = Mathf.Lerp(RunTimeSteeringLerpAngle, adjustedAngle, Time.deltaTime * 2);
        float turnInput = Mathf.Clamp(adjustedAngle, -maximumSteerAngle, maximumSteerAngle) / maximumSteerAngle;
        RunTimeSteeringLerpAngle = turnInput;


        UpdateSteeringAngle(turnInput);
        ApplyGasPedal(1);
        otherFlockObjects.Clear();

        if (lineRenderer != null)
        {
            UpdateFieldOfView();
        }
    }

    void CalculateAcceleration()
    {

    }

    float CalculateCohesion(FlockObject flockobject)
    {

        float result = 0;
        Vector3 vector = averagePosition - flockobject.transform.position;
        float distance = vector.magnitude;
        float flockObjectAngle = IsInFieldOfView(vector.normalized);
        if(Mathf.Abs(flockObjectAngle) < 25 && distance < 12)
        {
            result = 2 * Mathf.Sign(flockObjectAngle);
        }

        return result;
    }

    Vector3 CalculateSwidewaysSeperation(FlockObject flockObject)
    {
        Vector3 signedoffset = Vector3.zero;

        float signedAngle = 0;
        Vector3 direction = flockObject.transform.position - transform.position;
        float totalSafeDistance = myFlockObject.GetSafeRadius() + flockObject.GetSafeRadius();


        if (direction.magnitude < totalSafeDistance)
        {
            signedAngle = IsInFieldOfView(direction.normalized);
            float angle = Mathf.Abs(signedAngle);
            if (angle >= 45 && angle <= spatialAwarenessAngle)
            {

                if(flockObject.tag == "TrackNode")
                {
                    //TODO: Implement TrackNode avoidance
                    float angleDifference = Vector3.SignedAngle(transform.forward, direction.normalized, Vector3.up);
                    //signedAngle = Mathf.Clamp(angleDifference, 0, Mathf.Abs(angleDifference)) * -Mathf.Sign(angleDifference);
                    //Debug.Log(signedAngle);
                }
                else
                {
                    float ratio = (totalSafeDistance - direction.magnitude) / totalSafeDistance;
                    float angleDifference = Vector3.SignedAngle(transform.forward, flockObject.transform.forward, Vector3.up);
                    Vector3 directinoalDifference = transform.forward - flockObject.transform.forward;
                    signedoffset = directinoalDifference * myFlockObject.GetSafeRadius() * ratio * Mathf.Sign(angleDifference);
                }

            }

        }

        return signedoffset;
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

    void CalculateAverages()
    {
        int vehicleCount = 1;
        int raceTrackNodeCount = 0;
        averageForward = transform.forward;
        averagePosition = transform.position;
        trackAveragePosition = Vector3.zero;
        foreach (FlockObject flockObject in otherFlockObjects)
        {

            if (flockObject.tag == "TrackNode")
            {
                trackAveragePosition += flockObject.GetTrackCenterPoint();
                raceTrackNodeCount++;
            }
            else
            {
                float safeDistance = (myFlockObject.GetSafeRadius() + flockObject.GetSafeRadius()) * 2;
                Vector3 direction = flockObject.transform.position - transform.position;
                if (direction.magnitude < safeDistance)
                {
                    averagePosition += flockObject.transform.position;
                    averageForward += flockObject.transform.forward;
                    vehicleCount++;
                }

            }

        }
        averageForward /= vehicleCount;
        averagePosition /= vehicleCount;
        if (raceTrackNodeCount > 0)
        {
            trackAveragePosition = ((trackAveragePosition / raceTrackNodeCount) + transform.position) * 0.5f;
        }
        else
        {
            Debug.Log("Fix 0 Node Case");
        }



    }
    private void OnDrawGizmosSelected()
    {
        DrawLocationSphere();
    }

    void DrawLocationSphere()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(trackAveragePosition, 1.0f);


        Vector3 offset = transform.position - averagePosition;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(trackAveragePosition + offset, 1.0f);


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(TestLogicPosition + averagePosition, 2.0f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + TestLogicPosition * 5);

    }

    public void OnTriggerStay(Collider other)
    {

        FlockObject flockObject = other.gameObject.GetComponent<FlockObject>();
        if (flockObject != null)
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            if(other.tag == "TrackNode")
            {
                if(IsInFieldOfView(direction) <= perifialVisionAngle)
                otherFlockObjects.Add(flockObject);
            }
            else
            {
                if (IsInFieldOfView(direction) <= spatialAwarenessAngle)
                {
                    otherFlockObjects.Add(flockObject);
                }
            }




        }
    }

    float IsInFieldOfView(Vector3 direction)
    {
        float angle = Vector3.Angle(transform.forward, direction);
        return angle;
    }
}
