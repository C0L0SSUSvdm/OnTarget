using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

//This Script is Loaded by the gameManager.cs
//After the Startup Splash screens are done, this script initializes the GameManager Settings to run for the title screen.
public class startupScreens : MonoBehaviour
{
    [Header("Fade Objects")]
    [SerializeField] public List<GameObject> FadeObjects = new List<GameObject>();

    [Header("Spawn Timing")]
    [SerializeField] private float delayBetweenGroups = 1.0f;
    [SerializeField] private float delayBetweenObjects = 0.2f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float fadeOutTime = 0.25f;
    [SerializeField] private float startAlpha = 0f;
    [SerializeField] private float targetAlpha = 1f;

    [Header("Scene Management")]
    [SerializeField] private string startupUIScene;
    [SerializeField] private string startupLevelScene;
    [SerializeField] private string SplashScreens;
    [SerializeField] private string BlankScene;

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
        // Initially deactivate all fade objects
        foreach (GameObject fadeObject in FadeObjects)
        {
            fadeObject.SetActive(false);
        }
        
        StartCoroutine(TransitionSlide());
    }
    
    IEnumerator TransitionSlide()
    {
        // Sequential spawning and fade in
        yield return StartCoroutine(SequentialFadeIn());

        // Wait for Space Input
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        // Fade Out all objects simultaneously
        yield return StartCoroutine(FadeOutAll());
        
        // Load next scenes after splash is done
        gameManager.instance.ActiveUI = startupUIScene;
        gameManager.instance.ActiveLevel = startupLevelScene;
        
        SceneManager.LoadSceneAsync(startupUIScene, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(startupLevelScene, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(SplashScreens);
        SceneManager.UnloadSceneAsync(BlankScene);
    }

    IEnumerator SequentialFadeIn()
    {
        // First group (first 2 objects)
        for (int i = 0; i < Mathf.Min(2, FadeObjects.Count); i++)
        {
            if (i > 0)
                yield return new WaitForSeconds(delayBetweenObjects);
            
            StartCoroutine(FadeInSingleObject(FadeObjects[i]));
        }

        // Wait before spawning second group
        yield return new WaitForSeconds(delayBetweenGroups);

        // Second group (remaining objects)
        for (int i = 2; i < FadeObjects.Count; i++)
        {
            StartCoroutine(FadeInSingleObject(FadeObjects[i]));
        }

        // Wait for all fade-ins to complete
        yield return new WaitForSeconds(fadeInTime);
    }

    IEnumerator FadeInSingleObject(GameObject fadeObject)
    {
        // Activate the object
        fadeObject.SetActive(true);
        
        // Set initial alpha to start value
        SetObjectAlpha(fadeObject, startAlpha);

        float timer = 0;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            float ratio = timer / fadeInTime;
            SetObjectAlpha(fadeObject, Mathf.Lerp(startAlpha, targetAlpha, ratio));
            yield return null;
        }

        // Ensure final alpha is exactly the target value
        SetObjectAlpha(fadeObject, targetAlpha);
    }

    IEnumerator FadeOutAll()
    {
        FadeOutTimer = 0;
        while (FadeOutTimer < fadeOutTime)
        {
            FadeOutTimer += Time.deltaTime;
            float ratio = FadeOutTimer / fadeOutTime;
            
            // Fade out all objects simultaneously
            foreach (GameObject fadeObject in FadeObjects)
            {
                SetObjectAlpha(fadeObject, Mathf.Lerp(targetAlpha, startAlpha, ratio));
            }
            
            yield return null;
        }
    }

    private void SetObjectAlpha(GameObject obj, float alpha)
    {
        // Handle UI Image components
        Image imageComponent = obj.GetComponent<Image>();
        if (imageComponent != null)
        {
            Color color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, alpha);
            imageComponent.color = color;
        }

        // Handle TextMeshPro - UI components
        TextMeshProUGUI tmpUIComponent = obj.GetComponent<TextMeshProUGUI>();
        if (tmpUIComponent != null)
        {
            Color color = new Color(tmpUIComponent.color.r, tmpUIComponent.color.g, tmpUIComponent.color.b, alpha);
            tmpUIComponent.color = color;
        }

        // Handle TextMeshPro - 3D components (if needed)
        TextMeshPro tmpComponent = obj.GetComponent<TextMeshPro>();
        if (tmpComponent != null)
        {
            Color color = new Color(tmpComponent.color.r, tmpComponent.color.g, tmpComponent.color.b, alpha);
            tmpComponent.color = color;
        }

        // Handle regular Text components (legacy)
        Text textComponent = obj.GetComponent<Text>();
        if (textComponent != null)
        {
            Color color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, alpha);
            textComponent.color = color;
        }

        // Handle CanvasGroup for more complex fading (optional - affects all child elements)
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
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