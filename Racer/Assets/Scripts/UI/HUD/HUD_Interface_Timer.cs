using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HUD_Interface_Director
{
    public void UpdateTimer(float time);
}

public class HUD_Interface_Timer : MonoBehaviour
{
    
    [Header("----- Found On Start -----")]
    [SerializeField] public GameObject activeTimer;
    [SerializeField] public int childIndex = 0;
   
    void Start()
    {
        HUD.Item.SetTimerInterface(gameObject);
        activeTimer = gameObject.transform.GetChild(childIndex).gameObject;
        activeTimer.SetActive(true);
    }
    
    public void SetActiveTimer()
    {
        activeTimer.SetActive(false);
        activeTimer = gameObject.transform.GetChild(childIndex).gameObject;
        activeTimer.SetActive(true);
    }

    public void UpdateTimer(float time)
    {
        activeTimer.GetComponent<HUD_Interface_Director>().UpdateTimer(time);
    }

}
