using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using System;

public class PlayerInventory : NetworkBehaviour
{

    public struct item : INetworkSerializable, IEquatable<item>
    {
        public int id;
        public int quantity;
        public bool Equals(item other) => id == other.id;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
        }
    }

    [SerializeField] private NetworkList<item> networkInventory = new NetworkList<item>();
    private List<ItemClass> localInventory = new List<ItemClass>();
    private NetworkVariable<item> Equipped = new NetworkVariable<item>();
    private GameObject itemPickups;
    private List<GameObject> pickupsInRange = new List<GameObject>();
    private Camera mainCam;
    void Awake()
    {
        itemPickups = GameObject.Find("ItemPickups");
        mainCam = Camera.main;
    }


    private void CheckPickup()
    {
        GameObject nearestObj = null;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        foreach (GameObject obj in pickupsInRange)
        {
            if (nearestObj == null)
            {
                nearestObj = obj;
            }
            else
            {
                if (Vector2.Distance(obj.transform.position, mousePos) < Vector2.Distance(nearestObj.transform.position, mousePos))
                {
                    nearestObj = obj;
                }
            }
        }

        if (nearestObj == null) return;
        if (Input.GetKeyDown(KeyCode.E)) PickupItem(nearestObj);

    }
    
    
    private void PickupItem(GameObject itemObj)
    {
        ItemPickup pickupScript = itemObj.GetComponent<ItemPickup>();
        ItemData pickedupItemData = pickupScript.GetItemData();

        ItemClass newItemClass = new ItemClass();
        newItemClass.itemData = pickedupItemData;
        localInventory.Add(newItemClass);

        item newItem = new item();
        newItem.id = pickedupItemData.itemId;
        newItem.quantity = 1;
        AddToInventoryServerRpc(newItem);


        pickupScript.DespawnServerRpc();

    }

    [ServerRpc(RequireOwnership = false)]
    private void AddToInventoryServerRpc(item newItem)
    {
        Equipped.Value = newItem;
        networkInventory.Add(newItem);
    }

    public ItemData getEquipped()
    {
        if (InventoryManager.GetItemFromDatabase(Equipped.Value.id) == null) return null;
        return InventoryManager.GetItemFromDatabase(Equipped.Value.id);
    }

    void Update()
    {
        if (!IsOwner) return;

        pickupsInRange.Clear();
        foreach (Transform item in itemPickups.transform)
        {

            if (!item.gameObject.activeSelf) continue;

            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(item.position, mousePos) < 2f)
            {
                
                pickupsInRange.Add(item.gameObject);
            }
        }

        CheckPickup();

    }


    

}
