using System.Buffers;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour, IDamagable
{
    [SerializeField] public bool DebugMode;

    [SerializeField] public bool isGround;
    [SerializeField] public bool isSlope;

    [Header("Components")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] public Rigidbody2D rigid;
    [SerializeField] public Transform frontChecker;
    [SerializeField] public Animator animator;
    [SerializeField] public PlayerEffectController effect;
    [SerializeField] public PlayerAttacker attacker;
    [SerializeField] public PlayerInput playerInput;


    [SerializeField] PlayerStatus data;

    [Header("Base Status")]
    [SerializeField] public float moveSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] public float breakSpeed;
    [SerializeField] public float jumpSpeed;

    [SerializeField] int maxJumpCount;
    [SerializeField] int jumpCount;
    
    [SerializeField] int maxHp;
     public int MaxHP { get { return maxHp; } set { maxHp = value; OnChangeHP.Invoke(maxHp, hp); } }
    [SerializeField] int hp;
     public int HP { get { return hp; } set { if (value >= MaxHP) hp = maxHp; else hp = value; OnChangeHP.Invoke(maxHp, hp); } }
     public event UnityAction<int, int> OnChangeHP;

    [Header("Dash Status")]
    [SerializeField] public int maxDashCount;
    [SerializeField] public int dashCount;
    [SerializeField] public float dashSpeed;
    [SerializeField] public float dashCoolTime;
    [SerializeField] public float dashTime;
    [SerializeField] public bool isDash;
    [SerializeField] public bool dashMode;


    private bool isDown;

    public Vector2 moveDir;
     Vector2 mouseDir;


    [SerializeField] public LayerMask groundCheckLayer;


    private StateMachine<PlayerStates> stateMachine;

    /******************************************************
     *                      Unity Events
     ******************************************************/
    #region Unity Events
    private void Start()
    {
        effect = GetComponent<PlayerEffectController>();
        attacker = GetComponent<PlayerAttacker>();
        rigid = GetComponent<Rigidbody2D>();
       // InitData();
        InitStateMachine();
    }


    private void Update()
    {
        stateMachine.Update();
        Flip();
        RestoreDashCount();
    }
    
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate ();
        //Debug.Log(stateMachine.CurState);
    }
    #endregion


    /******************************************************
    *                       Logics
    ******************************************************/
    #region Logics 
    void InitData()
    {
        maxHp = data.maxHp;

        moveSpeed = data.moveSpeed;
        maxSpeed = data.maxSpeed;
        breakSpeed = data.breakSpeed;
        jumpSpeed = data.jumpSpeed;
        dashSpeed = data.dashSpeed;

        maxJumpCount = data.jumpCount;
        maxDashCount = data.dashCount;

        dashCoolTime = data.dashCoolTime;      
        dashTime = data.dashTime;

        HP = maxHp;
    }

    void InitStateMachine()
    {
        stateMachine = new StateMachine<PlayerStates>();
        stateMachine.AddState(PlayerStates.Idle, new IdleState(this));
        stateMachine.AddState(PlayerStates.Run, new RunState(this));
        stateMachine.AddState(PlayerStates.InAir, new InAirState(this));
        stateMachine.AddState(PlayerStates.Jump, new JumpState(this));
        stateMachine.AddState(PlayerStates.DoubleJump, new DoubleJumpState(this));
        stateMachine.AddState(PlayerStates.Dash, new DashState(this));
        stateMachine.AddState(PlayerStates.Die, new DieState(this));

        stateMachine.Start(PlayerStates.Idle);
    }

    //마우스 방향에 따라 플레이어 좌우반전
    void Flip()
    {
        mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.localScale = new Vector3((rigid.transform.position.x > mouseDir.x)? -1:1,1,1);
        frontChecker.localScale = transform.localScale;
    }


    void RestoreDashCount()
    {
        if (!isDash && DashCoolTimeRoutine == null && dashCount <maxDashCount)
        {
            DashCoolTimeRoutine = StartCoroutine(DashCoolTime());
        }
    }
    public void TakeDamage(float damage)
    {
        HP -= (int)damage;
        Manager.Game.ShowFloatingDamage(transform,(int)damage);
        if(HP< 0)
        {
            stateMachine.ChangeState(PlayerStates.Die);
        }
    }
    
    #endregion

    /********************************************************
     *          Input System Actions
     ********************************************************/
    #region Input System Actions

    void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && isGround && jumpCount>0)
        {
            isGround = false;
            jumpCount--;
            stateMachine.ChangeState(PlayerStates.Jump);
        }else if(value.isPressed && !isGround && jumpCount>0)
        {
            jumpCount--;
            stateMachine.ChangeState(PlayerStates.DoubleJump);
        }
    }

    void OnDownJump(InputValue value)
    {
        if (value.isPressed)
        {
            isDown = true;
        }
        else
        {
            isDown = false;
        }
    }

    void OnAttack(InputValue inputValue)
    { 
        if (inputValue.isPressed && !attacker.isAttack)
        {
            attacker.Attack();
        }
    }


    //Dash 마우스 에임기준 대쉬랑 moveDir 기준 Dash 두가지 옵션 있음
    void OnDash(InputValue value)
    {
        if (value.isPressed && dashCount > 0)
        {
            dashCount--;
            stateMachine.ChangeState(PlayerStates.Dash);
        }
    }

    Coroutine DashCoolTimeRoutine;
    IEnumerator DashCoolTime()
    {
        yield return new WaitForSeconds(dashCoolTime);
        dashCount++;
        DashCoolTimeRoutine = null;
    }

    #endregion


    /********************************************************
     *              OnCollider / OnTrigger Event
     ********************************************************/
    #region Collider/Trigger Event
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDown)
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
            stateMachine.ChangeState(PlayerStates.InAir);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (groundCheckLayer.Contain(collision.gameObject.layer))
        {
            isGround = true;
            jumpCount = maxJumpCount;
        }
        if (!isDown)
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (groundCheckLayer.Contain(collision.gameObject.layer))
        {
            isGround = false;
        }
    }

    #endregion
}
