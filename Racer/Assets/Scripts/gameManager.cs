using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

//gameManager class exists throughout the entire game, its never removed

// Menu and Hud Singletons get recreated. I don't like it. but that's what it is right now.

public class gameManager : MonoBehaviour
{
    // Using a singleton pattern to make sure there is only one instance of the gameManager
    public static gameManager instance;

    [Header("----- Managers -----")]
    [SerializeField] public GameObject Menu;
    [SerializeField] public GameObject Hud;
    private menu MENUScript;
    [Header("----- Loading Assets -----)")]
    [SerializeField] public GameObject LoadingScreen;
    [SerializeField] public string SelectedUI = "SplashScreens";
    [SerializeField] public string SelectedLevel = "BlankScene";
    [SerializeField] public string SelectedMode = "Default"; //Not Implemented
    [SerializeField] public string ActiveUI = "";
    [SerializeField] public string ActiveLevel = "";
    [SerializeField] public string ActiveMode = ""; //Not Implemented
    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    [Header("----- Settings -----")]
    [SerializeField] public bool TitleScreen = true;
    [SerializeField] public bool GameManagerInitialized = false;

    [Header("----- Player References -----")]
    [SerializeField] public GameObject playerObject;
    [SerializeField] public prototypeCar playerScript;

    [Header("----- Audio References -----")]
    [SerializeField] public AudioMixer Mixer;
    [SerializeField] public AudioSource MusicSource;
    [SerializeField] public AudioSource SFX;

    void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;     
    }

    private void Start()
    {   
        InitializeAudioMixer();
        if(GameManagerInitialized == false)
        {
            scenesToLoad.Add(SceneManager.LoadSceneAsync(SelectedUI, LoadSceneMode.Additive));
            scenesToLoad.Add(SceneManager.LoadSceneAsync(SelectedLevel, LoadSceneMode.Additive));
            StartCoroutine(LoadingProgress());
        }
    }

    void Update()
    {
        Escape_KeyPress();
    }

    public menu GetMenuScript()
    {
        return MENUScript;
    }

    void Escape_KeyPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        bool active = MENUScript.UpdateMenuState();
        if (active == false)
        {
            Hud.SetActive(true);
            UnpauseGame();
        }
        else
        {
            if (Hud.activeSelf == true)
            {
                Hud.SetActive(false);
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        if (TitleScreen == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
    }

    void UnpauseGame()
    {
        if (TitleScreen == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        }
    }
    // Had to look up how to hand scene changes, this is the resource I reffered to
    //https://www.youtube.com/watch?v=zObWVOv1GlE
    public void LoadSelectedLevel()
    {
        StopMusic();
        Menu.SetActive(false);
        LoadingScreen.SetActive(true);

        
        
        SceneManager.UnloadSceneAsync(ActiveUI);
        SceneManager.UnloadSceneAsync(ActiveLevel);
        scenesToLoad.Add(SceneManager.LoadSceneAsync(SelectedUI, LoadSceneMode.Additive));
        scenesToLoad.Add(SceneManager.LoadSceneAsync(SelectedLevel, LoadSceneMode.Additive));
        //Might need to clear Menu references in game manager
        HUD.Item.ResetHUD();
        Hud = null;

        ActiveUI = SelectedUI;
        ActiveLevel = SelectedLevel;

        if (playerObject != null)
            playerObject.SetActive(false);

        StartCoroutine(LoadingProgress());
    }

    IEnumerator LoadingProgress()
    {
        float totalProgress = 0;
        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            while (!scenesToLoad[i].isDone)
            {
                totalProgress += scenesToLoad[i].progress;
                float ratio = totalProgress / scenesToLoad.Count;
                LoadingScreen.GetComponent<loadingBar>().UpdateFillBar(ratio);
                yield return null;
            }
        }
        LoadingScreen.SetActive(false);
        UnpauseGame();
    }

    // Setters, When A scene loads, the assets are to initialize themselves to the gameManager
    public void SetMenuObject(GameObject gameObject)
    {
        Menu = gameObject;
        MENUScript = Menu.GetComponent<menu>();
    }

    public void SetHUDObject(GameObject gameObject)
    {
        Hud = gameObject;
    }

    public void SetPlayerObejct(GameObject player)
    {
        //if(player.name == "Player")
        //{
            playerObject = player;
            playerScript = playerObject.GetComponent<prototypeCar>();
        //}
    }

    public void InitializeAudioMixer()
    {          
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        Mixer.SetFloat("MusicVolume", Mathf.Log10(music) * 20);
        Mixer.SetFloat("SFXVolume", Mathf.Log10(sfx) * 20);
    }

    public void SetMusicSource(AudioClip source)
    {
        MusicSource.clip = source;
        MusicSource.Play();
    }
    public void StopMusic()
    {
        MusicSource.Stop();
    }
}
