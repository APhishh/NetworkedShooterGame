using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class BasicMeleeEnemy : BasicAIBase
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

    public void ReceiveHit(Vector2 HitPos, float Damage)
    {
        if (enableLagCompensation)
        {
            
        }
        else
        {
            OnHit(Damage);
        }
    }

    #endregion
}
