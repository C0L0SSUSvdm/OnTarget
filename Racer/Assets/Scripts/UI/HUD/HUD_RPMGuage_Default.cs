using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD_RPMGuage_Default : MonoBehaviour, HUD_RPMGuage_Director
{
    [SerializeField] public TextMeshProUGUI speedometerText;

    public void UpdateRPMGuage(int value)
    {
        speedometerText.text = value.ToString();
    }
}
