using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class Skeldog : Monster
{
    [SerializeField] Rigidbody2D rb;

    [SerializeField] Transform target;

    [SerializeField] float chaseRange;
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float attackCoolTime;

    [SerializeField] LayerMask attackableLayer;

    bool isAttack;
   
    private StateMachine<State> stateMachine = new StateMachine<State>();

 

    protected override void Start()
    {
        base.Start();
        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Chase, new ChaseState(this));
        stateMachine.AddState(State.Die, new DieState(this));

        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;
  
        stateMachine.Start(State.Idle);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    public enum State { Idle, Chase, Attack, Die }
    private class SkeldogState : BaseState<State>
    {
        public Skeldog owner;

        public SkeldogState(Skeldog owner)
        {
            this.owner = owner;
        }
    }

    private class IdleState : SkeldogState
    {
        public IdleState(Skeldog owner) : base(owner) { } 
        public override void Enter()
        {
            owner.animator.SetBool("Idle",true);
        }

        public override void Transition()
        {
            if (owner.hp <= 0)
            {
                ChangeState(State.Die);
            }
            else if (Vector2.Distance(owner.target.position, owner.transform.position) < owner.chaseRange)
            {
                ChangeState(State.Chase);
            }    
        }

        public override void Exit()
        {
            owner.animator.SetBool("Idle", false);
        }
    }

    private class ChaseState : SkeldogState
    {
        public ChaseState(Skeldog owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetBool("Run", true);
        }

        public override void Update()
        {
            Vector2 dir = (owner.target.position - owner.transform.position).normalized;
            if(dir.x < 0)
            {
               owner.spriteRenderer.flipX = true;
            }
            else
            {
                owner.spriteRenderer.flipX=false;
            }
            //owner.rb.AddForce(dir*owner.moveSpeed*Time.deltaTime, ForceMode2D.Impulse); 
            owner.transform.Translate(dir * owner.moveSpeed * Time.deltaTime);
        }

        public override void Transition()
        {
            if (owner.hp <= 0)
            {
                ChangeState(State.Die);
            }

            if (Vector2.Distance(owner.target.position, owner.transform.position) > owner.chaseRange) //�Ÿ��� �־�����
            {
                ChangeState(State.Idle);
            } else if (Vector2.Distance(owner.target.position, owner.transform.position) < owner.attackRange && !owner.isAttack) //bool�� üũ�ϴϱ� �� �����
            {
                ChangeState(State.Attack);
            } 
        }

        public override void Exit()
        {
            owner.animator.SetBool("Run", false);
        }
    }

    private class AttackState : SkeldogState
    {
        public AttackState(Skeldog owner) : base(owner) { }

        Collider2D[] colliders = new Collider2D[10];
        Vector2 range;

        public override void Enter()
        {
            if(attackCoroutine == null)
            attackCoroutine =  owner.StartCoroutine(Attackroutine());
            ChangeState(State.Chase);
        }

        Coroutine attackCoroutine;
        IEnumerator Attackroutine()
        {
           
            owner.rb.AddForce(Vector2.up * owner.jumpPower, ForceMode2D.Impulse);

            int size = Physics2D.OverlapCircleNonAlloc(owner.transform.position, owner.attackRange, colliders, owner.attackableLayer); 
            for (int i = 0; i < size; i++)
            {
                range = (colliders[i].transform.position - owner.transform.position).normalized;

                IDamagable damagable = colliders[i].GetComponent<IDamagable>();
                damagable?.TakeDamage(owner.attackDamage);
            }

            yield return new WaitForSeconds(owner.attackCoolTime);
            attackCoroutine = null;
        }
    }

    private class DieState : SkeldogState
    {
        public DieState(Skeldog owner) : base(owner) { }

        private Coroutine routine;
        public override void Enter()
        {
            routine = owner.StartCoroutine(DieRoutine());
        }

        IEnumerator DieRoutine()
        {
            yield return new WaitForSeconds(0.1f);
            Destroy(owner.gameObject);
        }
    }
}
