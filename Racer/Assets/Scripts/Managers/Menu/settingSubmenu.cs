using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settingSubmenu : subMenu
{
    [Header("----- Drag N Drops -----")]
    [SerializeField] GameObject VolumeView;
    [SerializeField] GameObject SettingsView;
    [SerializeField] GameObject ControlsView;

    public void ToggleVolumeTab()
    {
        ToggleTab(VolumeView);
    }
    public void ToggleSettingsTab()
    {
        ToggleTab(SettingsView);
    }
    public void ToggleControlsTab()
    {
        ToggleTab(ControlsView);
    }
    public void Close()
    {
        CloseActiveTab();
    }
}
