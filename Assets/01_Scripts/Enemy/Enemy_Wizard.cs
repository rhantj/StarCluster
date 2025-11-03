using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    State currentState;

    public Vector2 startPos;
    public Transform target;
    [SerializeField] GameObject projectile;
    int xDir = 1;

    protected override void Awake()
    {
        base.Awake();
        currentState = State.Patrol;
        startPos = transform.position;
        stateMachine = gameObject.AddComponent<StateMachine>();

        stateMachine.AddState(State.Patrol, new PatrolState(this));
        stateMachine.AddState(State.Attack, new AttacState(this));
        stateMachine.AddState(State.Return, new ReturnState(this));
        stateMachine.AddState(State.Die, new DieState(this));

        stateMachine.InitState(State.Patrol);
        Initialize(data);
    }

    private void OnEnable()
    {
        rb.isKinematic = false;
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        StopCoroutine(SetReference());
    }

    private void OnSceneChanged(Scene arg0, Scene arg1)
    {
        if (!arg1.name.Equals("Planet") || !gameObject.activeSelf) return;
        StartCoroutine(SetReference());
    }

    IEnumerator SetReference()
    {
        yield return null;

        if (target == null)
        {
            var obj = GameObject.FindGameObjectWithTag("Player").transform;
            if (obj != null)
            {
                target = obj;
            }
            else
            {
                target = null;
            }
        }
    }

    public void Initialize()
    {
        hp = 3;
        rb.isKinematic = false;
        col.isTrigger = false;
        currentState = State.Patrol;
        stateMachine.ChangeState(State.Patrol);
        StartCoroutine(SetReference());
    }

    public void SetStartPosition(Vector3 pos)
    {
        startPos = pos;
    }

    public override void TakeDamage(int dmg)
    {
        if (currentState == State.Die) return;
        base.TakeDamage(dmg);
        if (hp <= 0)
        {
            hp = 0;
            stateMachine.ChangeState(State.Die);
        }
    }

    public void StartAttack()
    {
        var pos = transform.position + new Vector3(0.9f * xDir, 0.33f, 0);
        ObjectPoolManager.Instance.SpawnFromPool("EnemyWizardProjectile", pos, out var obj);

        if(obj.TryGetComponent<WizardProjectile>(out var wp))
        {
            float x = target.position.x - transform.position.x;
            x = Mathf.Clamp01(x);
            if (x == 0) x--;
            wp.Fire(Vector2.right * x, 8f, xDir == -1);
        }
    }

    public void EndDie()
    {
        ObjectPoolManager.Instance.ReturnToPool("Enemy_Wizard", gameObject);
    }

    private class PatrolState : WizardState
    {
        public PatrolState(Enemy_Wizard owner) : base(owner) { }

        float moveDistance = 0f;

        public override void Enter()
        {
            owner.currentState = State.Patrol;
        }

        public override void FixedUpdate()
        {
            if (target == null) return;
            rb.velocity = new Vector2(xDir * moveSpeed, rb.velocity.y);
            moveDistance += moveSpeed * Time.fixedDeltaTime;

            if(moveDistance >= 5f)
            {
                xDir = -xDir;
                sr.flipX = xDir != 1;
                moveDistance = 0f;
            }
        }

        public override void Transition()
        {
            if (target == null) return;
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
        public override void Enter()
        {
            owner.currentState = State.Attack;
            rb.velocity = Vector2.zero;
            owner.isAttack = true;
        }

        public override void Update()
        {
            anim.SetBool(owner.GetAnimationHash(AnimationMap.IsAttack), owner.isAttack);
            var side = target.position.x - transform.position.x;
            sr.flipX = side < 0;
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
        float time = 0f;

        public override void Enter()
        {
            owner.currentState = State.Return;

            anim.SetBool(owner.GetAnimationHash(AnimationMap.IsAttack), owner.isAttack);

            if (rb.velocity.x < 0) xDir = -1;
            else xDir = 1;
            sr.flipX = xDir != 1;
        }

        public override void FixedUpdate()
        {
            time += Time.deltaTime;
            var dir = startPos - (Vector2)transform.position;
            dir = dir.normalized;

            rb.velocity = dir * moveSpeed;
        }

        public override void Transition()
        {
            if (Vector2.Distance(startPos, transform.position) <= 1f || time >=1f)
            {
                rb.velocity = Vector2.zero;
                time = 0f;
                ChangeState(State.Patrol);
            }
        }
    }

    private class DieState : WizardState
    {
        public DieState(Enemy_Wizard owner) : base(owner) { }
        public override void Enter()
        {
            owner.currentState = State.Die;

            var clearCtrl = GameObject.FindGameObjectWithTag("Canvas").GetComponent<ClearControl>();
            clearCtrl.MinusEnemyCount();
            rb.velocity = Vector2.zero;

            anim.SetInteger(owner.GetAnimationHash(AnimationMap.Death), owner.hp);
            rb.isKinematic = true;
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
