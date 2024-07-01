using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class HUD_Timer_Default : MonoBehaviour, HUD_Interface_Director
{
    [Header("----- Drag N Drop -----")]
    [SerializeField] TextMeshProUGUI timerText;

    public void UpdateTimer(float rawtime)
    {
        TimeSpan time = TimeSpan.FromSeconds(rawtime);
        //float test = rawtime % time.Seconds;
        //Debug.Log(test);
        timerText.text = time.ToString("hh':'mm':'ss':'fff");
    }
}
