using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndySelector : subMenu
{
    [SerializeField] GameObject previewButtonPrefab;
    [SerializeField] GameObject previewPrefab;

    [SerializeField] GameObject ButtonContainer;
    [SerializeField] GameObject ViewContainer;

    [SerializeField] TMP_InputField saveNameInput;

    [SerializeField] BaseCar baseCar;


    private void Awake()
    {
       List<string> fileNames = gameManager.instance.DataManager().GetAllIndyModeSaves();

        for (int i  = 0; i < fileNames.Count; i++)
        {
            if (fileNames[i].EndsWith(".json"))
            {
                string name = fileNames[i].Substring(0, fileNames[i].Length - 5);
                CreateButton(name);
            }
            
        }

    }

    private void CreateButton(string file)
    {
        Vector2 pos = new Vector2(0, (ButtonContainer.transform.childCount * -100) - 50);

        GameObject newView = CreateView(file);
        GameObject button = Instantiate(previewButtonPrefab);

        button.transform.SetParent(ButtonContainer.transform);
        RectTransform rt = button.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;

        button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = file;
        button.GetComponent<Button>().onClick.AddListener(delegate { ToggleTab(newView); });
    }

    private GameObject CreateView(string fileName)
    {
        GameObject view = Instantiate(previewPrefab);
        view.transform.SetParent(ViewContainer.transform);
        
        basePreview script = view.GetComponent<basePreview>();
        if(script != null)
        {
            BaseCar car = gameManager.instance.DataManager().LoadIndyModeSave(fileName);
            script.SetNameText(fileName);
            script.SetMotorPowerText(car.engine.motorPower);
        }
        else
        {
            Debug.LogError("basePreview script not found on prefab");
        }
       
        RectTransform rt = view.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(160, 0);
        

        view.SetActive(false);
        return view;
    }

    public void CreateNewIndySave()
    {
        //TODO: Create a Robust Save Name Validation
        if(saveNameInput.text != "" && saveNameInput.text.Length > 3)
        {
            //Temp CODE
            BaseCar car = ScriptableObject.CreateInstance<BaseCar>();
            car.name = saveNameInput.text;
            car.engineID = 1;
            
            //car = gameManager.instance.DataManager().AssembleCar(carModel);
            if (gameManager.instance.DataManager().CreateNewSave_IndyMode(saveNameInput.text, ref car))
            {
                CreateButton(saveNameInput.text);
                GetCarData();
            }
        }
        else
        {
            saveNameInput.placeholder.GetComponent<Text>().text = "Name Rejected";
        }
    }

    public void GetCarData()
    {
        string name = activeView.name;
        gameManager.instance.DataManager().LoadIndyModeSave(name);
    }
}
