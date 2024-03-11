using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectController : MonoBehaviour
{
    [Header("Ghost Trail")]
    [SerializeField] PooledObject effectGhostTrail;
    [SerializeField] float ghostDelay;
    private float ghostDelayTime;
    [SerializeField] public bool makeGhost;

    [Header("Dust Effect")]
    [SerializeField] Animator animator;
    public Animator Animator { get { return animator; } set { animator = value; } }

    public Vector3 JumpEffectPos = new Vector3(0, -0.85f, 0);
    public Vector3 WalkEffectPos = new Vector3 (-0.75f, -0.25f, 0);


    void Start()
    {
        this.ghostDelayTime = ghostDelay;
        Manager.Pool.CreatePool(effectGhostTrail, 10, 10);
    }

    void FixedUpdate()
    {
        if (makeGhost)
        {
            if (ghostDelayTime > 0)
            {
                ghostDelayTime -= Time.deltaTime;
            }
            else
            {
                PooledObject currentGhost = Manager.Pool.GetPool(effectGhostTrail, transform.position, transform.rotation);
                Sprite currentSprite = this.GetComponent<SpriteRenderer>().sprite; // update에서 GetComponet 안쓰게 수정해야함
                currentGhost.transform.localScale = this.transform.localScale;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                this.ghostDelayTime = this.ghostDelay;
            }
        }
    }



}
