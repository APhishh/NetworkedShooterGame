using Unity.Netcode;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    private GameObject playerCamera;
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            setupPlayer();
        }
    }

    public void setupPlayer()
    {
        playerCamera = Camera.main.gameObject;
        CameraScript camFollow = playerCamera.GetComponent<CameraScript>();
        camFollow.setup(gameObject);
    }
    
    void Update()
    {
        
    }
}
