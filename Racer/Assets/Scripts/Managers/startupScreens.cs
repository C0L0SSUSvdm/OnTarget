using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//This Script is Loaded by the gameManager.cs
//After the Startup Splash screens are done, this script intializes the GameManager Settings to run for the title screen.
public class startupScreens : MonoBehaviour
{

    [SerializeField] public Image ActiveImage;
    [SerializeField] public int index = 0;
    [SerializeField] public int SlideCount = 3;

    [SerializeField] public Image SlideOne;
    [SerializeField] public Image SlideTwo;
    [SerializeField] public Image SlideThree;

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
        ActiveImage = gameObject.transform.GetChild(index).gameObject.GetComponent<Image>();
        ActiveImage.gameObject.SetActive(true);
        StartCoroutine(TransitionSlide());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowSlideTimer = maxslideTime;
        }
    }


    IEnumerator TransitionSlide()
    {

        while (FadeInTimer < maxFadeTime)
        {
            FadeInTimer += Time.deltaTime;
            float ratio = FadeInTimer / maxFadeTime;
            Color color = new Color(ActiveImage.color.r, ActiveImage.color.g, ActiveImage.color.b, Mathf.Lerp(0, 1, ratio));
            ActiveImage.color = color;

            yield return null;
        }

        while (ShowSlideTimer < maxslideTime)
        {
            
            ShowSlideTimer += Time.deltaTime;

            yield return null;
        }

        while (FadeOutTimer > maxFadeTime * 0.5f) // Doesn't Work Correctly yet
        {
            FadeOutTimer += Time.deltaTime;
            float ratio = FadeOutTimer / maxFadeTime * 0.5f;
            Color color = new Color(ActiveImage.color.r, ActiveImage.color.g, ActiveImage.color.b, Mathf.Lerp(0, 1, ratio));
            ActiveImage.color = color;
            yield return null;
        }
        index++;
        if (index < SlideCount)
        {
            FadeInTimer = 0.0f;
            FadeOutTimer = 0.0f;
            ShowSlideTimer = 0.0f;
            ActiveImage.gameObject.SetActive(false);
            ActiveImage = gameObject.transform.GetChild(index).gameObject.GetComponent<Image>();
            ActiveImage.gameObject.SetActive(true);           
            StartCoroutine(TransitionSlide());
        }
        else
        {
            gameManager.instance.ActiveUI = startupUIScene;
            gameManager.instance.ActiveLevel = startupLevelScene;
            //gameManager.instance.SelectedUI = "GamePlayUI";
            //gameManager.instance.SelectedLevel = "DebugRoom2";
            SceneManager.LoadSceneAsync(startupUIScene, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync(startupLevelScene, LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync(SplashScreens);
            SceneManager.UnloadSceneAsync(BlankScene);

            StartCoroutine(LoadingProgress());
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
