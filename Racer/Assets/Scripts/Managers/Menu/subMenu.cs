using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class subMenu : MonoBehaviour
{
    

    [Header("----- Members -----")]

    [SerializeField] GameObject activeView;

    protected void ToggleTab(GameObject obj)
    {
        if (activeView == obj)
        {
            CloseActiveTab();
        }
        else
        {
            if (activeView != null)
            {
                activeView.SetActive(false);
            }
            activeView = obj;
            activeView.SetActive(true);
        }
    }

    protected void CloseActiveTab()
    {
        activeView.SetActive(false);
        activeView = null;
    }


}
