using Unity.Netcode;
using UnityEngine;

public class ItemPickup : NetworkBehaviour
{
    [SerializeField] ItemData data;


    public ItemData GetItemData()
    {
        return data;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnServerRpc()
    {
        NetworkObject itemNetworkObj = GetComponent<NetworkObject>();
        itemNetworkObj.Despawn();
    }

}
