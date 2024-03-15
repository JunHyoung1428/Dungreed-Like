using System.Collections;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [SerializeField] protected bool autoRelease;
    [SerializeField] protected float releaseTime;

    private ObjectPool pool;
    public ObjectPool Pool { get { return pool; } set { pool = value; } }

    protected virtual void OnEnable()
    {
        if (autoRelease)
        {
            StartCoroutine(ReleaseRoutine());
        }
    }

    protected IEnumerator ReleaseRoutine()
    {
        yield return new WaitForSeconds(releaseTime);
        Release();
    }

    public void Release()
    {
        if (pool != null)
        {
            pool.ReturnPool(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
