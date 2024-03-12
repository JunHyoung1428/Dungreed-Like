using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Sword : Weapon
{
    [SerializeField] bool attackForm;
    [SerializeField] float attackRange;


    public override void Attack(Vector3 dir)
    {
        base.Attack(dir);

        StartCoroutine(AttackRoutine(dir));
    }

    //TODO: 공격시 충돌 판정으로 Damage 주는거
    IEnumerator AttackRoutine(Vector3 dir)
    {
        isAttack = true;
        if (attackForm)
        {
            Debug.Log("Attack1");

            handPos.localScale = new Vector3(handPos.localScale.x, -1, handPos.localScale.z);
            attackForm = false;
        }
        else
        {
            Debug.Log("Attack2");
            handPos.localScale = new Vector3(handPos.localScale.x, 1, handPos.localScale.z);
            attackForm = true;
        }
        PooledObject pooledObject = Manager.Pool.GetPool(effect, transform.position+dir.normalized*attackRange, transform.rotation);
        yield return new WaitForSeconds(attackDuration);
        isAttack = false;
    }
}
