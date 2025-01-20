using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD_Speedometer_Default : MonoBehaviour, HUD_Speedometer_Director
{
    [SerializeField] GameObject needle;
    [SerializeField] public TextMeshProUGUI speedometerText;
    
    //12 degree is zero, subtract 12
    public void UpdateSpeedometer(float speed)
    {
        //164 from 0 to max
        float convertedspeed = (speed * 0.625f);
        float spedometerangle = (speed / 160.0f) * 164;


        Quaternion roation = Quaternion.Euler(0, 0, -(spedometerangle - 10.5f));
        needle.transform.rotation = roation;
        


        speedometerText.text = speed.ToString("F0");
    }
}
