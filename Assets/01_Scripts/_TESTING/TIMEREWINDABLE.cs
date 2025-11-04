using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TIMEREWINDABLE : ReplayRecorder
{
    DefaultInput InputActions;

    [Header("Movement")]
    float moveSpeed = 10f;
    Vector2 moveInput;
    public int COUNT;

    protected override void Awake()
    {
        base.Awake();
        InputActions = new DefaultInput();
        InputActions.Player.Move.performed += MoveInput;
        InputActions.Player.Move.canceled += _ => moveInput = Vector2.zero;
    }

    private void Start()
    {
        StartRecording();

        Invoke(nameof(StartReversePlayBack), 5f);
    }

    void OnEnable() => InputActions.Enable();
    void OnDisable() => InputActions.Disable();

    void OnDestroy()
    {
        InputActions.Player.Move.performed -= MoveInput;
        InputActions?.Dispose();
    }

    private void FixedUpdate()
    {
        COUNT = recordedFrames.Count;
        if (IsRewinding) return;
        CalculateMovement();
    }

    private void MoveInput(InputAction.CallbackContext obj)
    {
        moveInput = obj.ReadValue<Vector2>();
    }

    void CalculateMovement()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
    }
}
