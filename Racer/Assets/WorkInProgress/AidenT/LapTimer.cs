using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LapTimer : MonoBehaviour
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

    void OnTriggerEnter(Collider other)
    {
        if (lapStarted && other.CompareTag(playerTag))
        {
            float currentTime = Time.time;
            float lapTime = currentTime - lapStartTime;
            lapTimeText.text = FormatTime(lapTime);
            lapStartTime = currentTime; // reset for next lap
        }
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 1000F) % 1000F);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}
