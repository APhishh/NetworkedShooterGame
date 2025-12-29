using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraScript : MonoBehaviour
{
    [SerializeField] float offset;
    [SerializeField] float visionRange;
    [SerializeField] float multiplier;
    [SerializeField] float verticalMultiplier;

    private GameObject player;
    public bool initialized;


    public void setup(GameObject playerObj)
    {
        player = playerObj;
        initialized = true;
    }
    void Update()
    {
        if (initialized)
        {
            Vector3 newpos = new Vector3(CalculatePos().x, CalculatePos().y);
            newpos.z -= offset;
            transform.position = newpos;
        }
    }

    private Vector2 CalculatePos()
    {

        InputAction mouseAction = InputSystem.actions.FindActionMap("Player").FindAction("Mouse");
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseAction.ReadValue<Vector2>());
        Vector2 playerPos = player.transform.position;
        Vector2 dir = (mousePos - playerPos).normalized;
        float dist = Vector2.Distance(mousePos, playerPos);
        float xPos = playerPos.x + Mathf.Clamp(dir.x * dist /5, -visionRange, visionRange) * multiplier;
        float yPos = playerPos.y + Mathf.Clamp(dir.y * dist/5, -visionRange, visionRange) * multiplier * verticalMultiplier;

        Vector2 finalPos = new Vector2(xPos, yPos);

        return finalPos;
    }

}
