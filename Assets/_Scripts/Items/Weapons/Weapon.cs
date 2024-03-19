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
        //player.attacker.GetWeapon(this); Attacker�� ������Ʈ�� List�� �����µ�, ������ ������Ʈ ��ü�� ��ũ��Ʈ���� �Ѱ��ټ��� ����
        //���� �����丵�� ���� Weapon�� Weapondata���� �и��� �̸� ���� ����� �ʿ�
    }
}
