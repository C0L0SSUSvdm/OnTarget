using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HUD_HealthBar_Default : MonoBehaviour, HUD_HealthBar_Director
{
    [Header("----- Drag N Drop -----")]
    [SerializeField] public Image healthbar;

    public void UpdateHealthBar(float ratio)
    {
        healthbar.fillAmount = ratio;
    }

}
