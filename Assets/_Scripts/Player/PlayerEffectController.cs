using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerEffectController : MonoBehaviour
{
    [Header("Ghost Trail")]
    [SerializeField] PooledObject effectGhostTrail;
    [SerializeField] float ghostDelay;
    private float delayTime;
    [SerializeField] public bool makeGhost;

    [Header("Dust Effect")]
    [SerializeField] PooledObject effectWalk;
    [SerializeField] float dustDelay;
    [SerializeField] PooledObject effectJump;
    [SerializeField] PooledObject effectDoubleJump;

    public bool isFlip;

    public enum State {Idle, Walk, Jump, DoubleJump};
    public State state;
    void Start()
    {
        this.delayTime = ghostDelay;
        Manager.Pool.CreatePool(effectGhostTrail, 10, 10);
        Manager.Pool.CreatePool(effectWalk, 5, 10);
        Manager.Pool.CreatePool(effectJump, 2, 2);
        Manager.Pool.CreatePool(effectDoubleJump, 2, 2);
    }

    void Update()
    {
        if (makeGhost)
        {
            StartCoroutine(MakeGhostRoutine());
        }

        switch (state)
        {
            case State.Idle:
                break;
            case State.Walk:
                StartCoroutine(WalkEffectRoutine());
                break;
            case State.Jump:
                PooledObject jumpEffect = Manager.Pool.GetPool(effectJump, transform.position, transform.rotation);
                state = State.Idle;
                break;
            case State.DoubleJump:
                PooledObject doubleJumpEffect = Manager.Pool.GetPool(effectDoubleJump, transform.position, transform.rotation);
                state = State.Idle;
                break;
        }
    }

    IEnumerator MakeGhostRoutine()
    {
        if (delayTime > 0)
        {
            delayTime -= Time.deltaTime;
        }
        else
        {
            PooledObject currentGhost = Manager.Pool.GetPool(effectGhostTrail, transform.position, transform.rotation);
            currentGhost.GetComponent<SpriteRenderer>().flipX = isFlip;//= this.GetComponent<SpriteRenderer>().sprite;
            this.delayTime = this.ghostDelay;
        }
        yield return new WaitForSeconds(ghostDelay);
    }

    IEnumerator WalkEffectRoutine()
    {
        if (delayTime > 0)
        {
            delayTime -= Time.deltaTime;
        }
        else
        {
            PooledObject dustEffect = Manager.Pool.GetPool(effectWalk, transform.position + new Vector3(0, -0.25f, -1f), transform.rotation);
            dustEffect.GetComponent<SpriteRenderer>().flipX = isFlip;
            this.delayTime = this.dustDelay;
        }
        yield return new WaitForSeconds(0.1f);
    }
}
