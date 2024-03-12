using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] float speed;
    private bool hit;

    // Update is called once per frame
    void Update()
    {
        if (!hit) transform.Translate(Vector3.right * speed*Time.deltaTime,Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        Destroy(this, 0.35f);
    }
}
