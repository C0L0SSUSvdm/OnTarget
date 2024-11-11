using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD_Speedometer_Default : MonoBehaviour, HUD_Speedometer_Director
{
    [SerializeField] public TextMeshProUGUI speedometerText;

    public void UpdateSpeedometer(float speed)
    {
        speedometerText.text = (speed * 0.625f).ToString("F0");
    }
}
