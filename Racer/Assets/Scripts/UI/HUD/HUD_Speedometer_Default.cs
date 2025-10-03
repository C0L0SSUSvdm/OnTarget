using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD_Speedometer_Default : MonoBehaviour, HUD_Speedometer_Director
{
    [SerializeField] GameObject needle;
    [SerializeField] public TextMeshProUGUI speedometerText;
    
    [Header("Speedometer Settings")]
    [SerializeField] private float maxSpeed = 160f;
    [SerializeField] private float maxAngle = 164f;
    [SerializeField] private float zeroOffset = 10.5f;

    public void UpdateSpeedometer(float speed)
    {
        float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);
        float angle = normalizedSpeed * maxAngle;

        Quaternion rotation = Quaternion.Euler(0, 0, -(angle - zeroOffset));
        needle.transform.rotation = rotation;

        speedometerText.text = speed.ToString("F0");
    }

    
    // Old Coce
    // //12 degree is zero, subtract 12
    // public void UpdateSpeedometer(float speed)
    // {
    //     //164 from 0 to max
    //     float convertedspeed = (speed * 0.625f);
    //     float spedometerangle = (speed / 160.0f) * 164;
    //
    //
    //     Quaternion roation = Quaternion.Euler(0, 0, -(spedometerangle - 10.5f));
    //     needle.transform.rotation = roation;
    //     
    //     speedometerText.text = speed.ToString("F0");
    // }
}
