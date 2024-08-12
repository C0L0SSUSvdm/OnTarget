using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Wheel : MonoBehaviour
{
    [Header("----- Wheel Components -----")]
    [SerializeField] MeshCollider WheelCollider;

    [SerializeField] float MaxTurnAngle = 0;
    [SerializeField] float CurrentTurnAngle = 0;
    [SerializeField] float WheelSize = 1;

    [Range(-2, 2), SerializeField] float GizmoSpringOffset = 0.5f;
    [Range(0, 2.0f), SerializeField] float GizmoSpringLength;
    [Range(0.01f, 0.2f), SerializeField] float GizmoSpringRadius = 0.2f;

    //[SerializeField] float thetax = 0;
    //[SerializeField] float thetay = 0;
    //[SerializeField] float thetaz = 0;
    //[SerializeField] Vector3 forward;


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
        DrawForwardArrow();
        if(transform.parent != null)
            DrawHelix();
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

    void DrawHelix()
    {
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

        transform.localPosition = new Vector3(transform.localPosition.x, -GizmoSpringLength, transform.localPosition.z);
    }

    void DrawForwardArrow()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position + transform.forward * WheelSize * 2, transform.forward);
        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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

    public void SetShockAbsorberLength(float length)
    {
        GizmoSpringLength = length;
    }

}
