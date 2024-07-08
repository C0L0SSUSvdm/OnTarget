using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class basePreview : MonoBehaviour
{
    private string fileName;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI MotorPowerText;

    public void SetNameText(string name)
    {
        nameText.text = name;
    }

    public void SetMotorPowerText(float power)
    {
        MotorPowerText.text = power.ToString();
    }
}
