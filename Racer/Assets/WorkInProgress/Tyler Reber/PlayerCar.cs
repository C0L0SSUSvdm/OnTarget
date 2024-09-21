using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCar : baseVehicle
{

    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();

        float turnInput = Input.GetAxis("Horizontal");
        UpdateSteeringAngle(turnInput);

        float forwardInput = Input.GetAxis("Vertical");
        ApplyGasPedal(forwardInput);

        ApplySteerForce();
    }

   
}
