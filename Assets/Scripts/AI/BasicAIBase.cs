using Unity.Netcode;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class BasicAIBase : NetworkBehaviour
{
    [SerializeField] private EnemyData data;
    private float health;
    private float walkSpeed;
    private float runSpeed;

    protected void Initialize()
    {
        health = data.Health;
        walkSpeed = data.WalkSpeed;
        runSpeed = data.RunSpeed;
    }
    protected virtual void OnHit(float Damage)
    {
        Debug.Log("Took " + Damage + " Damage!");
        health -= Damage;
    }
}
