using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD_RPMGuage_Default : MonoBehaviour, HUD_RPMGuage_Director
{
    //[SerializeField] public TextMeshProUGUI speedometerText;

    //public void UpdateRPMGuage(int value)
    //{
    //    speedometerText.text = value.ToString();
    //}

    [SerializeField] GameObject needle;
    [SerializeField] public TextMeshProUGUI speedometerText;

    //12 degree is zero, subtract 12
    public void UpdateRPMGuage(int rpm)
    {
        //164 from 0 to max
        float convertedspeed = (rpm * 0.625f);
        float spedometerangle = (rpm / 160.0f) * 164;


        Quaternion roation = Quaternion.Euler(0, 0, -(spedometerangle - 10.5f));
        needle.transform.rotation = roation;



        speedometerText.text = rpm.ToString("F0");
    }
}
