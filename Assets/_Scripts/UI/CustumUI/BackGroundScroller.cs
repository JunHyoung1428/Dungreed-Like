using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScroller : MonoBehaviour
{
    [SerializeField] Transform[] children; // 자식들(땅)의 Transform이 들어갈 배열

    [SerializeField] float scrollSpeed;
    [SerializeField] float offset;

    private void Update()
    {
        foreach (Transform child in children)
        {
            child.Translate(Vector2.left * scrollSpeed * Time.deltaTime, Space.World);

            if (child.position.x < -offset)
            {
                Vector2 pos = new Vector2(offset, child.position.y);
                child.position = pos;
            }
        }
    }
}
