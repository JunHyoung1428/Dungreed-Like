using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{

    [Header("Component")]
    [SerializeField] new Rigidbody2D rigidbody;
    [SerializeField] new SpriteRenderer renderer;
    [SerializeField] Animator animator;

    [Header("Property")]
    [SerializeField] public float moveSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] public float breakSpeed;
    [SerializeField] public float jumpSpeed;
    [SerializeField] int hp;
    [SerializeField] int dashCount;
    [SerializeField] float dashPower;

    
    private bool isGround = true;
    private bool isDown;
    private float dashCoolTime;
    private Vector2 moveDir;

    [SerializeField] public LayerMask groundCheckLayer;

    public enum State { Idle, Jump, Walk, Die}
    
    public int HP {  get { return hp; } set { hp = value; } }

    public PlayerInput playerInput;
    private StateMachine<State> stateMachine = new StateMachine<State>();
    private void Start()
    {
        stateMachine.AddState(State.Idle, new IdleState());
        stateMachine.AddState(State.Jump, new JumpState());
        stateMachine.Start(State.Idle);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void LateUpdate()
    {
        stateMachine.LateUpdate();
    }

    private void FixedUpdate()
    {
        Move();
        stateMachine.FixedUpdate();
    }

    void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();

        if (moveDir.x < 0)
        {
            renderer.flipX = true;
            animator.SetBool("Run", true);

        }
        else if (moveDir.x > 0)
        {
            renderer.flipX = false;
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && isGround)
        {
            Jump();
        }
    }
    void Jump()
    {
        animator.SetTrigger("Jump");
        Vector2 vel = rigidbody.velocity;
        vel.y = jumpSpeed;
        rigidbody.velocity = vel;
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
  
        if (moveDir.x < 0 && rigidbody.velocity.x > -maxSpeed) 
        {
            rigidbody.AddForce(Vector2.right * moveDir.x * moveSpeed);
        }
        else if (moveDir.x > 0 && rigidbody.velocity.x < maxSpeed)
        {

            rigidbody.AddForce(Vector2.right * moveDir.x * moveSpeed);
        }
        else if (moveDir.x == 0 && rigidbody.velocity.x > 0) 
        {
            rigidbody.AddForce(Vector2.left * breakSpeed);
        }
        else if (moveDir.x == 0 && rigidbody.velocity.x < 0)
        {
            rigidbody.AddForce(Vector2.right * breakSpeed);
        }

        if (rigidbody.velocity.y < -maxSpeed * 2) 
        {
            Vector2 vel = rigidbody.velocity;
            vel.y = -maxSpeed * 2;
            rigidbody.velocity = vel;
        }

        animator.SetFloat("YSpeed", rigidbody.velocity.y);
    }

    void OnDash(InputValue value)
    {
        if (value.isPressed)
        {
            if(dashCount > 0)
            {

                Vector2 dashDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 playerPos = new Vector2(rigidbody.transform.position.x, rigidbody.transform.position.y);
                dashDir = dashDir - playerPos;
                Debug.Log(dashDir);
                rigidbody.velocity = Vector2.zero;

                 rigidbody.AddForce(dashDir.normalized*dashPower, ForceMode2D.Impulse);
                //dashCount--;
            }
        }
    }

    IEnumerator DashCoolTimeRoutine()
    {
        yield return new WaitForSeconds(dashCoolTime);
        dashCount++;
    }

    #region Ex)StateMachine
    private class PlayerState : BaseState<State>
    {
        protected PlayerController controller;
        protected PlayerInput input;
    }

    private class IdleState : PlayerState
    {
        public override void Enter()
        {
            base.Enter();
        }
    }

    private class JumpState : PlayerState
    {
        public override void Update()
        {
            if (input.actions["Jump"].IsPressed()
                && input.actions["Jump"].triggered)
            {

            }
        }
    }
    #endregion


  
}
