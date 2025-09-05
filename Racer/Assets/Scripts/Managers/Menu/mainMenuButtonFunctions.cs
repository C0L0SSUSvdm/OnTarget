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
    
    [Header("Level Select Camera Setup")]
    public List<Transform> levelSelectTargets; // Poster transforms
    public float transitionSpeed = 2f;
    
    // Dictionary to store our cameras by name
    private Dictionary<string, CinemachineVirtualCamera> vCameras = new Dictionary<string, CinemachineVirtualCamera>();
    private string currentActiveCam = "";
    private bool garageSceneLoaded = false;
    
    // Level select camera variables
    private Transform levelSelectFollowTarget;
    private int currentPosterIndex = 0;
    private bool isTransitioning = false;
    
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
            FindLevelPosters();
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
                    vCameras[mainVCamName] = cam;
                    Debug.Log($"Found Main VCam: {camName}");
                }
                else if (camName == levelSelectVCamName)
                {
                    vCameras[levelSelectVCamName] = cam;
                    Debug.Log($"Found Level Select VCam: {camName}");
                    
                    // Get the look at target from the level select camera
                    if (cam.Follow != null)
                    {
                        levelSelectFollowTarget = cam.Follow;
                        Debug.Log($"Found level select look at target: {levelSelectFollowTarget.name}");
                    }
                }
                else if (camName == garrageVCamName)
                {
                    vCameras[garrageVCamName] = cam;
                    Debug.Log($"Found Settings VCam: {camName}");
                }
            }
        }
    
        Debug.Log($"Found {vCameras.Count} VCameras in Garage scene");
    }
    
    void FindLevelPosters()
    {
        levelSelectTargets.Clear(); // Clear existing targets
        
        Scene garageScene = SceneManager.GetSceneByName("Garage");
        if (!garageScene.isLoaded)
        {
            Debug.LogWarning("Garage scene is not loaded!");
            return;
        }
        
        // Find poster GameObjects by name pattern
        for (int i = 1; i <= 4; i++) // Looking for Level 1-4 Posters
        {
            string posterName = $"Poster {i} Transform";
            GameObject posterObject = GameObject.Find(posterName);
            
            if (posterObject != null)
            {
                levelSelectTargets.Add(posterObject.transform);
                Debug.Log($"Found poster: {posterName}");
            }
            else
            {
                Debug.LogWarning($"Could not find poster: {posterName}");
            }
        }
        
        Debug.Log($"Found {levelSelectTargets.Count} level posters in Garage scene");
    }
    
    void SwitchCamera(string targetCamera)
    {
        if (!garageSceneLoaded)
        {
            Debug.LogWarning("Garage scene not loaded yet, cannot switch cameras");
            return;
        }
    
        Debug.Log($"Attempting to switch to camera: {targetCamera}");
        Debug.Log($"Available cameras: {string.Join(", ", vCameras.Keys)}");
    
        // Check if target camera exists
        if (!vCameras.ContainsKey(targetCamera))
        {
            Debug.LogError($"Camera '{targetCamera}' not found! Available cameras: {string.Join(", ", vCameras.Keys)}");
            return;
        }
    
        if (vCameras[targetCamera] == null)
        {
            Debug.LogError($"Camera '{targetCamera}' reference is null!");
            return;
        }
    
        // Turn off all cameras first
        foreach (var kvp in vCameras)
        {
            if (kvp.Value != null)
            {
                kvp.Value.Priority = 0;
                Debug.Log($"Set {kvp.Key} camera priority to 0");
            }
        }
    
        // Turn on target camera with higher priority
        vCameras[targetCamera].Priority = 100; // Use higher priority to ensure it takes precedence
        currentActiveCam = targetCamera;
    
        Debug.Log($"Successfully switched to {targetCamera} camera (Priority: {vCameras[targetCamera].Priority})");
    
        // Optional: Log all camera priorities for debugging
        foreach (var kvp in vCameras)
        {
            if (kvp.Value != null)
            {
                Debug.Log($"{kvp.Key} camera priority: {kvp.Value.Priority}");
            }
        }
    }
    
    public void SwitchToGarrageCamera()
    {
        SwitchCamera(garrageVCamName);
    }

    public void SwitchToLevelSelectCamera()
    {
        SwitchCamera(levelSelectVCamName);
        
        // Initialize to first poster when entering level select
        if (levelSelectTargets.Count > 0)
        {
            currentPosterIndex = 0;
            SetPosterImmediate(currentPosterIndex);
        }
    }

    public void SwitchToSettingsCamera()
    {
        SwitchCamera(mainVCamName);
    }
    
    /// <summary>
    /// Navigate to the next poster (right button)
    /// </summary>
    public void NextPoster()
    {
        if (levelSelectTargets.Count == 0 || isTransitioning) return;
        
        currentPosterIndex = (currentPosterIndex + 1) % levelSelectTargets.Count;
        TransitionToPoster(currentPosterIndex);
        
        Debug.Log($"Next poster: {currentPosterIndex + 1}/{levelSelectTargets.Count}");
    }
    
    /// <summary>
    /// Navigate to the previous poster (left button)
    /// </summary>
    public void PreviousPoster()
    {
        if (levelSelectTargets.Count == 0 || isTransitioning) return;
        
        // Handle wrapping: if at index 0, go to last poster
        currentPosterIndex = (currentPosterIndex - 1 + levelSelectTargets.Count) % levelSelectTargets.Count;
        TransitionToPoster(currentPosterIndex);
        
        Debug.Log($"Previous poster: {currentPosterIndex + 1}/{levelSelectTargets.Count}");
    }
    
    /// <summary>
    /// Get the current poster index (0-based)
    /// </summary>
    public int GetCurrentPosterIndex()
    {
        return currentPosterIndex;
    }
    
    /// <summary>
    /// Get the current poster name (if available)
    /// </summary>
    public string GetCurrentPosterName()
    {
        if (levelSelectTargets.Count == 0 || currentPosterIndex >= levelSelectTargets.Count)
            return "None";
            
        return levelSelectTargets[currentPosterIndex].name;
    }
    
    /// <summary>
    /// Jump directly to a specific poster by index
    /// </summary>
    public void GoToPoster(int index)
    {
        if (levelSelectTargets.Count == 0 || isTransitioning) return;
        
        // Clamp index to valid range
        index = Mathf.Clamp(index, 0, levelSelectTargets.Count - 1);
        currentPosterIndex = index;
        TransitionToPoster(currentPosterIndex);
        
        Debug.Log($"Jump to poster: {currentPosterIndex + 1}/{levelSelectTargets.Count}");
    }
    
    /// <summary>
    /// Smoothly transition to a specific poster
    /// </summary>
    private void TransitionToPoster(int posterIndex)
    {
        if (levelSelectTargets[posterIndex] != null && levelSelectFollowTarget != null)
        {
            StartCoroutine(MoveLookAtTarget(levelSelectTargets[posterIndex].position));
        }
    }
    
    /// <summary>
    /// Immediately set the camera to look at a specific poster (no transition)
    /// </summary>
    private void SetPosterImmediate(int posterIndex)
    {
        if (levelSelectTargets[posterIndex] != null && levelSelectFollowTarget != null)
        {
            levelSelectFollowTarget.position = levelSelectTargets[posterIndex].position;
            Debug.Log($"Set camera to poster: {posterIndex + 1}/{levelSelectTargets.Count}");
        }
    }
    
    /// <summary>
    /// Smoothly moves the look at target to a new position
    /// </summary>
    private IEnumerator MoveLookAtTarget(Vector3 targetPosition)
    {
        if (levelSelectFollowTarget == null) yield break;
        
        isTransitioning = true;
        
        Vector3 startPosition = levelSelectFollowTarget.position;
        float elapsed = 0f;
        float duration = 1f / transitionSpeed;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            // Smooth transition
            progress = Mathf.SmoothStep(0f, 1f, progress);
            
            levelSelectFollowTarget.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }
        
        levelSelectFollowTarget.position = targetPosition;
        isTransitioning = false;
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