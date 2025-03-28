using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Piston", menuName = "Inventory System/Items/Piston")]
public class Piston : Item
{
    
    [Range(4000, 14000)] public int MaximumForceRating_N;
    [Range(0.1f, 0.3f)] public float ThicknessCoefficient_CM;
    [Range(1500, 3000)] public float Mass_M3_KG;
}
