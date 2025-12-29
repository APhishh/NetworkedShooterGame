using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using System.Linq;
using NUnit.Framework;
public class EnemyProperties : BasicAIBase
{
    [SerializeField] private bool enableLagCompensation;
    private Queue<(int tick, Vector2 pos)> positionHistory = new Queue<(int, Vector2)>();
    private int maxTick;
    void Start()
    {
        Initialize(); //Init enemyData
        maxTick = (int)Mathf.Floor(2 / Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (IsServer)
        {
            positionHistory.Enqueue((TickSystem.PhysicsTick, transform.position));


            if (positionHistory.Count > 0 && TickSystem.PhysicsTick - positionHistory.Peek().tick > maxTick)
            {
                positionHistory.Dequeue();
            }    
        }
    }

    #region Lag Compensation

    [ServerRpc (RequireOwnership = false)]
    public void ReceiveHitServerRpc(Vector2 HitPos, float Ping, ulong id, float Damage)
    {  
        if (enableLagCompensation && !IsServer)
        {
            int rewindTicks = (int)Mathf.Floor((Ping/1000 * 0.5f) / Time.fixedDeltaTime);
            Vector2 currentPos = transform.position;
            
            foreach (var entry in positionHistory)
            {
                if (entry.tick == TickSystem.PhysicsTick - rewindTicks)
                {
                    
                    if (Vector2.Distance(entry.pos, transform.position) < 3f) OnHit(Damage);
                }
            }
        }
        else
        {
            OnHit(Damage);
        }
    }

    #endregion
}


