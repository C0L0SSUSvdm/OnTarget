using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD Item;

    [Header("----- Self Initialized -----")]
    [SerializeField] public HUD_Interface_Speedometer speedometerInterface;
    [SerializeField] public HUD_Interface_HealthBar healthBarInterface;
    [SerializeField] public HUD_Interface_Timer timerInterface;

    private void Awake()
    {
        if (Item != null)
            Destroy(gameObject);
        else
            Item = this;

        gameManager.instance.SetHUDObject(this.gameObject);
    }


    public void ResetHUD()
    {
        Item = null;
    }

    public void UpdateSpeedometer(float speed)
    {
        speedometerInterface.UpdateSpeedometer(speed);
    }

    public void UpdateHealthBar(float health)
    {
        healthBarInterface.UpdateHealthBar(health);
    }

    public void UpdateTimer(float time)
    {
        timerInterface.UpdateTimer(time);
    }

    /********** SETTERS *********/
    public void SetSpeedometerInterface(GameObject gameobject)
    {
        speedometerInterface = gameobject.GetComponent<HUD_Interface_Speedometer>();
    }
    public void SetHealthBarInterface(GameObject gameobject)
    {
        healthBarInterface = gameobject.GetComponentInChildren<HUD_Interface_HealthBar>();
    }
    public void SetTimerInterface(GameObject gameobject)
    {
        timerInterface = gameobject.GetComponentInChildren<HUD_Interface_Timer>();
    }

}
