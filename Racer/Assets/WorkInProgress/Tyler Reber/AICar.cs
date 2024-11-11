using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AICar : baseVehicle
{
    
    
    float RunTimeSteeringLerpAngle = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();

        Vector3 targetDirection = Vector3.Cross(transform.forward, nextNode.transform.position - transform.position);
        //Debug.Log($"Forward: {transform.forward}, Delta: {(nextNode.transform.position - transform.position).normalized}, Difference: {targetDirection}");
        float aiInput = targetDirection.y > 0 ? 1 : -1;
        RunTimeSteeringLerpAngle = Mathf.Lerp(RunTimeSteeringLerpAngle, aiInput, Time.deltaTime * 5);
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
