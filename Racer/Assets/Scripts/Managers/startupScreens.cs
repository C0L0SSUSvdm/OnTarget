using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class FadeObjectData
{
    [Header("Object Settings")]
    public GameObject fadeObject;
    
    [Header("Timing")]
    public float delayBeforeStart = 0f;
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.25f;
    
    [Header("Alpha Values")]
    [Range(0f, 1f)]
    public float startAlpha = 0f;
    [Range(0f, 1f)]
    public float targetAlpha = 1f;
    
    [Header("Animation")]
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
}

//This Script is Loaded by the gameManager.cs
//After the Startup Splash screens are done, this script initializes the GameManager Settings to run for the title screen.
public class startupScreens : MonoBehaviour
{
    [Header("Fade Configuration")]
    [SerializeField] private List<FadeObjectData> fadeObjectsData = new List<FadeObjectData>();

    [Header("Input Settings")]
    [SerializeField] private KeyCode continueKey = KeyCode.Space;

    [Header("Scene Management")]
    [SerializeField] private string startupUIScene;
    [SerializeField] private string startupLevelScene;
    [SerializeField] private string SplashScreens;
    [SerializeField] private string BlankScene;

    private void Awake()
    {
        gameManager.instance.SetMenuObject(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initially deactivate all fade objects and validate data
        foreach (FadeObjectData data in fadeObjectsData)
        {
            if (data.fadeObject != null)
            {
                data.fadeObject.SetActive(false);
            }
        }
        
        StartCoroutine(TransitionSlide());
    }
    
    IEnumerator TransitionSlide()
    {
        // Start all fade-in sequences and wait for them to complete
        yield return StartCoroutine(StartAllFadeIns());

        // Now wait for Space input to continue to next scene
        while (!Input.GetKeyDown(continueKey))
        {
            yield return null;
        }

        // Fade Out all objects simultaneously
        yield return StartCoroutine(FadeOutAll());
        
        // Load next scenes after fade out is complete
        LoadNextScenes();
    }

    IEnumerator StartAllFadeIns()
    {
        // Process each fade object in sequence (wait for each to complete before starting next)
        foreach (FadeObjectData data in fadeObjectsData)
        {
            if (data.fadeObject != null)
            {
                // Wait for the specified delay before starting this object
                if (data.delayBeforeStart > 0)
                {
                    yield return new WaitForSeconds(data.delayBeforeStart);
                }

                // Start and wait for this object's fade to complete
                yield return StartCoroutine(FadeInSingleObject(data));
            }
        }
    }

    IEnumerator FadeInSingleObject(FadeObjectData data)
    {
        // Activate the object
        data.fadeObject.SetActive(true);
        
        // Set initial alpha to start value
        SetObjectAlpha(data.fadeObject, data.startAlpha);

        float timer = 0;
        while (timer < data.fadeInDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / data.fadeInDuration;
            float curveValue = data.fadeInCurve.Evaluate(normalizedTime);
            float alpha = Mathf.Lerp(data.startAlpha, data.targetAlpha, curveValue);
            
            SetObjectAlpha(data.fadeObject, alpha);
            yield return null;
        }

        // Ensure final alpha is exactly the target value
        SetObjectAlpha(data.fadeObject, data.targetAlpha);
    }

    IEnumerator FadeOutAll()
    {
        List<Coroutine> fadeOutCoroutines = new List<Coroutine>();

        // Start fade-out for all active objects
        foreach (FadeObjectData data in fadeObjectsData)
        {
            if (data.fadeObject != null && data.fadeObject.activeInHierarchy)
            {
                Coroutine fadeOutCoroutine = StartCoroutine(FadeOutSingleObject(data));
                fadeOutCoroutines.Add(fadeOutCoroutine);
            }
        }

        // Wait for all fade-outs to complete
        foreach (Coroutine fadeOutCoroutine in fadeOutCoroutines)
        {
            yield return fadeOutCoroutine;
        }
    }

    IEnumerator FadeOutSingleObject(FadeObjectData data)
    {
        float currentAlpha = GetObjectAlpha(data.fadeObject);
        
        float timer = 0;
        while (timer < data.fadeOutDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / data.fadeOutDuration;
            float curveValue = data.fadeOutCurve.Evaluate(normalizedTime);
            float alpha = Mathf.Lerp(currentAlpha, data.startAlpha, curveValue);
            
            SetObjectAlpha(data.fadeObject, alpha);
            yield return null;
        }

        // Ensure final alpha is exactly the start value
        SetObjectAlpha(data.fadeObject, data.startAlpha);
        data.fadeObject.SetActive(false);
    }

    private void SetObjectAlpha(GameObject obj, float alpha)
    {
        // Handle UI Image components
        Image imageComponent = obj.GetComponent<Image>();
        if (imageComponent != null)
        {
            Color color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, alpha);
            imageComponent.color = color;
            return;
        }

        // Handle TextMeshPro - UI components
        TextMeshProUGUI tmpUIComponent = obj.GetComponent<TextMeshProUGUI>();
        if (tmpUIComponent != null)
        {
            Color color = new Color(tmpUIComponent.color.r, tmpUIComponent.color.g, tmpUIComponent.color.b, alpha);
            tmpUIComponent.color = color;
            return;
        }

        // Handle TextMeshPro - 3D components (if needed)
        TextMeshPro tmpComponent = obj.GetComponent<TextMeshPro>();
        if (tmpComponent != null)
        {
            Color color = new Color(tmpComponent.color.r, tmpComponent.color.g, tmpComponent.color.b, alpha);
            tmpComponent.color = color;
            return;
        }

        // Handle regular Text components (legacy)
        Text textComponent = obj.GetComponent<Text>();
        if (textComponent != null)
        {
            Color color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, alpha);
            textComponent.color = color;
            return;
        }

        // Handle CanvasGroup for more complex fading (affects all child elements)
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
            return;
        }
    }

    private float GetObjectAlpha(GameObject obj)
    {
        // Handle UI Image components
        Image imageComponent = obj.GetComponent<Image>();
        if (imageComponent != null)
            return imageComponent.color.a;

        // Handle TextMeshPro - UI components
        TextMeshProUGUI tmpUIComponent = obj.GetComponent<TextMeshProUGUI>();
        if (tmpUIComponent != null)
            return tmpUIComponent.color.a;

        // Handle TextMeshPro - 3D components
        TextMeshPro tmpComponent = obj.GetComponent<TextMeshPro>();
        if (tmpComponent != null)
            return tmpComponent.color.a;

        // Handle regular Text components (legacy)
        Text textComponent = obj.GetComponent<Text>();
        if (textComponent != null)
            return textComponent.color.a;

        // Handle CanvasGroup
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            return canvasGroup.alpha;

        return 1f; // Default alpha if no component found
    }

    private void LoadNextScenes()
    {
        gameManager.instance.ActiveUI = startupUIScene;
        gameManager.instance.ActiveLevel = startupLevelScene;
        
        SceneManager.LoadSceneAsync(startupUIScene, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(startupLevelScene, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(SplashScreens);
        SceneManager.UnloadSceneAsync(BlankScene);
    }
}