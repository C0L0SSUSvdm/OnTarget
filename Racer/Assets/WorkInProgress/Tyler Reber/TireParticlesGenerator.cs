using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireParticlesGenerator : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;

    public void PlayParticles()
    {
        _particleSystem.Play();
    }

    public void StopParticles()
    {
        _particleSystem.Stop();
    }
}
