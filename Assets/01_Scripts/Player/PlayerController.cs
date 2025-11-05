using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : ReplayRecorder, IRewindable
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
    bool canOpen = false;
    IInteractable currentInteractable;

    [Header("Fire")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform firePoint;
    public bool isFire { get; set; } = false;

    [Header("Get damage")]
    int getDmg = 0;

    [SerializeField] private int _stackCount;
    [SerializeField] private bool _isRecordingView;
    [SerializeField] private bool _isRewindingView;

    public int STACKCOUNT => _stackCount;
    public bool ISRECORDING => _isRecordingView;
    public bool ISREWINDING => _isRewindingView;

    protected override void Awake()
    {
        base.Awake();
        plc = GetComponent<PlayerContext>();
        //jumpPower *= 1.5f;

        plc.InputActions = new DefaultInput();
        plc.InputActions.Player.Move.performed += MoveInput;
        plc.InputActions.Player.Move.canceled += _ => moveInput = Vector2.zero;
        plc.InputActions.Player.Jump.started += JumpPressed;
        plc.InputActions.Player.Interaction.started += Interaction_started;
        plc.InputActions.Player.Fire.started += Fire_started;

        plc.InputActions.Time.Rewind.started += HandleRewind;
    }

    private void HandleRewind(InputAction.CallbackContext obj)
    {
        if (IsRewinding)
        {
            RewindableManager.Instance?.StopRewind();
        }
        else
        {
            RewindableManager.Instance?.StartRewind();
        }
    }

    private void OnEnable()
    {
        plc.InputActions.Enable();
    }

    void OnDisable()
    {
        StopRecording();
        StopPlaybackAndClearFrames(true);
    }

    private void OnDestroy()
    {
        plc.InputActions.Disable();
    }

    private void Update()
    {
        if(!IsRewinding)
            UpdateFacing();

        _stackCount = recordedFrames.Count;
        _isRecordingView = IsRecording;
        _isRewindingView = IsRewinding;
    }

    private void FixedUpdate()
    {
        if (!IsRewinding)
        {
            CalculateJump();
            CalculateMovement();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractable>(out var interactable))
        {
            currentInteractable = interactable;
            canOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentInteractable != null && collision.gameObject == ((MonoBehaviour)currentInteractable).gameObject)
        {
            currentInteractable = null;
            canOpen = false;
        }
    }

    private void MoveInput(InputAction.CallbackContext obj)
    {
        moveInput = obj.ReadValue<Vector2>();
    }

    private void JumpPressed(InputAction.CallbackContext obj)
    {
        jumpPressed = true;
    }

    private void Interaction_started(InputAction.CallbackContext obj)
    {
        if(canOpen && obj.started && currentInteractable != null)
        {
            currentInteractable.OnInteraction(this);
        }
    }

    private void Fire_started(InputAction.CallbackContext obj)
    {
        if (IsRewinding) return;
        isFire = true;
    }

    public void TakeDamage()
    {
        if (getDmg >= 99)
        {
            plc.Velocity = Vector2.zero;
            ObjectPoolManager.Instance.ReturnToPool("Player", gameObject);
        }
        getDmg++;
    }

    public void PlayerInit()
    {
        getDmg = 0;
        plc.Velocity = Vector2.zero;
        StartRecording();
    }

    void CalculateMovement()
    {
        plc.Velocity = new Vector2(moveInput.x * moveSpeed, plc.Velocity.y);
    }

    void CalculateJump()
    {
        if (jumpPressed && IsGrounded())
        {
            plc.Rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpPressed = false;
        }
    }

    void UpdateFacing()
    {
        if (moveInput.x >= 0.01f) plc.Facing = 1;
        else if (moveInput.x < 0f) plc.Facing = -1;

        if (moveInput == Vector2.zero) return;
        plc.Renderer.flipX = plc.Facing != 1;

        firePoint.position = transform.position + Vector3.right * plc.Facing;
    }

    public bool IsGrounded()
    {
        var hit = Physics2D.BoxCast(groundPivot.position, boxSize, angle, Vector2.down, distance, groundMask);
        return hit.collider != null;
    }

    public void EndFire()
    {
        isFire = false;
    }

    public void StartFire()
    {
        ObjectPoolManager.Instance.SpawnFromPool("PlayerProjectile", firePoint.position, out var p);
        SoundManager.Instance.PlaySFX("Player_Fire", transform.position, 0.5f);
        p.GetComponent<PlayerProjectile>().Fire(Vector3.right * plc.Facing, 11f, plc.Facing != 1);
    }

    public void Rewind()
    {
        plc.Rb.bodyType = RigidbodyType2D.Kinematic; 
        StartReversePlayBack();
    }

    public void StopRewind()
    {
        plc.Rb.bodyType = RigidbodyType2D.Dynamic;
        StartRecording();
    }
}
