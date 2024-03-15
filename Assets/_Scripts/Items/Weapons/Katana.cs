using System.Collections;
using UnityEngine;

public class Katana : Weapon
{
    [SerializeField] float attackRange;


    public override void Attack(Vector3 dir)
    {
        base.Attack(dir);

        Routine = StartCoroutine(AttackRoutine(dir));
    }

    //TODO: ���ݽ� �浹 �������� Damage �ִ°�
    IEnumerator AttackRoutine(Vector3 dir)
    {
        //�׳� ���ҽ� �̹����� 90�� �����ɷ� ������ ..
        // Quaternion rotate = Quaternion.AngleAxis(-90f, Vector3.forward) * Quaternion.LookRotation(transform.forward);
        PooledObject pooledObject = Manager.Pool.GetPool(effect, transform.position + dir.normalized * attackRange,
           transform.rotation);
        Shake(damage);
        yield return new WaitForSeconds(attackDuration);
        Routine = null;
    }
}
