using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBow : Weapon
{
    
    public override void Attack(Vector3 dir)
    {
        base.Attack(dir);
        StartCoroutine(AttackRoutine(dir));
    }

    IEnumerator AttackRoutine(Vector3 dir)
    {
        isAttack = true;
        for(int i=0; i < 1; i++)
        {
            PooledObject pooledObject =Manager.Pool.GetPool(effect, transform.position+dir.normalized * 1f , transform.rotation);
            //Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //pooledObject.GetComponent<TraceArrow>().target = new Vector3(target.x,target.y,0);
            yield return new WaitForSeconds(attackDuration);
        }
        isAttack = false;
    }
}
