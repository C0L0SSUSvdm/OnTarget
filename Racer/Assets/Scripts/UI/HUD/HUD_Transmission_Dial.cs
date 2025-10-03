using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD_Transmission_Dial : MonoBehaviour, HUD_Transmission_Director
{
    [SerializeField] GameObject needle;
    [SerializeField] public TextMeshProUGUI gearText;

    [Header("Transmission Dial Settings")]
    [SerializeField] private float gearAngleStep = 14.5f;
    [SerializeField] private float baseAngle = -28f;
    [SerializeField] private string[] gearLabels = { "R", "N", "1", "2", "3", "4", "5", "6" };

    public void UpdateTransmissionGear(int gearIndex)
    {
        gearIndex = Mathf.Clamp(gearIndex, 0, gearLabels.Length - 1);
        float angle = baseAngle + (-gearAngleStep * gearIndex);

        needle.transform.rotation = Quaternion.Euler(0, 0, angle);
        gearText.text = gearLabels[gearIndex];
    }

    
// OLD CODE
    // public void UpdateTransmissionGear(int gearIndex)
    // {
    //     float zero = -28.0f;
    //     float rotation = (-14.5f * gearIndex) + zero;
    //     needle.transform.rotation = Quaternion.Euler(0, 0, rotation);
    // }
}
