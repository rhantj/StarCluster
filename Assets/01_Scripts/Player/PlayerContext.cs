using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContext : MonoBehaviour
{
    public DefaultInput InputActions { get; set; }
    public Rigidbody2D Rb { get; private set; }
    public Animator Anim { get; set; }
    public int Facing { get; set; } = 1;
    public Vector2 Velocity
    {
        get { return Rb.velocity; }
        set { Rb.velocity = value; }
    }
    public Collider2D Col { get; private set; }
    public SpriteRenderer Renderer { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Col = GetComponent<Collider2D>();
        Renderer = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();
    }

    private void Start()
    {
        Rb.freezeRotation = true;
    }
}
