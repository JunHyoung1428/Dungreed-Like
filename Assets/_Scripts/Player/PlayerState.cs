using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


public enum PlayerStates { Idle, Run, InAir, Jump, DoubleJump, Dash, Die }
/********************************************************
    *             Player State Machine
********************************************************/
#region PlayerState
public class PlayerState : BaseState<PlayerStates>
{
    public PlayerController player;
    public PlayerEffectController effect;
    public PlayerInput input;

    public PlayerState(PlayerController owner)
    {
        this.player = owner;
        effect = owner.effect;
        input = owner.playerInput;
    }
}

/********************************************************
    *            On Ground State : Idle, Run
********************************************************/
#region On Ground States

public class IdleState : PlayerState
{
    public IdleState(PlayerController owner) : base(owner) { }

    public override void Enter()
    {
        effect.state = PlayerEffectController.State.Idle;
        player.rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        player.rigid.gravityScale = 15;
    }

    public override void FixedUpdate()
    {
        if (player.rigid.velocity.x != 0)
        {
            Vector2 breakForce = player.rigid.velocity.x > 0 ? Vector2.left : Vector2.right;
            player.rigid.AddForce(breakForce * player.breakSpeed);
        }
    }

    public override void Transition()
    {
        if (player.isGround && player.moveDir.x != 0)
        {
            ChangeState(PlayerStates.Run);
        }
        else if (!player.isGround)
        {
            ChangeState(PlayerStates.InAir);
        }
    }

    public override void Exit()
    {
        player.rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}

public class RunState : PlayerState
{
    public RunState(PlayerController owner) : base(owner) { }

    float slopeAngle;
    Vector2 perp;

    public override void Enter()
    {
        player.animator.SetBool("Run", true);
        effect.state = PlayerEffectController.State.Walk;
    }

    public override void Update()
    {
        SlopeCheck();
    }

    public override void FixedUpdate()
    {
        if (!player.isSlope)
        {
            if (player.moveDir.x < 0 && player.rigid.velocity.x > -player.maxSpeed)
            {
                player.rigid.velocity = new Vector2(-player.moveSpeed, player.rigid.velocity.y);
                effect.isFlip = true;
            }
            else if (player.moveDir.x > 0 && player.rigid.velocity.x < player.maxSpeed)
            {
                player.rigid.velocity = new Vector2(player.moveSpeed, player.rigid.velocity.y);
                effect.isFlip = false;
            }
        }
        else
        {
            player.rigid.velocity = player.moveDir.x * perp * -1 * player.moveSpeed;
        }
    }

    private void SlopeCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.down, 1f, player.groundCheckLayer);
        RaycastHit2D frontHit = Physics2D.Raycast(player.frontChecker.position, player.frontChecker.right * player.transform.localScale.x, 0.1f, player.groundCheckLayer);

        if (player.DebugMode)
        {
            Debug.DrawLine(player.frontChecker.position, player.frontChecker.position + player.frontChecker.right * player.transform.localScale.x, Color.blue);
            Debug.DrawLine(hit.point, hit.point + perp, Color.red);
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.blue);
        }

        if (hit || frontHit)
        {
            if (frontHit)
                hit = frontHit;
        }
        perp = Vector2.Perpendicular(hit.normal).normalized;
        slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        player.isSlope = (slopeAngle > 10) ? true : false;
    }

    public override void Transition()
    {
        if (!player.isGround)
        {
            ChangeState(PlayerStates.InAir);
        }
        if (player.isGround && player.moveDir.x == 0)
        {
            ChangeState(PlayerStates.Idle);
        }
    }

    public override void Exit()
    {
        player.animator.SetBool("Run", false);
        effect.state = PlayerEffectController.State.Idle;
    }
}
#endregion
/********************************************************
    *         InAirState : InAir, Jump, DoubleJump
********************************************************/

#region In Air States
public class InAirState : PlayerState
{
    protected float maxFallSpeed;
    public InAirState(PlayerController owner) : base(owner)
    {
        maxFallSpeed = player.breakSpeed * -2;
    }

    public override void Enter()
    {
        player.animator.SetBool("Jump", true);
        player.rigid.gravityScale = 1;
    }

    public override void FixedUpdate()
    {
        if (player.moveDir.x != 0)
        {
            if (player.moveDir.x > 0 && player.rigid.velocity.x < player.maxSpeed)
            {
                player.rigid.AddForce(Vector2.right * player.moveSpeed, ForceMode2D.Force);
            }
            else if (player.moveDir.x < 0 && player.rigid.velocity.x > -player.maxSpeed)
            {
                player.rigid.AddForce(Vector2.left * player.moveSpeed, ForceMode2D.Force);
            }
        }
        else if ( player.rigid.velocity.x != 0)
        {
            Vector2 breakForce = player.rigid.velocity.x > 0 ? Vector2.left : Vector2.right;
            player.rigid.AddForce(breakForce * player.breakSpeed);
        }


        if (player.rigid.velocity.y < maxFallSpeed)
        {
            player.rigid.velocity = new Vector2(player.rigid.velocity.x, maxFallSpeed);
        }
    }

    public override void Transition()
    {
        if (player.isGround)
        {
            ChangeState(PlayerStates.Idle);
        }
    }

    public override void Exit()
    {
        player.animator.SetBool("Jump", false);
    }
}


public class JumpState : InAirState
{
    public JumpState(PlayerController owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        effect.state = PlayerEffectController.State.Jump;
        player.rigid.velocity = new Vector2(player.rigid.velocity.x, player.jumpSpeed);
    }
}

public class DoubleJumpState : InAirState
{
    public DoubleJumpState(PlayerController owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        effect.state = PlayerEffectController.State.DoubleJump;
        player.rigid.velocity = new Vector2(player.rigid.velocity.x, player.jumpSpeed * 0.9f);
    }
}
#endregion
/********************************************************
    *         Dash & Die ...
********************************************************/
public class DashState : PlayerState
{
    public DashState(PlayerController owner) : base(owner) { }

    Vector2 dashDir;
    public override void Enter()
    {
        player.rigid.gravityScale = 0.1f;
        dashDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
        dashDir = dashDir.normalized;

        effect.isFlip = (dashDir.x > 0) ? false : true;

        if (player.dashMode)
        {
            DashRoutine = player.StartCoroutine(DashWithMousePos(dashDir));
        }
        else
        {
            DashRoutine = player.StartCoroutine(DashWithMoveDir());
        }
    }

    Coroutine DashRoutine;
    IEnumerator DashWithMousePos(Vector3 dashDir)
    {
        player.isDash = true;
        effect.makeGhost = player.isDash;
        player.rigid.velocity = dashDir.normalized * player.dashSpeed; // velocity 말고 transform.position 자체를 옮기는거 고려해 볼것
        yield return new WaitForSeconds(player.dashTime);
    
        player.isDash = false;
        effect.makeGhost = player.isDash;
        DashRoutine = null;
    }

    IEnumerator DashWithMoveDir()
    {
        player.isDash = true;
        effect.makeGhost = player.isDash;
        player.rigid.velocity = player.moveDir * player.dashSpeed;
        yield return new WaitForSeconds(player.dashTime);

        player.isDash = false;
        effect.makeGhost = player.isDash;
        DashRoutine = null;
    }



    public override void Transition()
    {
       if(DashRoutine == null)
        {
            if(player.isGround)
            {
                ChangeState(PlayerStates.Idle);
            }
            else
            {
                ChangeState(PlayerStates.InAir);
            }
        }
    }


    public override void Exit()
    {
        player.rigid.velocity = Vector2.zero;
        player.rigid.gravityScale = 1f;

        //충돌 무시 판정 Off
    }
}
public class DieState : PlayerState
{
    public DieState(PlayerController owner) : base(owner) { }

    public override void Enter()
    {
        Debug.Log("Die");

        player.animator.SetTrigger("Die");
        player.enabled = false;
    }
}
#endregion