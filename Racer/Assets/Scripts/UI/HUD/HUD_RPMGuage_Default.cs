using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD_RPMGuage_Default : MonoBehaviour, HUD_RPMGuage_Director
{

    [SerializeField] GameObject needle;
    [SerializeField] public TextMeshProUGUI rpmText;

    [Header("RPM Gauge Settings")]
    [SerializeField] private float maxRPM = 8000f;
    [SerializeField] private float maxAngle = 180f;
    [SerializeField] private float zeroOffset = 30f;

    public void UpdateRPMGuage(int rpm)
    {
        float normalizedRPM = Mathf.Clamp01(rpm / maxRPM);
        float angle = normalizedRPM * maxAngle;

        Quaternion rotation = Quaternion.Euler(0, 0, -(angle - zeroOffset));
        needle.transform.rotation = rotation;

        rpmText.text = rpm.ToString("F0");
    }

    
    // OLD CODE
    // //12 degree is zero, subtract 12
    // public void UpdateRPMGuage(int rpm)
    // {
    //     //164 from 0 to max
    //     //float convertedspeed = (rpm * 0.625f);
    //     float spedometerangle = (rpm / 1000.0f) * 15.3f; // 46 euler = 3000 rpm
    //
    //
    //     Quaternion rotation = Quaternion.Euler(0, 0, -(spedometerangle - 30.0f));
    //     needle.transform.rotation = rotation;
    //
    //     speedometerText.text = rpm.ToString("F0");
    //     
    // }
}
