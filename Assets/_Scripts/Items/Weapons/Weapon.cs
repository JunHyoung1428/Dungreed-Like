using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    [Header("Weapon")]
    [SerializeField] protected Transform handPos;
    [SerializeField] protected PooledObject effect;
    [SerializeField] protected CinemachineImpulseSource impulse;

    [SerializeField] protected float attackDuration;
    [SerializeField] protected int damage;

    protected Coroutine Routine;
    protected bool attackType; // �ٰŸ� true, ���Ÿ� false �� ���� ��..����?


    protected override void Start()
    {
        base.Start();
        Manager.Pool.CreatePool(effect, 5, 5);
    }
    

    public virtual void Attack(Vector3 dir)
    {
    }

    protected void Shake(float roughness)
    {
        impulse.GenerateImpulseWithForce(roughness/20);
    }

}
