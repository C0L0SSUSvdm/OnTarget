using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HUD_Transmission_Director
{
    public void UpdateTransmissionGear(int gear);
}

public class HUD_Interface_Transmission : MonoBehaviour
{
    [Header("Found On Start")]
    [SerializeField] private GameObject defaultTransmissionDisplay;

    [Header("----- Fields -----")]
    [SerializeField] private GameObject activeTransmissionDisplay;
    [SerializeField] private int childIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        HUD.Item.SetTransmissionGuageInterface(gameObject);
        activeTransmissionDisplay = gameObject.transform.GetChild(childIndex).gameObject;
        activeTransmissionDisplay.SetActive(true);
    }

    public void SetTransmissionDisplay()
    {
        activeTransmissionDisplay.SetActive(false);
        activeTransmissionDisplay = gameObject.transform.GetChild(childIndex).gameObject;
        activeTransmissionDisplay.SetActive(true);
    }

    public void UpdateTransmissionDisplay(int value)
    {
        activeTransmissionDisplay.GetComponent<HUD_Transmission_Director>().UpdateTransmissionGear(value);
    }
}

