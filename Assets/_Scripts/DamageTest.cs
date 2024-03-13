using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class DamageTest : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float tickTime;


    private void OnTriggerStay2D(Collider2D other)
    {
        if (damageCoroutine == null)
        {
            Debug.Log("damageOn");
            IDamagable damagable = other.GetComponent<IDamagable>();
            damageCoroutine = StartCoroutine(DamageRoutine(damagable));
        }
     
    }

    private Coroutine damageCoroutine;
    IEnumerator DamageRoutine(IDamagable damagable)
    {
    
            damagable?.TakeDamage(damage);
        yield return new WaitForSeconds(tickTime);
      damageCoroutine = null; 
    }
}
