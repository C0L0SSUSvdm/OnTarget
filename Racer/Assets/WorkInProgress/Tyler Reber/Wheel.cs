using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Wheel : MonoBehaviour
{
    [Header("----- Springs -----")]
    [SerializeField] int NumberOfCoils;
    [SerializeField] int NumberOfActiveCoils;
    
    [SerializeField] float MaximumSpringForce;
    [SerializeField] float springLength;
    [SerializeField] float springRestPosition;
    [SerializeField] float springTargetRestDistance;
    [SerializeField] float BobberForce;

    [SerializeField] float SpringStiffness = 1.0f;
    [SerializeField] float SpringDampening = 1.0f;

    [SerializeField] float CurrentWeightOnWheel;
    [SerializeField] float massWheelSupporting;

    //[SerializeField] Vector3 LastFramesVelocity;
    //[SerializeField] Vector3 DeltaVelocity;
    //[SerializeField] float DistanceFromGround;
    [SerializeField] float inverseWheelMass;

    [Header("----- Wheel Components -----")]
    //[SerializeField] MeshCollider WheelCollider;
    [SerializeField] CapsuleCollider RoadCollider;

    [SerializeField] float MaxTurnAngle = 0;
    [SerializeField] float CurrentTurnAngle = 0;
    [SerializeField] float WheelSize = 1;

    [Range(-2, 2), SerializeField] float GizmoSpringOffset = 0.5f;
    //[Range(0, 2.0f), SerializeField] float GizmoSpringLength = 1;
    [Range(0.01f, 0.2f), SerializeField] float GizmoSpringRadius = 0.2f;

    [Header("----- Runtime Parameters -----")]
    [SerializeField] public bool isGrounded;

    [SerializeField] LineRenderer line;


    private void OnDrawGizmos()
    {
        DrawGizmoWheel();
    }

    private void OnDrawGizmosSelected()
    {
        //DrawForwardArrow();
        if (transform.parent != null)
            DrawSpringHelix();
    }

    void DrawGizmoWheel()
    {
        Gizmos.color = Color.red;
        Vector3 startpos = transform.forward * WheelSize + transform.position;// transform.forward * 1.5f;
        for (int i = 0; i <= 360; i += 10)
        {
            Quaternion offset = Quaternion.Euler(i + transform.localEulerAngles.y, 0, 0);

            Matrix4x4 rotation = Matrix4x4.Rotate(offset) * Matrix4x4.identity;
            Matrix4x4 result = rotation * transform.worldToLocalMatrix;
            Vector3 nextpos = new Vector3(result.m20, result.m21, result.m22) * WheelSize + transform.position;
            Gizmos.DrawLine(startpos, nextpos);
            startpos = nextpos;
        }
    }

    void DrawSpringHelix()
    {
        //GizmoSpringLength = -transform.localPosition.y;
        Gizmos.color = Color.red;

        //float length = -GizmoSpringLength / 360;
        float length = transform.localPosition.y / 360;
        Vector3 start = transform.localPosition;
        start.x += GizmoSpringOffset;
        //start.y -= GizmoSpringLength;
        for (int i = 0; i <= 360; i += 5)
        {
            float theta = 2 * Mathf.PI * i;
            Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.Euler(0, theta, 0));

            Vector3 nextpoint = new Vector3(rotation.m20, rotation.m21, rotation.m22) * GizmoSpringRadius + start;
            nextpoint.y = length * (360 - i);
            //nextpoint.y = transform.localPosition.y * i;
            Gizmos.DrawLine(transform.parent.rotation * start + transform.parent.position, transform.parent.rotation * nextpoint + transform.parent.position);
            start = nextpoint;
        }
    }

    void DrawForwardArrow()
    {
        //Gizmos.DrawRay(transform.position + transform.forward * WheelSize * 2, transform.forward);      
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startColor = Color.blue;
        line.endColor = Color.blue;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + transform.forward * WheelSize * 2);

    }

    // Start is called before the first frame update
    void Start()
    {
        DrawForwardArrow();
        RoadCollider = GetComponent<CapsuleCollider>();


    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + transform.forward * WheelSize * 2);

        transform.localEulerAngles = new Vector3(0, CurrentTurnAngle, 0);

    }

    public void SetMaxTurnAngle(float angle)
    {
        MaxTurnAngle = angle;
    }

    public void UpdateWheelAngle(float power)
    {

        CurrentTurnAngle = MaxTurnAngle * power;
        //Debug.Log(CurrentTurnAngle);
    }

    public float GetTurnAngle()
    {
        return CurrentTurnAngle;
    }

    public void InitializeShockAbsorber(float length, float force, float damper, float wheelMass)
    {
        inverseWheelMass = 1 / wheelMass;
        MaximumSpringForce = force;
        springLength = -length;

        transform.localPosition = new Vector3(transform.localPosition.x, springLength, transform.localPosition.z);

        //transform.localPosition.Set(0, SpringRestPosition, 0);
        //GizmoSpringLength = length;
        SpringStiffness = force / length;
        SpringDampening = damper;
    }

    //https://www.youtube.com/watch?app=desktop&v=CdPYlj5uZeI&embeds_euri=https%3A%2F%2Fforum.unity.com%2F&feature=emb_logo
    /// <summary>
    /// Calculates how much counter force the spring is applying to the car. 
    /// </summary>


    public float SetSpringDistance(Vector3 WheelWorldVelocity, float massAtPoint)
    {
        float springDirection = Vector3.Dot(transform.up, WheelWorldVelocity);
        float deltaDistance = transform.localPosition.y - springLength;
        float force = (deltaDistance * SpringStiffness) - (springDirection * SpringDampening);

        return force;
    }

    public float GetCompressedDistance()
    {
        return springLength - transform.localPosition.y;
    }


    public Vector3 GetCurrentSpringForce(Vector3 WheelWorldVelocity, float massAtPoint, float weightOnWheel) //Distribute weight accross grounded tires
    {
        massWheelSupporting = massAtPoint;
        float inverseMassAtPoint = massAtPoint == 0 ? inverseWheelMass : 1 / massAtPoint;
        CurrentWeightOnWheel = weightOnWheel;


        Vector3 returnedForce = Vector3.zero;
        //When doing springs digitally, They are not inherently confined to the limitations of the materials in real life.
        // This means the spring can stretch and compress infinitely.
        // Deriving a distance from the springs force can yeild magnitudes farther than what the spring could do in real life.
        // To correct this, I need to store the current position in a new variable and add the delta distance to it.
        // Clamp the result with the spring's max and min values. Values that are too small or large will be handled

        //Step 1: Store the current position of the Spring
        float NewPhysicalWheelPosition = transform.localPosition.y;


        //Value a little larger than the Dot Product that accounts for amount of the car's mass the wheel is supporting.
        float equalibriumOffset = (WheelWorldVelocity.y / Time.deltaTime) * massAtPoint;
        //springRestPosition = Mathf.Lerp(springRestPosition, targetRestPosition, Time.deltaTime * NumberOfActiveCoils);


        //if (isGrounded)
        //{


        //    float delta = springRestPosition - springTargetRestDistance;
        //    BobberForce = (delta * -SpringStiffness) - Vector3.Dot(transform.up, WheelWorldVelocity) * SpringDampening;
        //    //BobberForce = Mathf.Lerp(BobberForce, 0, Time.deltaTime * 5);


        //    float bobberacceleration = BobberForce * inverseMassAtPoint;
        //    float bobberVelocity = (bobberacceleration * Time.deltaTime * Time.deltaTime * 0.5f);
        //    springTargetRestDistance -= bobberVelocity;
        //}
        //else
        //{
        //    springTargetRestDistance = 0;
        //}


        //Uncomment 2 lines these to restore
        springRestPosition = isGrounded ? springLength - (weightOnWheel / SpringStiffness) : springLength;
        springRestPosition = Mathf.Clamp(springRestPosition, springLength, 0);

        //Step 3: Use the new distance to calculate the theoretical force applied to the spring
        float deltaDistance = transform.localPosition.y - springRestPosition;// - springTargetRestDistance;

        //We want to include the wheel's movement on the y axis in the velocity and use that as the dampen force
        //float adjustedVelocity = ((deltaDistance / Time.deltaTime) * 0.5f) - WheelWorldVelocity.y; //This will give a more accurate Dampening force
        //float dotProduct = Vector3.Dot(transform.up, WheelWorldVelocity); //Different Formula used

        float ForceToRestPosition = (deltaDistance * -SpringStiffness) - (equalibriumOffset); //Method 1
        //Debug.Log($"Force to Rest: {ForceToRestPosition}, equalibrium Dot: {equalibriumOffset}");
        //Step 4: Calculate the acceleration of the spring and Find the Theoretical position of the spring
        float acceleration = ForceToRestPosition * inverseMassAtPoint;
        float newVelocity = (acceleration * Time.deltaTime * Time.deltaTime) * 0.5f;
        NewPhysicalWheelPosition += newVelocity;

        //springRestPosition = Mathf.Clamp(springRestPosition, springLength, 0);

        //Step 3: Use the new distance to calculate the theoretical force applied to the spring
        //float deltaDistance = transform.localPosition.y - springRestPosition;
        //float ForceToRestPosition = (deltaDistance * -SpringStiffness) - (equalibriumOffset); //Method 1
        //Step 4: Calculate the acceleration of the spring and Find the Theoretical position of the spring
        //float acceleration = ForceToRestPosition * inverseMassAtPoint;
        //float newVelocity = (acceleration * Time.deltaTime * Time.deltaTime) * 0.5f;
        //NewPhysicalWheelPosition += newVelocity;


        //Step 5: Define the spring's limits by clamping the new position
        if (NewPhysicalWheelPosition <= springLength) //Stretched to the Max
        {
            NewPhysicalWheelPosition = springLength;
        }
        else if (NewPhysicalWheelPosition >= 0)
        {
            NewPhysicalWheelPosition = 0;
            returnedForce.y = (NewPhysicalWheelPosition + springLength) * SpringStiffness;


        }

        //Step 6: Update the spring's position
        transform.localPosition = new Vector3(transform.localPosition.x, NewPhysicalWheelPosition, transform.localPosition.z);
        

        
        //Step 7: Use the remainder of the clamp to find out if any force wasn't nuetralized from the shock absorber

        //Convert remainder into distance



        return returnedForce; //TODO:: Adjust return force for odd trajectories
    }

    //Make Sure Exit Trigger occurs before the Stay Trigger
    //Incase multiple objects are collising at once.
    public void OnTriggerExit(Collider other)
    {
        isGrounded = false;
        //if (transform.localPosition.y > springLength)
        //{
        //    isGrounded = true;
        //}
    }

    public void OnTriggerStay(Collider other)
    {
        //Debug.Log("Collision Stay");
        isGrounded = true;



    }

}
////springRestPosition = Mathf.Clamp(springRestPosition, springLength, 0);

////Step 3: Use the new distance to calculate the theoretical force applied to the spring
//float deltaDistance = transform.localPosition.y - springRestPosition;

////We want to include the wheel's movement on the y axis in the velocity and use that as the dampen force
////float adjustedVelocity = ((deltaDistance / Time.deltaTime) * 0.5f) - WheelWorldVelocity.y; //This will give a more accurate Dampening force
////float dotProduct = Vector3.Dot(transform.up, WheelWorldVelocity); //Different Formula used

//float ForceToRestPosition = (deltaDistance * -SpringStiffness) - (equalibriumOffset); //Method 1
//                                                                                      //Debug.Log($"Force to Rest: {ForceToRestPosition}, equalibrium Dot: {equalibriumOffset}");
//                                                                                      //Step 4: Calculate the acceleration of the spring and Find the Theoretical position of the spring
//float acceleration = ForceToRestPosition * inverseMassAtPoint;
//float newVelocity = (acceleration * Time.deltaTime * Time.deltaTime) * 0.5f;
//NewPhysicalWheelPosition += newVelocity;