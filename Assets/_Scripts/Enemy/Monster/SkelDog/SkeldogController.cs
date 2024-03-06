using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class SkeldogController : Monster
{
    [SerializeField] Animator animator;
    public enum State { Idle, Chase, Jump, Die}
    private StateMachine<State> stateMachine = new StateMachine<State>();

    private void Start()
    {
        //stateMachine.AddState(State.Idle, new IdleState());
        //stateMachine.AddState(State.Jump, new JumpState());
        //stateMachine.AddState(State.Run, new RunState());
        //stateMachine.AddState(State.Die, new DieState());

        stateMachine.Start(State.Idle);
    }

}
