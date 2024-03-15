using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Monster : MonoBehaviour, IDamagable
{
    [SerializeField] int maxHp;
    public int MaxHP { get { return maxHp; } set { maxHp = value; OnChangeHP.Invoke(maxHp, hp); } }
    [SerializeField] protected int hp;
    public int HP { get { return hp; } set { if (value >= MaxHP) hp = maxHp; else hp = value; OnChangeHP.Invoke(maxHp, hp); } }

    public event UnityAction<int, int> OnChangeHP;

    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    public void TakeDamage(float damage)
    {
       HP -= (int)damage;
       Manager.Game.ShowFloatingDamage(transform, (int)damage);
    }

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
}
