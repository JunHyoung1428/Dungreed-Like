using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingDamage : PooledObject
{
    [SerializeField] public TextMeshPro TMP;
    [SerializeField] float floatingSpeed;
    [SerializeField] float fadeSpeed;
    [SerializeField] Color alpha;


    private void OnEnable()
    {
        alpha.a = 255;
    }
    void Update()
    {
        transform.Translate(new Vector3(0, floatingSpeed * Time.deltaTime,0));
        alpha.a = Mathf.Lerp(alpha.a,0, Time.deltaTime * fadeSpeed);
        TMP.color = alpha;
    }
}
