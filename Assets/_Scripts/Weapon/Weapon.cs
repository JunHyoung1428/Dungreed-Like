using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected Transform handPos;
    [SerializeField] protected PooledObject effect;

    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float attackDuration;

    [SerializeField] protected bool isAttack;


    private void Start()
    {
        Manager.Pool.CreatePool(effect, 5, 5);
    }


    public virtual void Attack(Vector3 dir)
    {
        if (isAttack) return;
    }

}
