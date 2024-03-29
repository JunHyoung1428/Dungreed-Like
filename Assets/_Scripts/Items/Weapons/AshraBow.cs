using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AshraBow : Weapon
{
    
    [SerializeField] bool isImpulse;
    [SerializeField] float attackCoolTime;
    [SerializeField] Animator animator;

    protected override void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public override void Attack(Vector3 dir)
    {
        if(Routine==null)
            Routine = StartCoroutine(AttackRoutine(dir));
    }
    IEnumerator AttackRoutine(Vector3 dir)
    {
        if(isImpulse)Shake(damage*5);
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.25f);
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target = new Vector3(target.x, target.y, 0);
        for (int i = 0; i < damage; i++)
        {
            TraceArrow pooledObject = (TraceArrow)Instantiate(effect, transform.position + dir.normalized * 1f, transform.rotation);
            //TraceArrow pooledObject = (TraceArrow)Manager.Pool.GetPool(effect, transform.position + dir.normalized * 1f, transform.rotation);
            pooledObject.damage = damage;
            pooledObject.target = target;
            yield return new WaitForSeconds(attackDuration);
        }
        yield return new WaitForSeconds(attackCoolTime);           
        Routine = null;
    }
}
