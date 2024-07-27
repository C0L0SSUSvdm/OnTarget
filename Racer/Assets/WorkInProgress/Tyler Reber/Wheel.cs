using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        for(int i = 0; i < 360; i += 10) {  
            float rad = i * Mathf.Deg2Rad;                   
            Gizmos.DrawRay(transform.position, new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * 1.5f);
        }

        Gizmos.DrawRay(transform.forward * 1.5f, transform.position);
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            transform.Rotate(Vector3.right * 10);
        }
    }
}
