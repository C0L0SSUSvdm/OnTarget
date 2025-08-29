using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//This Script is Loaded by the gameManager.cs
//After the Startup Splash screens are done, this script initializes the GameManager Settings to run for the title screen.
public class startupScreens : MonoBehaviour
{
    [SerializeField] public List<Image> FadeObjects = new List<Image>();

    [SerializeField] private string startupUIScene;
    [SerializeField] private string startupLevelScene;
    [SerializeField] private string SplashScreens;
    [SerializeField] private string BlankScene;
    
    private float maxslideTime = 0.25f;
    private float maxFadeTime = 0.5f;

    private float FadeInTimer = 0;
    private float FadeOutTimer = 0;
    private float ShowSlideTimer = 0;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    private void Awake()
    {
        gameManager.instance.SetMenuObject(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Activate all fade objects
        foreach (Image fadeObject in FadeObjects)
        {
            fadeObject.gameObject.SetActive(true);
        }
        
        StartCoroutine(TransitionSlide());
    }
    
    IEnumerator TransitionSlide()
    {
        // Fade In
        while (FadeInTimer < maxFadeTime)
        {
            FadeInTimer += Time.deltaTime;
            float ratio = FadeInTimer / maxFadeTime;
            
            // Fade in all objects simultaneously
            foreach (Image fadeObject in FadeObjects)
            {
                Color color = new Color(fadeObject.color.r, fadeObject.color.g, fadeObject.color.b, Mathf.Lerp(0, 1, ratio));
                fadeObject.color = color;
            }

            yield return null;
        }

        // Show Slide - Wait minimum time first
        while (ShowSlideTimer < maxslideTime)
        {
            ShowSlideTimer += Time.deltaTime;
            yield return null;
        }

        // Wait for Space Input
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        // Fade Out
        while (FadeOutTimer < maxFadeTime * 0.5f)
        {
            FadeOutTimer += Time.deltaTime;
            float ratio = FadeOutTimer / (maxFadeTime * 0.5f);
            
            // Fade out all objects simultaneously
            foreach (Image fadeObject in FadeObjects)
            {
                Color color = new Color(fadeObject.color.r, fadeObject.color.g, fadeObject.color.b, Mathf.Lerp(1, 0, ratio));
                fadeObject.color = color;
            }
            
            yield return null;
        }
        
        // Load next scenes after splash is done
        gameManager.instance.ActiveUI = startupUIScene;
        gameManager.instance.ActiveLevel = startupLevelScene;
        
        SceneManager.LoadSceneAsync(startupUIScene, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(startupLevelScene, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(SplashScreens);
        SceneManager.UnloadSceneAsync(BlankScene);

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
                gameManager.instance.LoadingScreen.GetComponent<loadingBar>().UpdateFillBar(ratio);
                yield return null;
            }
        }

        gameManager.instance.LoadingScreen.SetActive(false);
    }
}