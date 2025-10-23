using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Base")]
    protected string enemyName;
    [SerializeField] protected int hp;
    protected float moveSpeed = 3f;

    protected Rigidbody2D rb;
    protected Collider2D col;
    protected SpriteRenderer sr;
    protected Animator anim;

    protected float findRange = 5f;

    protected bool isAttack = false;

    public enum AnimationMap
    {
        Death,
        IsAttack
    }
    private static Dictionary<AnimationMap, int> hashMap = new();

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        foreach(var an in Enum.GetValues(typeof(AnimationMap)))
        {
            if (!hashMap.ContainsKey((AnimationMap)an))
            {
                hashMap[(AnimationMap)an] = Animator.StringToHash(an.ToString());
            }
        }
    }

    protected void Initialize(EnemyData data)
    {
        enemyName = data.enemyName;
        hp = data.hp;
        moveSpeed = data.moveSpeed;
    }

    public virtual void TakeDamage(int dmg)
    {
        hp -= dmg;
    }

    public int GetAnimationHash(AnimationMap anim) => hashMap[anim];

    public void EndDie()
    {
        transform.gameObject.SetActive(false);
    }

    public void EndAttack()
    {
        isAttack = false;
    }
}
