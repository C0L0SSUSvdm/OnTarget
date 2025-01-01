using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

public class FlockObject : MonoBehaviour
{
    [SerializeField] FlockObject AdjacentFlockObj = null;
    Vector3 TrackCenterPosition = Vector3.zero;
    [SerializeField] float SafeRadius;
    [Range(-1, 1), SerializeField] float AggressionFactor;
    Vector3 GetPosition { get; }

    private void Start()
    {
        if(transform.tag == "TrackNode")
        {
            Assert.IsTrue(AdjacentFlockObj != null, "Adjacent Flock Object is Null");
            if(AdjacentFlockObj == null)
            {
                TrackCenterPosition = transform.position;
            }
            else
            {
                TrackCenterPosition = (transform.position + AdjacentFlockObj.transform.position) * 0.5f;
            }
            
        }
        else
        {
            
        }
        

        
    }

    public Vector3 GetTrackCenterPoint()
    {
        
        return TrackCenterPosition;
    }

    public Vector3 GetRotation()
    {
        return transform.forward;
    }
    public float GetSafeRadius()
    {
        return SafeRadius;
    }

    public float GetAggressionFactor()
    {
        return AggressionFactor;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (transform.tag == "TrackNode")
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 1.0f);
        }
        else
        {
            Vector3 offset = Vector3.up * 4.0f;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + offset, 1.0f);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(TrackCenterPosition, 1.0f);
    }
#endif

}
