using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{

    [Header("Anchor Points")]
    [SerializeField] Transform top;
    [SerializeField] Transform bottom;
    [SerializeField] Transform right;
    [SerializeField] Transform left;

    [Header("Images")]
    [SerializeField] Image minimapImage;
    [SerializeField] Image minimapPlayerImage;
    [SerializeField]  Vector2 defaultPos;

    [Header("Player")]
    [SerializeField] PlayerController player;

    bool isMinimapOpen =true;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        if(player != null )
        {
            minimapPlayerImage.enabled = true;
        }
        defaultPos = minimapPlayerImage.rectTransform.localPosition;
    }

    void Update()
    {
        if (!isMinimapOpen) return;
        if(player != null)
        {
            Vector2 mapArea = new Vector2(Vector3.Distance(left.position, right.position), Vector3.Distance(bottom.position, top.position));
            Vector2 charPos = new Vector2(Vector3.Distance(left.position, new Vector3(player.transform.position.x, 0f, 0f)),
                Vector3.Distance(bottom.position, new Vector3(0f,player.transform.position.y,0f)));
            Vector2 nomalPos = new Vector2(charPos.x / mapArea.x , charPos.y / mapArea.y);

            minimapPlayerImage.rectTransform.anchoredPosition = new Vector2(minimapImage.rectTransform.sizeDelta.x * nomalPos.x , minimapImage.rectTransform.sizeDelta.y * nomalPos.y) ;
        }
    }

    public void SetNewAnchor(Transform top, Transform bottom , Transform left, Transform right)
    {
        this.top = top;
        this.bottom = bottom;
        this.left = left;
        this.right = right;
        minimapPlayerImage.rectTransform.anchoredPosition = defaultPos;
    }

    public void TurnOff()
    {
        isMinimapOpen = false;
        minimapImage.enabled = false;
        minimapPlayerImage.enabled = false;
    }
}
