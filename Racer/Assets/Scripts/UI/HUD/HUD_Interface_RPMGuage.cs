using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HUD_RPMGuage_Director
{
    public void UpdateRPMGuage(int value);
}

public class HUD_Interface_RPMGuage : MonoBehaviour
{
    [Header("Found On Start")]
    [SerializeField] private GameObject defaultRPMGuage;

    [Header("----- Fields ------")]
    [SerializeField] private GameObject activeRPMGuage;
    [SerializeField] private int childIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        HUD.Item.SetRPMGuageInterface(gameObject);
        activeRPMGuage = gameObject.transform.GetChild(childIndex).gameObject;
        activeRPMGuage.SetActive(true);


    }
    public void SetRPMGuage()
    {
        activeRPMGuage.SetActive(false);
        activeRPMGuage = gameObject.transform.GetChild(childIndex).gameObject;
        activeRPMGuage.SetActive(true);
    }
    
    public void UpdateRPMGuage(int value)
    {
        activeRPMGuage.GetComponent<HUD_RPMGuage_Director>().UpdateRPMGuage(value);
    }
}

