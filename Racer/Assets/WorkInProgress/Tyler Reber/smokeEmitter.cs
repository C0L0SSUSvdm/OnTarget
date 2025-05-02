using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smokeEmitter : MonoBehaviour
{
    public GameObject emitterObj;
    private ParticleSystem emitter;

    public bool isTriggered = false;

    private void Start()
    {
        emitter = GetComponent<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        if (isTriggered)
        {
            emitter.Play();
        }
        else
        {
            emitter.Stop();
        }

        //isTriggered = false;
    }


    public void TriggerSmokeEffectThisFrame() { 
        isTriggered = true;
    }
}
