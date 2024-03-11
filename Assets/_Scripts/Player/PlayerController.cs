using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{

    [Header("Component")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;

    [Header("Property")]
    [SerializeField] public float moveSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] public float breakSpeed;
    [SerializeField] public float jumpSpeed;
    [SerializeField] int hp;
     public int HP { get { return hp; } set { hp = value; } }

    [Header("DashStatus")]
    [SerializeField] int dashCount;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashCoolTime;
    [SerializeField] float dashTime;
    [SerializeField] bool isDash;
    [SerializeField] bool dashMode;
    [SerializeField] PlayerEffectController playerEffectController;
    [SerializeField] Transform effectPos;



    [SerializeField] private bool isGround;
    private bool isDown;
    private Vector2 moveDir;
    private Vector3 mouseDir;
    private Vector3 flipVec = new Vector3(-1, 1, 1);

    [SerializeField] public LayerMask groundCheckLayer;

   // public enum State { Idle, Jump, Walk, Die }

    // public PlayerInput playerInput;
   // private StateMachine<State> stateMachine = new StateMachine<State>();

    /******************************************************
     *                      Unity Events
     ******************************************************/
    #region Unity Events
    private void Start()
    {
     
    }

   /*
    private void Update()
    {
        stateMachine.Update();
    }
    */
    private void FixedUpdate()
    {
        mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Move();
        Flip();
        animator.SetBool("Jump", !isGround);
    }
    #endregion

    void Flip()
    {
        if (rb.transform.position.x > mouseDir.x)
        {
            transform.localScale = flipVec;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
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
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            effectPos.transform.localPosition =  playerEffectController.JumpEffectPos;
            playerEffectController.Animator.SetTrigger("Jump");
        }else if(value.isPressed && !isGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed*0.75f);
            effectPos.transform.localPosition = playerEffectController.JumpEffectPos;
            playerEffectController.Animator.SetTrigger("DoubleJump");
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

    void Move()
    {

        if (moveDir.x < 0 && rb.velocity.x > -maxSpeed)
        {
            rb.AddForce(Vector2.right * moveDir.x * moveSpeed);
        }
        else if (moveDir.x > 0 && rb.velocity.x < maxSpeed)
        {

            rb.AddForce(Vector2.right * moveDir.x * moveSpeed);
        }
        else if (moveDir.x == 0 && rb.velocity.x > 0)
        {
            rb.AddForce(Vector2.left * breakSpeed);
        }
        else if (moveDir.x == 0 && rb.velocity.x < 0)
        {
            rb.AddForce(Vector2.right * breakSpeed);
        }

        
        if (isGround && moveDir.x != 0)
        {
            animator.SetBool("Run", isGround);
            effectPos.transform.localPosition = playerEffectController.WalkEffectPos;
            playerEffectController.Animator.SetBool("Run", isGround);
        }
        else
        {
            animator.SetBool("Run", false);
            playerEffectController.Animator.SetBool("Run", false);
            effectPos.transform.localPosition = playerEffectController.JumpEffectPos;
        }

        if (rb.velocity.y < -maxSpeed * 2)
        {
            Vector2 vel = rb.velocity;
            vel.y = -maxSpeed * 2;
            rb.velocity = vel;
        }
    }

    //Dash 마우스 에임기준 대쉬랑 moveDir 기준 Dash 두가지 옵션 있음
    void OnDash(InputValue value)
    {
        if (value.isPressed)
        {
            if (dashCount > 0)
            {
                Vector3 dashDir = mouseDir;
                dashDir.z = 0;
                dashDir = dashDir - transform.position;
                if (dashMode)
                {

                    StartCoroutine(DashWithMousePos(dashDir));

                }
                else
                {
                    StartCoroutine(NewDashRoutine(dashDir));
                    //StartCoroutine(DashWithMoveDir());
                }
            }
        }
    }

    IEnumerator DashWithMousePos(Vector3 dashDir)
    {
        isDash = true;
        dashCount--;
        float orginGravityScale = rb.gravityScale;
        //rb.gravityScale = 0f;
        playerEffectController.makeGhost = true;
        rb.velocity = dashDir.normalized * dashSpeed; // velocity 말고 transform.position 자체를 옮기는거 고려해 볼것
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = orginGravityScale;
        playerEffectController.makeGhost = false;
        isDash = false;
        yield return new WaitForSeconds(dashCoolTime);
        dashCount++;
    }

    IEnumerator DashWithMoveDir()
    {
        isDash = true;
        dashCount--;
        float orginGravityScale = rb.gravityScale;
        //rb.gravityScale = 0f;
        playerEffectController.makeGhost = true;
        rb.velocity = moveDir * dashSpeed;
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = orginGravityScale;
        playerEffectController.makeGhost = false;
        isDash = false;
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

        playerEffectController.makeGhost = true;

        while (time <= dashTime)
        {
            time += step;
            rb.MovePosition(Vector3.Lerp(startingPos, moveTarget, time));
            yield return new WaitForFixedUpdate();
        }
        playerEffectController.makeGhost = false;

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
}
