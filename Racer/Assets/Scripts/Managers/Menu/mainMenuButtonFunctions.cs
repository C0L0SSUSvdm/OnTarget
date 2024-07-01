using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenuButtonFunctions : MonoBehaviour
{
    public void StartGame()
    {
        gameManager.instance.TitleScreen = false;
        gameManager.instance.LoadSelectedLevel();
    }

    public void Resume()
    {
        gameManager.instance.ToggleMenu();
    }

    public void Restart()
    {
        gameManager.instance.SelectedUI = gameManager.instance.ActiveUI;
        gameManager.instance.SelectedLevel = gameManager.instance.ActiveLevel;
        gameManager.instance.LoadSelectedLevel();

    }

    public void OpenSubMenuDynamically()
    {
        gameManager.instance.GetMenuScript().OpenSubMenuByName();
    }

    public void OpenSubMenu(GameObject gameObject)
    {
        gameManager.instance.GetMenuScript().OpenSubMenu(gameObject);
    }

    //public void Settings()
    //{
        //gameManager.instance.GetMenuScript().ToggleSettingsMenu();
    //}

    public void EscapeMenu()
    {
        gameManager.instance.ToggleMenu();
    }

    public void QuitToTitleScreen()
    {
        gameManager.instance.SelectedUI = "TitleUI";
        gameManager.instance.SelectedLevel = "TitleLevel";
        gameManager.instance.LoadSelectedLevel();
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }

    public void TestButton()
    {
        gameManager.instance.InitializeAudioMixer();
    }
}
