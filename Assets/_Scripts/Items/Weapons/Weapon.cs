using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item, IInteractable
{
    [Header("Weapon")]
    [SerializeField] protected Transform handPos;
    [SerializeField] protected PooledObject effect;
    [SerializeField] protected CinemachineImpulseSource impulse;

    [SerializeField] protected float attackDuration;
    [SerializeField] protected int damage;

    protected Coroutine Routine;
    protected bool attackType;


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

    public void Interact(PlayerController player)
    {
        //player.attacker.GetWeapon(this); Attacker는 오브젝트를 List로 가지는데, 문제는 오브젝트 자체를 스크립트에서 넘겨줄수는 없음
        //추후 리팩토링을 통해 Weapon과 Weapondata들의 분리와 이를 통한 방법이 필요
    }
}
