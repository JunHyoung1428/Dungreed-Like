using System.Collections;
using UnityEngine;

public class Sword : Weapon
{
    [SerializeField] bool attackForm;
    [SerializeField] float attackRange;
    [SerializeField] float attackAngle;
    [SerializeField] LayerMask attackableLayer;


    float cosAngle;
    protected override void Start()
    {
        base.Start();
        cosAngle = Mathf.Cos(attackAngle * Mathf.Deg2Rad);
    }
    
    public override void Attack(Vector3 dir)
    {
        base.Attack(dir);
        if (Routine == null)
            StartCoroutine(AttackRoutine(dir));
    }

    Collider2D[] colliders = new Collider2D[10];
    Vector2 range;

    //TODO: 공격시 충돌 판정으로 Damage 주는거
    IEnumerator AttackRoutine(Vector3 dir)
    {
        if (attackForm)
        {
            handPos.localScale = new Vector3(handPos.localScale.x, -1, handPos.localScale.z);
            attackForm = false;
        }
        else
        {
            handPos.localScale = new Vector3(handPos.localScale.x, 1, handPos.localScale.z);
            attackForm = true;
        }
       PooledObject pooledObject = Manager.Pool.GetPool(effect, transform.position + dir.normalized * attackRange, transform.rotation);

        int size = Physics2D.OverlapCircleNonAlloc(transform.position, attackRange, colliders, attackableLayer);
        for (int i = 0; i < size; i++)
        {
            range = (colliders[i].transform.position - transform.position).normalized;

            IDamagable damagable = colliders[i].GetComponent<IDamagable>();
            
            if (Vector2.Dot(range, dir) > cosAngle)
            {
                damagable.TakeDamage(damage);
            }
        }

        Shake(damage);
        yield return new WaitForSeconds(attackDuration);
        Routine = null;
    }
}
