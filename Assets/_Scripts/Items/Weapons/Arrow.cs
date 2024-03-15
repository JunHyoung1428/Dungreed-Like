using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : PooledObject
{
    [SerializeField] float speed;
    [SerializeField] public float damage;
    private bool hit;

    // Update is called once per frame
    void Update()
    {
        if (!hit) transform.Translate(Vector3.right * speed*Time.deltaTime,Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamagable damagable = collision.GetComponent<IDamagable>();
        damagable?.TakeDamage(damage * Random.Range(0.8f, 1.3f));
        hit = true;
    }

    private void OnDisable()
    {
        hit = false;
    }
}
