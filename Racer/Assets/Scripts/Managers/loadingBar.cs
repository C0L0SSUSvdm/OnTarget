using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadingBar : MonoBehaviour
{
    [SerializeField] public Image ImageFillBar;

    public void UpdateFillBar(float ratio)
    {
        ImageFillBar.fillAmount = ratio;
    }
}
