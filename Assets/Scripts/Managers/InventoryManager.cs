using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : NetworkBehaviour
{

    [SerializeField] private List<ItemData> itemList = new List<ItemData>();

    public static Dictionary<int, ItemData> itemDatabase = new Dictionary<int, ItemData>();
    public static UnityEvent<ulong> ListInventory = new UnityEvent<ulong>();


    void Awake()
    {
        foreach (ItemData data in itemList)
        {
            itemDatabase.Add(data.itemId, data);
        }

    }

    void Update()
    {

    }


    public static ItemData GetItemFromDatabase(int id)
    {
        if (!   itemDatabase.ContainsKey(id))
        {
            return null;
        }

        return itemDatabase[id];
    }




}
