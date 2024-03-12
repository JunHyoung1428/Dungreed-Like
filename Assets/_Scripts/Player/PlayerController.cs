using System.Buffers;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{

    [Header("Component")]
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [SerializeField] PlayerEffectController effect;

    [Header("Base Status")]
    [SerializeField] public float moveSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] public float breakSpeed;
    [SerializeField] public float jumpSpeed;
    [SerializeField] int hp;
     public int HP { get { return hp; } set { hp = value; } }

    [Header("Dash Status")]
    [SerializeField] int dashCount;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashCoolTime;
    [SerializeField] float dashTime;
    [SerializeField] bool isDash;
    [SerializeField] bool dashMode;



    [SerializeField] private bool isGround;
    [SerializeField] private bool isSlope;
    private Transform frontChecker;
    private float slopeAngle;
    private bool isDown;
    private Vector2 moveDir;
    private Vector2 perp;
    private Vector3 mouseDir;


    [SerializeField] public LayerMask groundCheckLayer;

    public enum State { Idle, Run, Jump,DoubleJump ,Dash, Die }

    public PlayerInput playerInput;
    private StateMachine<State> stateMachine = new StateMachine<State>();

    /******************************************************
     *                      Unity Events
     ******************************************************/
    #region Unity Events
    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Jump, new JumpState(this));
        stateMachine.AddState(State.DoubleJump, new DoubleJumpState(this));
        stateMachine.AddState(State.Dash, new DashState(this));
        stateMachine.AddState(State.Run, new RunState(this));
        stateMachine.AddState(State.Die, new DieState(this));
        stateMachine.Start(State.Idle);
    }


    private void Update()
    {
        stateMachine.Update();
    }
    
    private void FixedUpdate()
    {
        mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Move();
        Flip();
    }
    #endregion


    /******************************************************
    *                      Move Logics
    ******************************************************/
    //마우스 방향에 따라 플레이어 좌우반전
    void Flip()
    {
        if (rigid.transform.position.x > mouseDir.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            //캐싱해놓고 써야하나 했는데 C#에선 구조체형식이라 힙에 할당 안일어난다함 다행
            
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    void Move()
    {
        if (isGround)
        {
            if (moveDir.x < 0 && rigid.velocity.x > -maxSpeed)
            {
                rigid.AddForce(Vector2.right * moveDir.x * moveSpeed);
                effect.isFlip = true;
            } //최대 속도 제어
            else if (moveDir.x > 0 && rigid.velocity.x < maxSpeed)
            {
                rigid.AddForce(Vector2.right * moveDir.x * moveSpeed);
                effect.isFlip = false;
            }
            else if (moveDir.x == 0 && rigid.velocity.x > 0)
            {
                rigid.AddForce(Vector2.left * breakSpeed);
            }//방향 전환시 역방향으로 가속
            else if (moveDir.x == 0 && rigid.velocity.x < 0)
            {
                rigid.AddForce(Vector2.right * breakSpeed);
            }
        }
        else
        {

        }


        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down,0.3f, groundCheckLayer);
        RaycastHit2D frontHit = Physics2D.Raycast(transform.position, transform.right, 0.1f, groundCheckLayer);

        if (hit || frontHit)
        {
            if(frontHit)
            {
                SlopeCheck(frontHit);
            }
            else if(hit)
            {
                SlopeCheck(hit);
            }
        }

        Debug.DrawLine(hit.point, hit.point + perp, Color.red);
        Debug.DrawLine(hit.point, hit.point + hit.normal, Color.blue);


        if (isSlope)
        {
            Debug.Log("isSlope");
        }


        if (rigid.velocity.y < -maxSpeed * 2)//낙하 속도 제어
        {
            Vector2 vel = rigid.velocity;
            vel.y = -maxSpeed * 2;
            rigid.velocity = vel;
        }
    }


    //경사면 확인
    private void SlopeCheck(RaycastHit2D raycastHit)
    {
        perp = Vector2.Perpendicular(raycastHit.normal).normalized;
        slopeAngle = Vector2.Angle(raycastHit.normal, perp);

        isSlope = (slopeAngle != 0) ? true:false;

        /*
        if(slopeAngle != 0)
        {
            isSlope = true;
        }
        else
        {
            isSlope=false;
        }*/
    }


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
        if (value.isPressed && isGround)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, jumpSpeed);
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


    //Dash 마우스 에임기준 대쉬랑 moveDir 기준 Dash 두가지 옵션 있음
    void OnDash(InputValue value)
    {
        if (value.isPressed)
        {
             stateMachine.ChangeState(State.Dash);
        }
    }

     IEnumerator DashWithMousePos(Vector3 dashDir)
    {
        isDash = true;
        effect.makeGhost = isDash;
        dashCount--;
        rigid.velocity = dashDir.normalized * dashSpeed; // velocity 말고 transform.position 자체를 옮기는거 고려해 볼것
        yield return new WaitForSeconds(dashTime);
        isDash = false;
        effect.makeGhost = isDash;
        yield return new WaitForSeconds(dashCoolTime);
        dashCount++;
    }

     IEnumerator DashWithMoveDir()
    {
        isDash = true;
        effect.makeGhost = isDash;
        dashCount--;
        rigid.velocity = moveDir * dashSpeed;
        yield return new WaitForSeconds(dashTime);
        isDash = false;
        effect.makeGhost = isDash;
        yield return new WaitForSeconds(dashCoolTime);
        dashCount++;
    }

    // lerp하게 고정 좌표로 position 변경
    // 폐기하거나 조작감 개선 필요
     IEnumerator NewDashRoutine(Vector3 dashDir)
    {
        isDash = true;
        dashCount--;

        float dashDis = 4.0f;
        Vector3 startingPos = transform.position;
        Vector3 moveTarget = startingPos + Vector3.ClampMagnitude(dashDir, dashDis);
        float dis = Vector3.Distance(transform.position, moveTarget);
        float step = (dashSpeed / dis) * Time.deltaTime;
        float time = 0f;

        effect.makeGhost = true;

        while (time <= dashTime)
        {
            time += step;
            rigid.MovePosition(Vector3.Lerp(startingPos, moveTarget, time));
            yield return new WaitForFixedUpdate();
        }
        effect.makeGhost = false;

        isDash = false;
        yield return new WaitForSeconds(dashCoolTime);
        dashCount++;
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
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (groundCheckLayer.Contain(collision.gameObject.layer))
        {
            isGround = true;
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


    /********************************************************
    *             Player State Machine
    ********************************************************/
    #region PlayerState
    public class PlayerState : BaseState<State>
    {
        public PlayerController owner;
        public PlayerEffectController effect;
        public PlayerInput input;

        public PlayerState(PlayerController owner)
        {
            this.owner = owner;
            effect = owner.effect;
            input = owner.playerInput;
        }
    }
    public class IdleState : PlayerState
    {
        public IdleState(PlayerController owner) : base(owner) { }

        public override void Enter()
        {
            effect.state=PlayerEffectController.State.Idle;
        }

        public override void Transition()
        {
            if(owner.isGround && owner.moveDir.x != 0)
            {
                ChangeState(State.Run);
            }else if(input.actions["Jump"].IsPressed() && input.actions["Jump"].triggered)
            {
                ChangeState(State.Jump);
            }
        }
    }

    public class RunState : PlayerState
    {
        public RunState(PlayerController owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetBool("Run", true);
            effect.state = PlayerEffectController.State.Walk;
        }

        public override void Transition()
        {
            if (!owner.isGround)
            {
                ChangeState(State.Jump);
            }
            if(owner.moveDir.x == 0)
            {
                ChangeState(State.Idle);
            }
        }

        public override void Exit()
        {
            owner.animator.SetBool("Run", false);
            effect.state = PlayerEffectController.State.Idle;
        }
    }

    public class DashState : PlayerState
    {
        public DashState(PlayerController owner) : base(owner) { }

        Vector3 dashDir;
        public override void Enter()
        {
            if (owner.dashCount > 0)
            {
                dashDir = owner.mouseDir;
                dashDir.z = 0;
                dashDir = dashDir - owner.transform.position;
                if (owner.dashMode)
                {
                    owner.StartCoroutine(owner.DashWithMousePos(dashDir));
                }
                else
                {
                    owner.StartCoroutine(owner.DashWithMoveDir());
                    //StartCoroutine(DashWithMoveDir());
                }
            }
            ChangeState(State.Idle);
        }
        /*
        public override void Update()
        {
            if (owner.isDash)
            {
                owner.rb.velocity = dashDir.normalized * owner.dashSpeed;
            }
        }*/
        public override void Exit()
        {
            //충돌 무시 판정 Off
        }
    }

    public class JumpState : PlayerState
    {
        public JumpState(PlayerController owner) : base(owner) {}

        public override void Enter()
        {
            owner.animator.SetBool("Jump", true);
            effect.state = PlayerEffectController.State.Jump;
        }

        public override void Transition()
        {
            if (owner.isGround)
            {
                ChangeState(State.Idle);
            }
            if (input.actions["Jump"].IsPressed()&& input.actions["Jump"].triggered)
            {
                ChangeState(State.DoubleJump);
            }
        }

        public override void Exit()
        {
            owner.animator.SetBool("Jump", false);
        }
    }

    public class DoubleJumpState : PlayerState
    {
        public DoubleJumpState(PlayerController owner) : base(owner) { }

        public override void Enter()
        {
            owner.rigid.velocity = new Vector2(owner.rigid.velocity.x, owner.jumpSpeed * 0.75f);
            owner.animator.SetBool("Jump", true);
            effect.state = PlayerEffectController.State.DoubleJump;
        }

        public override void Transition()
        {
            if(owner.isGround)
            {
                ChangeState(State.Idle);
            }
            if (input.actions["Jump"].IsPressed() && input.actions["Jump"].triggered)
            {
                ChangeState(State.DoubleJump);
            }
        }
        public override void Exit()
        {
            owner.animator.SetBool("Jump", false);
        }
    }

    public class DieState : PlayerState
    {
        public DieState(PlayerController owner) : base(owner) { }

    }
    #endregion
}
