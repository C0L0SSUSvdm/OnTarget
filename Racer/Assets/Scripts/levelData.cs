using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelData : MonoBehaviour
{
    public AudioClip levelMusic;

    private float levelTime;

    void Start()
    {
        if(levelMusic != null)
            gameManager.instance.SetMusicSource(levelMusic);
    }

    private void Update()
    {
        if(gameManager.instance.TitleScreen == false && gameManager.instance.Hud != null)
        {
            levelTime += Time.deltaTime;
            HUD.Item.UpdateTimer(levelTime);
        }

    }

}