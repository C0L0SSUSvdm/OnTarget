using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AICar : baseVehicle
{
    
    [SerializeField] GameObject nextNode;


    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();

        Vector3 targetDirection = Vector3.Cross(transform.forward, nextNode.transform.position - transform.position);
        //Debug.Log($"Forward: {transform.forward}, Delta: {(nextNode.transform.position - transform.position).normalized}, Difference: {targetDirection}");
        if (targetDirection.y > 0)
        {
            UpdateSteeringAngle(1);
        }
        else
        {
            UpdateSteeringAngle(-1);
        }

        ApplyGasPedal(1);
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Triggered");
    }

    public void SetNextNode(GameObject _nextNode)
    {
        Debug.Log($"Setting Node to {_nextNode}");
        nextNode = _nextNode;
    }
}
