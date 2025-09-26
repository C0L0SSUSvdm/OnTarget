using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LapTimer : MonoBehaviour, HUD_Interface_Director
{
    [Tooltip("Assign this in the inspector")]
    public TextMeshProUGUI lapTimeText; // Assign this in the inspector
    
    [Header("Update Settings")]
    [Tooltip("How many times per second to update the display (lower = less jittery)")]
    public int updatesPerSecond = 10; // Update 10 times per second instead of every frame
    
    private float lapStartTime;
    private bool lapStarted = false;
    private const string playerTag = "Player"; // Cache the player tag
    
    // Anti-jitter variables
    private float lastUpdateTime;
    private float updateInterval;
    private string lastDisplayedTime = "";
    
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
        updateInterval = 1f / updatesPerSecond;
        lastUpdateTime = 0f;
    }
    
    public void UpdateTimer(float rawtime)
    {
        // Only update if enough time has passed since last update
        if (Time.time - lastUpdateTime < updateInterval)
            return;
            
        TimeSpan time = TimeSpan.FromSeconds(rawtime);
        string formattedTime = time.ToString("hh':'mm':'ss':'fff");
        
        // Only update text if it actually changed
        if (formattedTime != lastDisplayedTime)
        {
            lapTimeText.text = formattedTime;
            lastDisplayedTime = formattedTime;
        }
        
        lastUpdateTime = Time.time;
    }
    
    public void createTimestamp()
    {
        // Only update if enough time has passed since last update
        if (Time.time - lastUpdateTime < updateInterval)
            return;
            
        TimeSpan time = TimeSpan.FromSeconds(gameManager.instance.timeSinceRaceStart);
        string formattedTime = time.ToString("hh':'mm':'ss':'fff");
        
        // Only update text if it actually changed
        if (formattedTime != lastDisplayedTime)
        {
            lapTimeText.text = formattedTime;
            lastDisplayedTime = formattedTime;
        }
        
        lastUpdateTime = Time.time;
    }
}