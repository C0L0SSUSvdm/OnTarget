using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TitleScreen : MonoBehaviour
{
    public void Awake()
    {
        gameManager.instance.TitleScreen = true;
    }

    public void Start()
    {
        gameManager.instance.GameManagerInitialized = true;
        //gameManager.instance.SelectedUI = "GamePlayUI";
        //gameManager.instance.SelectedLevel = "DebugRoom2";
    }

}
