using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class volumeSettings : MonoBehaviour
{
    [Header("----- Drag N Drop -----")]
    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] public Slider MusicSlider;
    [SerializeField] public Slider SFXSlider;
    [SerializeField] public AudioClip testMusic;
    [SerializeField] public AudioClip testSFX;

    float musicVolume;
    float sfxVolume;

    float testMusicVolume;
    float testSFXVolume;

    private void Awake()
    {
        MusicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
        SFXSlider.onValueChanged.AddListener(delegate { SetSFXVolume();  });
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        MusicSlider.value = musicVolume;
        SFXSlider.value = sfxVolume;
        //testMusicVolume = musicVolume;
        //testSFXVolume = sfxVolume;
        //SetMixerMusicVolume();
        //SetMixerSFXVolume();
    }

    public void SetMusicVolume()
    {
        testMusicVolume = MusicSlider.value;
        SetMixerMusicVolume();
    }

    public void SetSFXVolume()
    {
        testSFXVolume = SFXSlider.value;
        SetMixerSFXVolume();
    }
    
    public void SaveVolumeLevels()
    {
        musicVolume = testMusicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        sfxVolume = testSFXVolume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void CancelChanges()
    {
        testMusicVolume = musicVolume;
        testSFXVolume = sfxVolume;
        SetMixerMusicVolume();
        SetMixerSFXVolume();
    }


    void SetMixerMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(testMusicVolume) * 20);
    }

    void SetMixerSFXVolume()
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(testSFXVolume) * 20);
    }
}
