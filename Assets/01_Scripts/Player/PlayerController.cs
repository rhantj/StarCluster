using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    PlayerContext plc;

    [Header("Movement")]
    float moveSpeed = 5f;
    Vector2 moveInput;

    [Header("Jump")]
    [SerializeField] Transform groundPivot;
    float jumpPower = 10f;
    int groundMask = 1 << 6;
    bool jumpPressed = false;
    float distance = 0.1f;
    Vector2 boxSize = new Vector2(.7f, 0.1f);
    float angle = 0f;

    [Header("Interaction")]
    bool canEntry = false;

    [Header("Scene")]
    SceneManagement scene;

    private void Awake()
    {
        plc = GetComponent<PlayerContext>();
        scene = GetComponent<SceneManagement>();
        //jumpPower *= 1.5f;

        plc.InputActions = new DefaultInput();
        plc.InputActions.Player.Move.performed += MoveInput;
        plc.InputActions.Player.Move.canceled += e => moveInput = Vector2.zero;
        plc.InputActions.Player.Jump.started += JumpPressed;
        plc.InputActions.Player.Interaction.started += Interaction_started;
    }

    private void OnEnable()
    {
        plc.InputActions.Enable();
    }

    private void OnDestroy()
    {
        plc.InputActions.Disable();
    }

    private void Update()
    {
        UpdateFacing();
    }

    private void FixedUpdate()
    {
        CalculateMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var tag = collision.tag;
        ChangeEntry(tag);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var tag = collision.tag;
        ChangeEntry(tag);
    }

    void ChangeEntry(string tag)
    {
        if (tag.Equals("ReturnPoint"))
        {
            canEntry = !canEntry;
        }
    }

    private void MoveInput(InputAction.CallbackContext obj)
    {
        moveInput = obj.ReadValue<Vector2>();
    }


    private void Interaction_started(InputAction.CallbackContext obj)
    {
        if (!canEntry || scene.isLoading || !obj.started) return;
        scene.LoadScene();
    }

    void CalculateMovement()
    {
        plc.Velocity = new Vector2(moveInput.x * moveSpeed, plc.Velocity.y);
    }

    void UpdateFacing()
    {
        if (moveInput.x >= 0.01f) plc.Facing = 1;
        else if (moveInput.x < 0f) plc.Facing = -1;

        if (moveInput == Vector2.zero) return;
        plc.Renderer.flipX = plc.Facing != 1;
    }

    private void JumpPressed(InputAction.CallbackContext obj)
    {
        jumpPressed = true;
        if (jumpPressed && IsGrounded())
        {
            plc.Rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpPressed = false;
        }
    }

    public bool IsGrounded()
    {
        var hit = Physics2D.BoxCast(groundPivot.position, boxSize, angle, Vector2.down, distance, groundMask);
        return hit.collider != null;
    }
}
