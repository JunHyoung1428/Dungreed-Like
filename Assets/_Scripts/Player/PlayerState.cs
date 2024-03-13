using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


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
        player.rigid.gravityScale = 20; 
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

    public override void Enter()
    {
        player.animator.SetBool("Run", true);
        effect.state = PlayerEffectController.State.Walk;
    }

    public override void FixedUpdate()
    {
        if (!player.isSlope)
        {
            if (player.moveDir.x < 0 && player.rigid.velocity.x > -player.maxSpeed)
            {
                player.rigid.velocity = new Vector2(-player.moveSpeed, player.rigid.velocity.y);
                effect.isFlip = true;
            } //최대 속도 제어
            else if (player.moveDir.x > 0 && player.rigid.velocity.x < player.maxSpeed)
            {
                player.rigid.velocity = new Vector2(player.moveSpeed, player.rigid.velocity.y);
                effect.isFlip = false;
            }
        }
        else
        {
            player.rigid.velocity = player.moveDir.x * player.perp * -1 * player.moveSpeed;
        }

        if (player.moveDir.x == 0 && player.rigid.velocity.x != 0)
        {
            Vector2 breakForce = player.rigid.velocity.x > 0 ? Vector2.left : Vector2.right;
            player.rigid.AddForce(breakForce * player.breakSpeed);
        }
    }

    public override void Transition()
    {
        if (!player.isGround)
        {
            ChangeState(PlayerStates.InAir);
        }
        if (player.moveDir.x == 0)
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
    public InAirState(PlayerController owner) : base(owner) {
        maxFallSpeed = player.breakSpeed * -2;
    }

    public override void Enter()
    {
        player.animator.SetBool("Jump", true);
        player.rigid.gravityScale = 1;
    }

    public override void FixedUpdate()
    {
        if (player.moveDir.x > 0 && player.rigid.velocity.x < player.maxSpeed)
        {
            player.rigid.AddForce(Vector2.right * player.moveSpeed, ForceMode2D.Force);
        }
        else if (player.moveDir.x < 0 && player.rigid.velocity.x > -player.maxSpeed)
        {
            player.rigid.AddForce(Vector2.left * player.moveSpeed, ForceMode2D.Force);
        }

        if (player.rigid.velocity.y < maxFallSpeed)
        {
            player.rigid.velocity= new Vector2(player.rigid.velocity.x,maxFallSpeed);
        }
    }

    public override void Transition()
    {
        if (player.isGround)
        {
            ChangeState(PlayerStates.Idle);
        }
        if (input.actions["Jump"].IsPressed() && input.actions["Jump"].triggered)
        {
            ChangeState(PlayerStates.DoubleJump);
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
        player.rigid.velocity = new Vector2(player.rigid.velocity.x, player.jumpSpeed * 0.75f);
    }
}
#endregion
/********************************************************
    *         Dash & Die ...
********************************************************/
public class DashState : PlayerState
{
    public DashState(PlayerController owner) : base(owner) { }

    Vector3 dashDir;
    public override void Enter()
    {
        if (player.dashCount > 0)
        {
            dashDir = player.mouseDir;
            dashDir.z = 0;
            dashDir = dashDir - player.transform.position;
            if (player.dashMode)
            {
                player.StartCoroutine(player.DashWithMousePos(dashDir));
            }
            else
            {
                player.StartCoroutine(player.DashWithMoveDir());
                //StartCoroutine(DashWithMoveDir());
            }
        }
        ChangeState(PlayerStates.Idle);
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


public class DieState : PlayerState
{
    public DieState(PlayerController owner) : base(owner) { }

}
#endregion