using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deactivateOnStart : MonoBehaviour
{
    // I put this here because on awake and on start isn't called until the object is active
    // the gameManager needs to know about the hud 
    private void Start()
    {
        gameObject.SetActive(false);
    }

}
