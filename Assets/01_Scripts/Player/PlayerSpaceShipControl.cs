using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class PlayerSpaceShipControl : MonoBehaviour
{
    PlayerSpaceShipContext context;

    [Header("Movement")]
    float moveSpeed = 5f;
    public Vector2 moveInput;

    [Header("Scene load")]
    bool isLoading;
    bool canEntry = false;
    AssetReference planet;

    private void Awake()
    {
        context = GetComponent<PlayerSpaceShipContext>();

        context.InputActions = new DefaultInput();
        context.InputActions.Player.Move.performed += ReadMoveInput;
        context.InputActions.Player.Move.canceled += e => moveInput = Vector2.zero;
        context.InputActions.Player.Interaction.started += Interaction_started;
    }

    private void OnEnable()
    {
        context.InputActions.Enable();
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


        if (collision.gameObject.TryGetComponent<PlanetEntry>(out var p))
        {
            planet = p.planetScene;
        }

        if ((planetMask & layer) != 0)
        {
            Debug.Log("Enty [Space]");
            canEntry = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int planetMask = 1 << 7;
        int layer = 1 << collision.gameObject.layer;

        planet = null;

        if ((planetMask & layer) != 0)
        {
            Debug.Log("Enty [Space]");
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
        if (!canEntry || isLoading) return;
        LoadPlanetScene();
    }

    void LoadPlanetScene()
    {
        isLoading = true;

        var load = 
            Addressables.LoadSceneAsync(planet, LoadSceneMode.Single, activateOnLoad: false);

        load.Completed += OnSceneLoaded;
    }

    void OnSceneLoaded(AsyncOperationHandle<SceneInstance> scene)
    {
        if (scene.Status == AsyncOperationStatus.Succeeded)
        {
            var activate = scene.Result.ActivateAsync();
            activate.completed += e => isLoading = false;
        }
        else
        {
            Debug.LogError("Scene load fail");
            isLoading = false;
        }
    }
}
