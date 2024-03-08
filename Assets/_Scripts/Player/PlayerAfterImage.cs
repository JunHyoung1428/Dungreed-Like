using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{
    public float ghostDelay;
    private float ghostDelayTime;
    public PooledObject ghost;
    public bool makeGhost;

    void Start()
    {
        this.ghostDelayTime = ghostDelay;
        Manager.Pool.CreatePool(ghost, 10, 10);
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
                PooledObject currentGhost = Manager.Pool.GetPool(ghost,transform.position, transform.rotation);
                Sprite currentSprite = this.GetComponent<SpriteRenderer>().sprite; // update에서 GetComponet 안쓰게 수정해야함
                currentGhost.transform.localScale = this.transform.localScale;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                this.ghostDelayTime = this.ghostDelay;
            }
        }
    }
}
