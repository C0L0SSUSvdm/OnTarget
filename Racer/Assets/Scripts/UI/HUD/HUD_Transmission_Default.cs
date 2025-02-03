using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HUD_Transmission_Default : MonoBehaviour, HUD_Transmission_Director
{
    [SerializeField] TextMeshProUGUI GearTextValue;

    public void UpdateTransmissionGear(int gear)
    {
        GearTextValue.text = (1 + gear).ToString();
    }

}

