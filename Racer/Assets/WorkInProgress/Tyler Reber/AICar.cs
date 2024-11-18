using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AICar : baseVehicle
{   
    SphereCollider radarCollider;
    float RunTimeSteeringLerpAngle = 0;

    [Range(5, 25)] float RadarDistance = 15.0f;

    public void Start()
    {
        base.Start();
        radarCollider = gameObject.GetComponent<SphereCollider>();
        radarCollider.radius = RadarDistance;
        radarCollider.isTrigger = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();

        Vector3 nodeDistance = nextNode.transform.position - transform.position;
        float targetDirection = Vector3.SignedAngle(nodeDistance, transform.forward, Vector3.up);
        
        
        float steerStrength = 0;

        if (Mathf.Abs(targetDirection) < maximumSteerAngle)
        {
            if(targetDirection > 2.0f) 
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
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Triggered");
    }

    public void SetNextNode(GameObject _nextNode)
    {
        
        nextNode = _nextNode;
    }
}
