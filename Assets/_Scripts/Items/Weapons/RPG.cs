using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG : Weapon
{
    [SerializeField] bool isImpulse;
    public override void Attack(Vector3 dir)
    {
        base.Attack(dir);
        StartCoroutine(AttackRoutine(dir));
    }

    IEnumerator AttackRoutine(Vector3 dir)
    {
        if(isImpulse)Shake(damage*5);
        for (int i = 0; i < damage; i++)
        {
            PooledObject pooledObject = Instantiate(effect, transform.position + dir.normalized * 1f, transform.rotation);
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target = new Vector3(target.x, target.y, 0);
            Debug.Log(target);
            pooledObject.GetComponent<TraceArrow>().target = target;
            yield return new WaitForSeconds(attackDuration);
        }
    }
}
