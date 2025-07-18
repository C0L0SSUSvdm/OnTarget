using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD_RPMGuage_Default : MonoBehaviour, HUD_RPMGuage_Director
{

    [SerializeField] GameObject needle;
    [SerializeField] public TextMeshProUGUI speedometerText;

    //12 degree is zero, subtract 12
    public void UpdateRPMGuage(int rpm)
    {
        //164 from 0 to max
        //float convertedspeed = (rpm * 0.625f);
        float spedometerangle = (rpm / 1000.0f) * 15.3f; // 46 euler = 3000 rpm


        Quaternion rotation = Quaternion.Euler(0, 0, -(spedometerangle - 30.0f));
        needle.transform.rotation = rotation;

        speedometerText.text = rpm.ToString("F0");
        
    }
}
