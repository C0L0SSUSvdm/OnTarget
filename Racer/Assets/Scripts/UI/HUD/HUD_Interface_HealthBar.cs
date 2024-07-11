using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HUD_HealthBar_Director
{
    public void UpdateHealthBar(float health);
}

public class HUD_Interface_HealthBar : MonoBehaviour
{
    [Header("----- Found On Start -----")]
    [SerializeField] private GameObject defaultHealthBar;

    [Header("----- Fields -----")]
    [SerializeField] private GameObject activeHealthBar;
    [SerializeField] private int childIndex = 0;



    void Start()
    {
        HUD.Item.SetHealthBarInterface(gameObject);
        activeHealthBar = gameObject.transform.GetChild(childIndex).gameObject;
        activeHealthBar.SetActive(true);
    }

    public void SetActiveHealthBar()
    {
        activeHealthBar.SetActive(false);
        activeHealthBar = gameObject.transform.GetChild(childIndex).gameObject;
        activeHealthBar.SetActive(true);
    }

    public void UpdateHealthBar(float health)
    {
        activeHealthBar.GetComponent<HUD_HealthBar_Director>().UpdateHealthBar(health);
    }
}
