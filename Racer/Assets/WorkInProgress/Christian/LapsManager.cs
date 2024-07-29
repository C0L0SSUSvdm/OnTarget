using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapsManager : MonoBehaviour
{
    [Header("Checkpoints")]
    public GameObject start;
    public GameObject end;
    public GameObject[] checkpoints;

    [Header("Settings")]
    public float laps = 1;

    [Header("Information")]
    private float currentCheckpoint;
    private float currentLap;
    private bool started;
    private bool finished;

    // Start is called before the first frame update
    void Start()
    {
        currentCheckpoint = 0;
        currentLap = 1;

        started = false;
        finished = false;
    }

    private void OnTriggerEnter(Collider player) 
    {
        if (player.CompareTag("Checkpoint")) 
        {
            GameObject thisCheckpoint = player.gameObject;

            //started race
            if (thisCheckpoint == start && !started)
            {
                print("Started");       //replace with UI indication
                started = true;
            }
            //ended race
            else if (thisCheckpoint == end && started) 
            {
                // if all laps are finished , end the race
                if (currentLap == laps)
                {
                    if (currentCheckpoint == checkpoints.Length)
                    {
                        print("finished");
                        finished = true;
                    }
                    else
                    {
                        print("Did not go through all checkpoints");
                    }
                }
                //if all laps are not finished start new lap
                else if (currentLap < laps)
                {
                    if (currentCheckpoint == checkpoints.Length)
                    {
                        currentLap++;
                        currentCheckpoint = 0;
                        print($"started lap {currentLap}");
                    }
                }
                else 
                {
                    print("Did not go through all the checkpoints");
                }
            }
            //loop through checkpoints and compare and check which one play has passed through
            for (int i = 0; i < checkpoints.Length; i++)
            {
                if (finished) 
                {
                    return;
                }

                //if the checkpoint is correct
                if (thisCheckpoint == checkpoints[i] && i == currentCheckpoint)
                {
                    print("Correct Checkpoint");
                    currentCheckpoint++;
                }
                else if (thisCheckpoint == checkpoints[i] && i != currentCheckpoint) 
                {
                    print("Incorrect Checkpoint");
                }
            }
        }
    }
}
