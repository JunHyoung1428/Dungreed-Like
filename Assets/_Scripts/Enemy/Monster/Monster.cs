using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Monster : MonoBehaviour, IDamagable
{
    [SerializeField] protected int hp;
    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    public void TakeDamage(float damage)
    {
       hp -= (int)damage;
       Manager.Game.ShowFloatingDamage(transform, (int)damage);
    }

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
}
