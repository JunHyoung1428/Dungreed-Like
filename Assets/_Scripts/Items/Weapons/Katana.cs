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

    //TODO: 공격시 충돌 판정으로 Damage 주는거
    IEnumerator AttackRoutine(Vector3 dir)
    {
        //그냥 리소스 이미지를 90도 돌린걸로 수정함 ..
        // Quaternion rotate = Quaternion.AngleAxis(-90f, Vector3.forward) * Quaternion.LookRotation(transform.forward);
        PooledObject pooledObject = Manager.Pool.GetPool(effect, transform.position + dir.normalized * attackRange,
           transform.rotation);
        Shake(damage);
        yield return new WaitForSeconds(attackDuration);
        Routine = null;
    }
}
