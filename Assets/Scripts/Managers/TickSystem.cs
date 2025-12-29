using UnityEngine;

public class TickSystem : MonoBehaviour
{
    public static int PhysicsTick;



    void FixedUpdate()
    {
        PhysicsTick = Mathf.FloorToInt((float)Time.timeAsDouble / Time.fixedDeltaTime);
    }

}
