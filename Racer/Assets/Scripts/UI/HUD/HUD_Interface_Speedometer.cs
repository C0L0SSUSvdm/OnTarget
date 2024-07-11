using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HUD_Speedometer_Director
{
    public void UpdateSpeedometer(float speed);
}

public class HUD_Interface_Speedometer : MonoBehaviour
{
    [Header("Found On Start")]
    [SerializeField] private GameObject defaultSpeedometer;    

    [Header("----- Fields -----")]
    [SerializeField] private GameObject activeSpeedometer;
    [SerializeField] private int childIndex = 0;

    public void Start()
    {
        HUD.Item.SetSpeedometerInterface(gameObject);
        activeSpeedometer = gameObject.transform.GetChild(childIndex).gameObject;
        activeSpeedometer.SetActive(true);
    }

    public void SetSpeedometer()
    {
        activeSpeedometer.SetActive(false);
        activeSpeedometer = gameObject.transform.GetChild(childIndex).gameObject;
        activeSpeedometer.SetActive(true);
    }

    public void UpdateSpeedometer(float speed)
    {
        activeSpeedometer.GetComponent<HUD_Speedometer_Director>().UpdateSpeedometer(speed);
    }

}
