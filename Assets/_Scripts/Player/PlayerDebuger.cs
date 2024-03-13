using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebuger : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }



    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            playerController.TakeDamage(10);
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            playerController.TakeDamage(-10);
        }
    }
}
