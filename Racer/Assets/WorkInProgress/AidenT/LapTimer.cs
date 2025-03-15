using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LapTimer : MonoBehaviour, HUD_Interface_Director
{
    [Tooltip("Assign this in the inspector")]
    public TextMeshProUGUI lapTimeText; // Assign this in the inspector
    private float lapStartTime;
    private bool lapStarted = false;
    private const string playerTag = "Player"; // Cache the player tag
    void Start()
    {
        if (lapTimeText == null)
        {
            Debug.LogError("LapTimeText is not assigned in the inspector.");
            enabled = false; // Disable the script to prevent further errors
            return;
        }
        lapStartTime = Time.time;
        lapStarted = true;
    }
    //void OnTriggerEnter(Collider other)
    //{
    //    if (lapStarted && other.CompareTag(playerTag))
    //    {
    //        float currentTime = Time.time;
    //        float lapTime = currentTime - lapStartTime;
    //        lapTimeText.text = FormatTime(lapTime);
    //        lapStartTime = currentTime; // reset for next lap
    //    }
    //}
    
    public void UpdateTimer(float rawtime)
    {
        TimeSpan time = TimeSpan.FromSeconds(rawtime);
        //float test = rawtime % time.Seconds;
        //Debug.Log(test);
        lapTimeText.text = time.ToString("hh':'mm':'ss':'fff");
    }
    public void createTimestamp()
    {
        TimeSpan time = TimeSpan.FromSeconds(gameManager.instance.timeSinceRaceStart);
        lapTimeText.text = time.ToString("hh':'mm':'ss':'fff");
    }
}

