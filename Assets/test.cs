using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
public class Test : NetworkBehaviour
{
    public float speed = 5f;
    public float switchTime = 2f; // seconds before switching direction

    private Rigidbody2D rb;
    private int direction = 1;
    private float timer;
    public static int CurrentTick { get; private set; }


    // Server keeps a history of positions by tick
    private Queue<(int tick, Vector2 pos)> history = new Queue<(int, Vector2)>();

    void Start()
    {

        
        rb = GetComponent<Rigidbody2D>();
    }

   void FixedUpdate()
{
    var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;

    // Calculate authoritative tick from elapsed time
    int newTick = Mathf.FloorToInt((float)(Time.timeAsDouble / Time.fixedDeltaTime));

    if (IsServer)
    {
        history.Enqueue((newTick, transform.position));

        // keep ~2 seconds of history 
        int maxHistoryTicks = Mathf.RoundToInt(2f / Time.fixedDeltaTime);

        while (history.Count > 0 && newTick - history.Peek().tick > maxHistoryTicks)
        {
            history.Dequeue();
            
        }
    }
}

    void Update()
    {
        
      
        if (!IsServer && NetworkManager.Singleton.NetworkConfig.NetworkTransport != null)
        {
           // Debug.Log(NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId));

        }
        if (!IsServer) return;
        
        // Move continuously
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        // Timer for switching direction
        timer += Time.deltaTime;
        if (timer >= switchTime)
        {
            direction *= -1; // flip direction
            timer = 0f;
        }

        // Tell clients about velocity
        MoveBoxClientRPC(direction);
    }

    public void GetShot(Vector2 hitPos)
    {
        if (IsHost) return;
        Debug.Log("What");
        OnShotServerRpc((int)NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId), hitPos);
    }

    [ClientRpc]
    private void MoveBoxClientRPC(int dir)
    {
        if (IsHost) return;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnShotServerRpc(int ping, Vector2 bulletPos)
    {
        Vector2 foundPos = Vector2.zero;

        // Convert ping to seconds (on-way)
        float oneWaySeconds = (ping * 0.5f) / 1000f;

        // Convert seconds to ticks
        int rewindTicks = Mathf.RoundToInt(oneWaySeconds / Time.fixedDeltaTime);

        // Determine which tick we need to rewind to
        int targetTick = history.Last().tick - rewindTicks;

        // Find matching entry
        foreach (var entry in history)
        {
            if (entry.tick == targetTick)
            {
                foundPos = entry.pos;
                break;
            }
        }
    
        if (Vector2.Distance(bulletPos, foundPos) < 3f)
        {
            
            Debug.Log("HIT");
        }
    }

    
    
}
