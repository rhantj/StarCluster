using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class Enemy_Wizard : EnemyBase
{
    public EnemyData data;
    private StateMachine stateMachine;
    public enum State
    {
        Patrol,
        Attack,
        Return,
        Die
    }

    Vector2 startPos;
    Transform target;
    int xDir = 1;

    protected override void Awake()
    {
        base.Awake();
        startPos = transform.position;
        stateMachine = gameObject.AddComponent<StateMachine>();

        stateMachine.AddState(State.Patrol, new PatrolState(this));
        stateMachine.AddState(State.Attack, new AttacState(this));
        stateMachine.AddState(State.Return, new ReturnState(this));
        stateMachine.AddState(State.Die, new DieState(this));

        stateMachine.InitState(State.Patrol);
    }

    private void Start()
    {
        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        Initialize(data);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int mask = 1 << 6;
        int layer = 1 << collision.gameObject.layer;

        if ((mask & layer) != 0)
        {
            xDir = -xDir;
            sr.flipX = xDir != 1;
        }
    }

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        if(hp <= 0)
        {
            hp = 0;
            stateMachine.ChangeState(State.Die);
        }
    }

    private class PatrolState : WizardState
    {
        public PatrolState(Enemy_Wizard owner) : base(owner) { }

        public override void FixedUpdate()
        {
            rb.velocity = new Vector2(xDir * moveSpeed, rb.velocity.y);
        }

        public override void Transition()
        {
            if(Vector2.Distance(target.position, transform.position) <= findRange)
            {
                rb.velocity = Vector2.zero;
                ChangeState(State.Attack);
            }
        }
    }

    private class AttacState : WizardState
    {
        public AttacState(Enemy_Wizard owner) : base(owner) { }

        public override void Update()
        {
            anim.SetBool(owner.GetAnimationHash(AnimationMap.IsAttack), owner.isAttack);
            var side = target.position.x - transform.position.x;
            sr.flipX = side < 0;
        }

        public override void Enter()
        {
            rb.velocity = Vector2.zero;
            owner.isAttack = true;
        }

        public override void Transition()
        {
            if(Vector2.Distance(target.position, transform.position) > findRange)
            {
                owner.isAttack = false;
                ChangeState(State.Return);
            }
        }
    }

    private class ReturnState : WizardState
    {
        public ReturnState(Enemy_Wizard owner) : base(owner) { }

        public override void Enter()
        {
            anim.SetBool(owner.GetAnimationHash(AnimationMap.IsAttack), owner.isAttack);

            if (rb.velocity.x < 0) xDir = -1;
            else xDir = 1;
            sr.flipX = xDir != 1;
        }

        public override void FixedUpdate()
        {
            var dir = startPos - (Vector2)transform.position;
            dir = dir.normalized;

            rb.velocity = dir * moveSpeed;
        }

        public override void Transition()
        {
            if (Vector2.Distance(startPos, transform.position) <= 1f)
            {
                rb.velocity = Vector2.zero;
                ChangeState(State.Patrol);
            }
        }
    }

    private class DieState : WizardState
    {
        public DieState(Enemy_Wizard owner) : base(owner) { }
        public override void Enter()
        {
            rb.velocity = Vector2.zero;

            anim.SetInteger(owner.GetAnimationHash(AnimationMap.Death), owner.hp);
            col.isTrigger = true;
        }
    }

    private class WizardState : BaseState
    {
        public Enemy_Wizard owner;
        public WizardState(Enemy_Wizard owner)
        {
            this.owner = owner;
        }

        protected Transform target => owner.target;
        protected Transform transform => owner.transform;
        protected float findRange => owner.findRange;
        protected Rigidbody2D rb => owner.rb;
        protected float moveSpeed => owner.moveSpeed;
        protected float xDir
        {
            get { return owner.xDir; }
            set { owner.xDir = (int)value; }
        }
        protected Vector2 startPos => owner.startPos;
        protected Animator anim => owner.anim;
        protected Collider2D col => owner.col;
        protected SpriteRenderer sr => owner.sr;
    }
}
