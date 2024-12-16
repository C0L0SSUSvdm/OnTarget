#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINode : MonoBehaviour
{
    public GameObject NextNode;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1.0f);
    }


    //public void OnTriggerEnter(Collider other)
    //{
       
    //    if (other.gameObject.tag == "AI")
    //    {
    //        AICar car = other.gameObject.GetComponent<AICar>();
    //        if (car != null)
    //        {
    //            car.SetNextNode(NextNode);
    //        }
    //    }
    //}
    
}
#endif