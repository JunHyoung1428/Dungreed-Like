using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Transform body;
    [SerializeField] Transform handPos;
    [SerializeField] Weapon weapon;

    [SerializeField] float rotationAngle;
    [SerializeField] float attackSpeed;
    [SerializeField] float attackDuration;
    [SerializeField] bool attackForm;
    [SerializeField] bool isAttack;


    private Vector3 dir;
    /******************************************************
     *                      Unity Events
     ******************************************************/

    void Update()
    {
        dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - handPos.position;
        if (!isAttack)
            Aim();
    }

    void Aim()
    {
        if (dir != Vector3.zero)
        {
           dir.z = 0f;
           handPos.localScale = new Vector3(body.localScale.x, handPos.localScale.y, handPos.localScale.z);
           handPos.right = dir.normalized;
        }
    }
    /******************************************************
     *                     Input Action Events
     ******************************************************/

    //�ϴ��� �庴�� ����
    void OnAttack(InputValue inputValue)
    {
        if (inputValue.isPressed && !isAttack)
        {
            weapon.Attack(dir);
        }
    }
}
