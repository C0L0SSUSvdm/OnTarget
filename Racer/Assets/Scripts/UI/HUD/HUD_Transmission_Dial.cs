using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD_Transmission_Dial : MonoBehaviour, HUD_Transmission_Director
{
    [SerializeField] GameObject needle;
    [SerializeField] public TextMeshProUGUI textBox;


    public void UpdateTransmissionGear(int gearIndex)
    {
        float zero = -28.0f;
        float rotation = (-14.5f * gearIndex) + zero;
        needle.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
}
