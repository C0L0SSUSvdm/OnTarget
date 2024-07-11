using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menu : MonoBehaviour
{
    [Header("-----SubMenus-----")]
    [SerializeField] GameObject ActiveObject;
    [SerializeField] GameObject MainMenu;
    [Tooltip("A Placeholder to Dynamically Search Child submenu by name")]
    [SerializeField] string childName;

    public void Awake()
    {
        gameManager.instance.SetMenuObject(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

        if (ActiveObject == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }

    }

    public bool UpdateMenuState()
    {

        if (ActiveObject == null)
        {
            MainMenu.transform.parent.gameObject.SetActive(true);
            MainMenu.SetActive(true);
            ActiveObject = MainMenu;
        }
        else
        {
            if (ActiveObject == MainMenu)
            {
                gameObject.SetActive(false);
                ActiveObject = null;
                MainMenu.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                ExitSubMenu();
            }

        }

        return (ActiveObject == null) ? false : true;
    }

    public void OpenSubMenu(GameObject submenu)
    {
        ActiveObject.SetActive(false);
        ActiveObject = submenu;
        ActiveObject.SetActive(true);
    }

    public void ExitSubMenu()
    {
        ActiveObject.SetActive(false);
        ActiveObject = MainMenu;
        ActiveObject.SetActive(true);
    }

    public void SetSubmMenuPlaceHolderValue(string submenuName)
    {
        childName = submenuName;
    }

    public void OpenSubMenuByName()
    {
        OpenSubMenu(gameObject.transform.Find(childName).gameObject);
    }
}
