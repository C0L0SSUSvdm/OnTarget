using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Item : ScriptableObject
{
    public enum ItemType
    {
        Chasis,
        Engine,
        Tires,
        ShockAbsorbers,
        PowerSteering,
        Brakes,
        //Weapon,
        //Armor,
        //Consumable,
        //Ammo,
        //Material,
        //Quest,
        //Key,
        //Blueprint,
        //Junk
    }

    public string Name;
    public ItemType Type;
    public int Cost;
    public Sprite Background;
    public Sprite Icon;
    public string Description;
}