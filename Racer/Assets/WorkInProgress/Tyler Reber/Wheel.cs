using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Wheel : MonoBehaviour
{
    [Header("----- Springs -----")]
    //[SerializeField] float SpringLength = 1.2f;
    [SerializeField] float MaximumSpringForce;
    [SerializeField] float MaximumSpringLength;

    [SerializeField] float CurrentSpringLength;
    [SerializeField] float springDistance = 0.0f;
    [SerializeField] float SpringStiffness = 1.0f; 
    [SerializeField] float SpringDampening = 1.0f; 
    

    [Header("----- Wheel Components -----")]
    [SerializeField] MeshCollider WheelCollider;

    [SerializeField] float MaxTurnAngle = 0;
    [SerializeField] float CurrentTurnAngle = 0;
    [SerializeField] float WheelSize = 1;

    [Range(-2, 2), SerializeField] float GizmoSpringOffset = 0.5f;
    [Range(0, 2.0f), SerializeField] float GizmoSpringLength = 1;
    [Range(0.01f, 0.2f), SerializeField] float GizmoSpringRadius = 0.2f;

    [Header("----- Runtime Parameters -----")]
    [SerializeField] public bool isGrounded;

    [SerializeField] LineRenderer line;


    private void OnDrawGizmos()
    {
        DrawGizmoWheel();

        //Quaternion ff = transform.rotation * Quaternion.Euler(45, 0, 0);

        //Create a Rotation




        //Quaternion offsets = Quaternion.Euler(thetax, thetay, thetaz);
        //Matrix4x4 rotation = Matrix4x4.Rotate(offsets) * Matrix4x4.identity;
        //Matrix4x4 result = transform.worldToLocalMatrix * rotation;

        //Vector3 currentPos = transform.forward * WheelSize + transform.position;
        //Vector3 nextpos = new Vector3(result.m20, result.m21, result.m22) * WheelSize + transform.position;
        //Gizmos.DrawLine(currentPos, nextpos);
        //Gizmos.DrawRay(currentPos, (nextpos - currentPos));
    }

    private void OnDrawGizmosSelected()
    {
        //DrawForwardArrow();
        if(transform.parent != null)
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
        Gizmos.color = Color.blue;
        float length = -GizmoSpringLength / 360;
        Vector3 start = transform.localPosition;
        start.x += GizmoSpringOffset;
        //start.y -= GizmoSpringLength;
        for (int i = 0; i <= 360; i+=5)
        {
            float theta = 2 * Mathf.PI * i;
            Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.Euler(0, theta, 0));
                    
            Vector3 nextpoint = new Vector3(rotation.m20, rotation.m21, rotation.m22) * GizmoSpringRadius + start;
            nextpoint.y = length * (360 - i);

            Gizmos.DrawLine(transform.parent.rotation * start + transform.parent.position, transform.parent.rotation * nextpoint + transform.parent.position);
            start = nextpoint;
        }

        
    }

    void DrawDamperHelix()
    {
        Gizmos.color = Color.red;
        
        float length = -GizmoSpringLength / 360;
        Vector3 start = transform.localPosition;
        start.x += GizmoSpringOffset;
        //start.y -= GizmoSpringLength;
        for (int i = 0; i <= 360; i += 5)
        {
            float theta = 2 * Mathf.PI * i;
            Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.Euler(0, theta, 0));

            Vector3 nextpoint = new Vector3(rotation.m20, rotation.m21, rotation.m22) * GizmoSpringRadius + start;
            nextpoint.y = length * (360 - i);

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
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + transform.forward * WheelSize * 2);



        CollisionCheck();
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

    public void SetShockAbsorberLength(float length, float force)
    {
        MaximumSpringForce = force;
        MaximumSpringLength = length;
        //GizmoSpringLength = length;
        SpringStiffness = force / length;
        
    }

    public void CollisionCheck()
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, WheelSize + 0.05f))
        {           
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        
    }

    /// <summary>
    /// Calculates how much counter force the spring is applying to the car.
    /// </summary>
    public Vector3 ReturnProjectedSpringForce(Vector3 TireWorldVelocity)
    {
        float result = 0;
        RaycastHit hit;
        //Debug.Log(transform.position + (transform.up * MaximumSpringLength));
        //Project RayCast From Join Attached to the Car.
        //Debug.DrawRay(transform.position + (transform.up * MaximumSpringLength), -transform.up * (MaximumSpringLength + WheelSize + 0.05f), Color.red);
        if (Physics.Raycast(transform.position + (transform.up * MaximumSpringLength), -transform.up, out hit, WheelSize + MaximumSpringLength + 0.05f))
        {          
            //Debug.Log(hit.transform.gameObject);
            if (hit.transform.tag != "Player")
            {
                float mechanicalDistance = hit.distance - MaximumSpringLength;

                GizmoSpringLength = Mathf.Clamp(mechanicalDistance, 0, MaximumSpringLength);
                //Debug.Log($"Calculated: {mechanicalDistance}, GizmoLength: {GizmoSpringLength}");
                transform.localPosition = new Vector3(transform.localPosition.x, -GizmoSpringLength, transform.localPosition.z);
            }
          result = SpringStiffness * (MaximumSpringLength - GizmoSpringLength);
        }
        else
        {
            GizmoSpringLength = MaximumSpringLength;
            transform.localPosition = new Vector3(transform.localPosition.x, -GizmoSpringLength, transform.localPosition.z);
        }

        return new Vector3(0, result, 0);
    }

    public float SetSpringDistance(float force)
    {
        
        float forceReturned = 0;
        float length = MaximumSpringLength - Mathf.Abs(force) / SpringStiffness;
        
        if(length < 0)
        {

            GizmoSpringLength = 0;
            forceReturned = Mathf.Abs(length) * SpringStiffness;
        }
        else
        {
            GizmoSpringLength = length;
            forceReturned = GizmoSpringLength * 0.05f;
        }

        transform.localPosition = new Vector3(transform.localPosition.x, -GizmoSpringLength, transform.localPosition.z);

        return forceReturned;
    }

    public void GetSpring()
    {

    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Detected");
    }

}
