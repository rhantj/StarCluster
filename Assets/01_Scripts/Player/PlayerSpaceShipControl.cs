using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpaceShipControl : MonoBehaviour
{
    PlayerSpaceShipContext context;

    [Header("Movement")]
    float moveSpeed = 5f;
    public Vector2 moveInput;

    public Vector2 MoveSpector;

    private void Awake()
    {
        context = GetComponent<PlayerSpaceShipContext>();

        context.InputActions.Player.Move.performed += ReadMoveInput;
        context.InputActions.Player.Move.canceled += e => moveInput = Vector2.zero;
    }

    private void Update()
    {
        CalculateRotate();
    }

    private void FixedUpdate()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        float x = Mathf.Clamp(context.Velocity.x, -moveSpeed, moveSpeed);
        float y = Mathf.Clamp(context.Velocity.y, -moveSpeed, moveSpeed);

        if (moveInput == Vector2.zero)
        {
            x = Mathf.Lerp(x, 0f, moveSpeed * Time.deltaTime);
            y = Mathf.Lerp(y, 0f, moveSpeed * Time.deltaTime);
        }

        context.Velocity = new Vector2
            (
                moveSpeed * moveInput.x + x, 
                moveSpeed * moveInput.y + y
            );

        
        MoveSpector = context.Velocity;
    }

    void CalculateRotate()
    {
        if (moveInput == Vector2.zero) return;
        float x = moveInput.x;
        float y = moveInput.y;
        float z = Mathf.Atan2(y, x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, z), moveSpeed * Time.deltaTime);
    }

    private void ReadMoveInput(InputAction.CallbackContext obj)
    {
        moveInput = obj.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        context.InputActions.Enable();
    }
}
