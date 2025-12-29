using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
public class PlayerMovement : NetworkBehaviour
{

    [Header("Movement Fields")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Rigidbody2D rb;

    #region Input Actions

    private InputAction m_moveAction;
    private InputAction m_sprint;
    #endregion

    private Vector2 movementVel;

    private void EnableMovement()
    {
        Debug.Log(inputActions.FindActionMap("Player"));
        inputActions.FindActionMap("Player").Enable();
    }
    private void DisableMovement()
    {
        inputActions.FindActionMap("Player").Disable();
    }


    private void Awake()
    {
        m_moveAction = inputActions.FindAction("Move");
        m_sprint = inputActions.FindAction("Sprint");
        
    }
    void FixedUpdate()
    {
        if (!IsOwner) return;
        movementVel = m_moveAction.ReadValue<Vector2>();

        if (CheckSprint())
        {
            Sprint();
        }
        else
        {
            Move();
        }

    }

    private bool CheckSprint()
    {
        bool canSprint = false;

        if (m_sprint.ReadValue<float>() > 0f)
        {
            canSprint = true;
        }

        return canSprint;
    }

    private void Move()
    {
        rb.MovePosition(rb.position + (movementVel * walkSpeed) * Time.fixedDeltaTime);
    }

    private void Sprint()
    {

        rb.MovePosition(rb.position + (movementVel * runSpeed) * Time.fixedDeltaTime);
    }
}
