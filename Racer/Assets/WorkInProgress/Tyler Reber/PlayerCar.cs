using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCar : baseCar
{


    // Update is called once per frame
    private new void Update()
    {
        base.Update();

        //Vector3 accelerations = Vector3.zero;
        Vector3 accelerations = Input.GetAxis("Vertical") * transform.forward * ForwardForce;
        AddForcesToCar(accelerations);

        float test = Input.GetAxis("Horizontal");
        
        UpdateWheelTurnAngle(test);
        TurnCar(test);

    }

   
}
