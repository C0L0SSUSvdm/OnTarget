using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINode : MonoBehaviour
{
    public GameObject NextNode;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Node: {this.gameObject.name}");
        if (other.gameObject.tag == "AI")
        {
            AICar car = other.gameObject.GetComponent<AICar>();
            if (car != null)
            {
                car.SetNextNode(NextNode);
            }
        }
    }
    
}
