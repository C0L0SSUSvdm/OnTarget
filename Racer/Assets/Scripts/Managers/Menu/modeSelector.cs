using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class modeSelector : MonoBehaviour
{
    [SerializeField] List<modeData> modes;
    [SerializeField] int selectedModeIndex;

    [SerializeField] Image ThumbnailImage;
    [SerializeField] TextMeshProUGUI DescriptionText;

    private void Start()
    {
        UpdateViewSprite();
        SetTargetChildName();
        //UpdateScenes();
    }

    public void IterateLeft()
    {
        if (selectedModeIndex == 0)
        {
            selectedModeIndex = modes.Count - 1;
        }
        else
        {
            selectedModeIndex--;
        }
        UpdateViewSprite();
        //UpdateScenes();
    }

    public void IterateRight()
    {
        if(selectedModeIndex == modes.Count - 1)
        {
            selectedModeIndex = 0;
        }
        else
        {
            selectedModeIndex++;
        }
        UpdateViewSprite();
        //UpdateScenes();
    }

    public void SetTargetChildName()
    {
        gameManager.instance.GetMenuScript().SetSubmMenuPlaceHolderValue(modes[selectedModeIndex].ModeName);
    }

    public void SetSelectedScenes()
    {
        gameManager.instance.SelectedUI = modes[selectedModeIndex].UIName;
        gameManager.instance.SelectedLevel = modes[selectedModeIndex].LevelName;
    }

    //void UpdateScenes()
    //{
    //    gameManager.instance.SelectedUI = modes[selectedModeIndex].UIName;
    //    gameManager.instance.SelectedLevel = modes[selectedModeIndex].LevelName;
    //}

    void UpdateViewSprite()
    {
        ThumbnailImage.overrideSprite = modes[selectedModeIndex].thumbnail;
        DescriptionText.text = modes[selectedModeIndex].descriptionText;
    }

}
