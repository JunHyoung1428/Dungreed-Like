using System.Collections;
using UnityEngine;

public class DarkBow : Weapon
{

    public override void Attack(Vector3 dir)
    {
        if (Routine == null)
            Routine = StartCoroutine(AttackRoutine(dir));
    }

    IEnumerator AttackRoutine(Vector3 dir)
    {
        Arrow pooledObject =  (Arrow)Manager.Pool.GetPool(effect, transform.position + dir.normalized * 1f, transform.rotation);
        pooledObject.damage = damage;
        yield return new WaitForSeconds(attackDuration);
 
        Routine = null;
    }
}
