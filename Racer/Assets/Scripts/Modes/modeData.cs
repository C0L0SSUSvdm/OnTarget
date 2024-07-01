using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Mode", menuName = "Mode Data")]
public class modeData : ScriptableObject
{
    public string descriptionText;
    public string UIName;
    public string LevelName;
    public string ModeName;
    public Sprite thumbnail;
}