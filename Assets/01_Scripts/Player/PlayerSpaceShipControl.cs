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

    [Header("Scene load")]
    bool canEntry = false;
    SceneManagement scene;

    [Header("Upgrade")]
    UpgradeControl upgradeCtrl;

    private void Awake()
    {
        context = GetComponent<PlayerSpaceShipContext>();
        scene = GetComponent<SceneManagement>();

        context.InputActions = new DefaultInput();
        context.InputActions.Player.Move.performed += ReadMoveInput;
        context.InputActions.Player.Move.canceled += e => moveInput = Vector2.zero;
        context.InputActions.Player.Interaction.started += Interaction_started;
    }

    private void OnEnable()
    {
        context.InputActions.Player.Panel.started += ShowUpgradePanel;
        context.InputActions.Enable();
    }

    private void Start()
    {
        upgradeCtrl = GameManager.Instance.GetUpgradeCtrlUI();
    }

    private void OnDisable()
    {
        context.InputActions.Disable();
    }

    private void Update()
    {
        CalculateRotate();
    }

    private void FixedUpdate()
    {
        CalculateMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int planetMask = 1 << 7;
        int layer = 1 << collision.gameObject.layer;

        if ((planetMask & layer) != 0)
        {
            canEntry = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int planetMask = 1 << 7;
        int layer = 1 << collision.gameObject.layer;

        if ((planetMask & layer) != 0)
        {
            canEntry = false;
        }
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

    private void Interaction_started(InputAction.CallbackContext obj)
    {
        if (!canEntry || scene.isLoading) return;
        GameManager.Instance.SetSpaceShipPosition(transform.position);
        scene.LoadScene();
    }

    private void ShowUpgradePanel(InputAction.CallbackContext obj)
    {
        upgradeCtrl.Toggle();
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
