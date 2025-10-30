using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpaceShipContext : MonoBehaviour
{
    public DefaultInput InputActions { get; set; }
    public Rigidbody2D Rb { get; private set; }
    public Vector2 Velocity
    {
        get { return Rb.velocity; }
        set {  Rb.velocity = value; }
    }
    public Collider2D Col { get; private set; }
    public SpriteRenderer Renderer { get; private set; }

    private void Awake()
    {
        InputActions = new DefaultInput();
        Rb = GetComponent<Rigidbody2D>();
        Col = GetComponent<Collider2D>();
        Renderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        Rb.freezeRotation = true;
    }
}
