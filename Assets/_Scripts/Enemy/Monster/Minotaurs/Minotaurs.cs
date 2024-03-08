using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Minotaurs : Monster
{

    [SerializeField] Rigidbody2D rb;

    [SerializeField] Transform target;

    [SerializeField] float findRange;
    [SerializeField] float attackRange;
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float chargeCoolTime;



    protected override void Start()
    {
        base.Start();
        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Charge, new ChargeState(this));
        stateMachine.AddState(State.Die, new DieState(this));

        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;

        stateMachine.Start(State.Idle);
    }

    private void Update()
    {
        stateMachine.Update();
    }


    #region StateMachine

    private StateMachine<State> stateMachine = new StateMachine<State>();

    public enum State { Idle, Charge, Attack, Die }
    private class MinotaursState : BaseState<State>
    {
        public Minotaurs owner;

        public MinotaursState(Minotaurs owner) => this.owner = owner;
    }

    private class IdleState : MinotaursState
    {
        public IdleState(Minotaurs owner) : base(owner) { }
        public override void Enter()
        {
            owner.animator.SetBool("Idle", true);
        }

        public override void Update()
        {
            Vector2 dir = (owner.target.position - owner.transform.position).normalized;
            if (dir.x < 0)
            {
                owner.spriteRenderer.flipX = true;
            }
            else
            {
                owner.spriteRenderer.flipX = false;
            }
        }

        public override void Transition()
        {
            if (owner.hp <= 0)
            {
                ChangeState(State.Die);
            }
            else if (Vector2.Distance(owner.target.position, owner.transform.position) < owner.findRange)
            {
                ChangeState(State.Charge);
            }
        }

        public override void Exit()
        {
            owner.animator.SetBool("Idle", false);
        }
    }

    private class ChargeState : MinotaursState
    {
        public ChargeState(Minotaurs owner) : base(owner) { }

        private  Vector3 dir;

        public override void Enter()
        {
            dir = owner.target.position;
            owner.animator.SetBool("Charge", true);
        }

        public override void Update()
        {
            owner.StartCoroutine(ChargeRountine());
        }
        IEnumerator ChargeRountine()
        {
            owner.transform.position = Vector3.MoveTowards(owner.transform.position, dir, owner.moveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(owner.chargeCoolTime);
        }

        public override void Transition()
        {
            if (Vector2.Distance(owner.target.position, owner.transform.position) > owner.attackRange) //거리가 멀어지면
            {
                ChangeState(State.Idle);
            }
            else if (Vector2.Distance(owner.target.position, owner.transform.position) < owner.attackRange)
            {
                ChangeState(State.Attack);
            }
        }

        public override void Exit()
        {
            owner.animator.SetBool("Charge", false);
        }
    }

    private class AttackState : MinotaursState
    {
        public AttackState(Minotaurs owner) : base(owner) { }
        public override void Enter()
        {
            Debug.Log("Attack");
            //owner.rb.AddForce(Vector2.up * owner.jumpPower, ForceMode2D.Impulse);
        }
    }

    private class DieState : MinotaursState
    {
        public DieState(Minotaurs owner) : base(owner) { }

        private Coroutine routine;
        public override void Enter()
        {
            routine = owner.StartCoroutine(DieRoutine());
        }

        IEnumerator DieRoutine()
        {
            yield return new WaitForSeconds(1);
            Destroy(owner.gameObject);
        }
    }

    #endregion
}
