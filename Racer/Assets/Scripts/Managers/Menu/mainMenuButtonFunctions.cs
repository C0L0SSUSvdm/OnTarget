using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class mainMenuButtonFunctions : MonoBehaviour
{
    [Header("Camera Names (must match GameObject names in Garage scene)")]
    public string mainVCamName = "CM vcam1";
    public string levelSelectVCamName = "CM vcam1";
    public string garrageVCamName = "CM vcam1";
    
    // Dictionary to store our cameras by name
    private Dictionary<string, CinemachineVirtualCamera> vCameras = new Dictionary<string, CinemachineVirtualCamera>();
    private string currentActiveCam = "";
    private bool garageSceneLoaded = false;
    
    void Awake()
    {
        // Listen for scene loaded events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDestroy()
    {
        // Remove listener to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Garage")
        {
            Debug.Log("Garage scene loaded, setting up cameras and buttons");
            FindVCamerasInGarageScene();
            garageSceneLoaded = true;
        }
    }
    
    void FindVCamerasInGarageScene()
    {
        vCameras.Clear(); // Clear existing cameras
    
        Scene garageScene = SceneManager.GetSceneByName("Garage");
        if (!garageScene.isLoaded)
        {
            Debug.LogWarning("Garage scene is not loaded!");
            return;
        }
    
        // Find all CinemachineVirtualCamera components in the scene directly
        CinemachineVirtualCamera[] allCams = FindObjectsOfType<CinemachineVirtualCamera>();
    
        foreach (var cam in allCams)
        {
            // Only process cameras that belong to the Garage scene
            if (cam.gameObject.scene == garageScene)
            {
                string camName = cam.gameObject.name;
            
                // Store cameras by their names
                if (camName == mainVCamName)
                {
                    vCameras["main"] = cam;
                    Debug.Log($"Found Main VCam: {camName}");
                }
                else if (camName == levelSelectVCamName)
                {
                    vCameras["levelselect"] = cam;
                    Debug.Log($"Found Level Select VCam: {camName}");
                }
                else if (camName == garrageVCamName)
                {
                    vCameras["garrageVCamName"] = cam;
                    Debug.Log($"Found Settings VCam: {camName}");
                }
            }
        }
    
        Debug.Log($"Found {vCameras.Count} VCameras in Garage scene");
    }
    
    void SwitchCamera(string targetCamera)
    {
        if (!garageSceneLoaded)
        {
            Debug.LogWarning("Garage scene not loaded yet, cannot switch cameras");
            return;
        }
        
        // Turn off all cameras
        foreach (var cam in vCameras.Values)
        {
            if (cam != null)
                cam.Priority = 0;
        }
        
        // Turn on target camera
        if (vCameras.ContainsKey(targetCamera) && vCameras[targetCamera] != null)
        {
            vCameras[targetCamera].Priority = 10;
            currentActiveCam = targetCamera;
            Debug.Log($"Switched to {targetCamera} camera");
        }
        else
        {
            Debug.LogWarning($"Camera '{targetCamera}' not found in Garage scene!");
        }
    }
    
    public void SwitchToGarrageCamera()
    {
        SwitchCamera(garrageVCamName);
    }

    public void SwitchToLevelSelectCamera()
    {
        SwitchCamera(levelSelectVCamName);
    }

    public void SwitchToSettingsCamera()
    {
        SwitchCamera(mainVCamName);
    }
    
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
        gameManager.instance.SelectedUI = "UI_Title";
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
