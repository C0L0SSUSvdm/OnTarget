using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//https://www.youtube.com/watch?v=232EqU1k9yQ
[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Database")]
public class SO_Item_Database : ScriptableObject, ISerializationCallbackReceiver
{
    public Item[] items;
    private Dictionary<uint, Item> itemMap = new Dictionary<uint, Item>();
    private Dictionary<Item, uint> idMap = new Dictionary<Item, uint>();


    public void OnAfterDeserialize()
    {
        Debug.Log("After Deserialize");
        itemMap = new Dictionary<uint, Item>();
        idMap = new Dictionary<Item, uint>();
        AddItemArrayToDictionary();

    }

    public void OnBeforeSerialize()
    {
        //Debug.Log("Before Serialize");
        //throw new System.NotImplementedException();
    }

    private void AddItemArrayToDictionary()
    {
        for (uint i = 0; i < items.Length; i++)
        {
            //Debug.Log($"count: {i}, this array: {items.Length}, total: { items.Length}");
            idMap.Add(items[i], i);
            itemMap.Add(i, items[i]);
        }
    }

    public Item GetItem(uint id)
    {
        //Debug.Log(GetItem[0]);
        return itemMap[id];
    }

    public uint GetID(Item item)
    {
        return idMap[item];
    }

    public int GetItemCounts()
    {
        return items.Length;
    }

}
