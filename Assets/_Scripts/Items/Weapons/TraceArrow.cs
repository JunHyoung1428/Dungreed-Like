using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceArrow : PooledObject
{
    Vector2[] point = new Vector2[4];

    [SerializeField] public int damage;
    [SerializeField][Range(0, 1)] private float t = 0;
    [SerializeField] public float spd = 5;
    [SerializeField][Range(0,30)] public float posA = 0.55f;
    [SerializeField][Range(0,30)] public float posB = 0.45f;

    bool hit;
    public GameObject master;
    public Vector3 target;

    void Start()
    {
        master = this.gameObject;
        point[0] = master.transform.position; // P0
        point[1] = PointSetting(master.transform.position); // P1
        point[2] = PointSetting(target); // P2
        point[3] = target; // P3
    }

    void FixedUpdate()
    {
        if (t > 1) return;
        if (hit) return;
        t += Time.deltaTime * spd;
        DrawTrajectory();
    }

    Vector2 PointSetting(Vector2 origin)
    {
        float x, y;
        x = posA * Mathf.Cos(Random.Range(0, 360) * Mathf.Deg2Rad)
        + origin.x;
        y = posB * Mathf.Sin(Random.Range(0, 360) * Mathf.Deg2Rad)
        + origin.y;
        return new Vector2(x, y);
    }

    void DrawTrajectory()
    {
        transform.position = new Vector2(
        FourPointBezier(point[0].x, point[1].x, point[2].x, point[3].x),
        FourPointBezier(point[0].y, point[1].y, point[2].y, point[3].y)
        );
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        IDamagable damagable = collision.GetComponent<IDamagable>();
        damagable?.TakeDamage(damage*Random.Range(0.8f,1.3f));
    }


    private float FourPointBezier(float a, float b, float c, float d)
    {
        return Mathf.Pow((1 - t), 3) * a
        + Mathf.Pow((1 - t), 2) * 3 * t * b
        + Mathf.Pow(t, 2) * 3 * (1 - t) * c
        + Mathf.Pow(t, 3) * d;
    }

    public void OnDisable()
    {
        hit = false;
    }
}