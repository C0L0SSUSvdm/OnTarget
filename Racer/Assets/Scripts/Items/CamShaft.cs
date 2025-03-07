using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EngineBlock", menuName = "Inventory System/Items/CamShaft")]
public class CamShaft : Item
{

    [Range(100, 400)] public float IgnitionsPerSecond;//200
    //[Range(0.1f, 1.0f)] public float CamShaftRadius;// length = pi r  //Don't remember what this was used for, Guess it was factored out
    [Range(0.5f, 1.0f)] public float IgnitionTimingEfficiency;
}
