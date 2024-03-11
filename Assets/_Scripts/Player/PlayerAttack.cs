using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Transform body;
    [SerializeField] Transform handPos;
    [SerializeField] GameObject weapon;

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
            //handPos.transform.LookAt2DLerp(dir); // x스케일 -1 일때 회전이 반대 방향으로 이뤄짐
           handPos.localScale = new Vector3(body.localScale.x, handPos.localScale.y, handPos.localScale.z);
           handPos.right = dir.normalized;
        }
    }
    /******************************************************
     *                     Input Action Events
     ******************************************************/

    //일단은 장병기 구현
    void OnAttack(InputValue inputValue)
    {
        if (inputValue.isPressed && !isAttack)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    //TODO:공격시 이펙트 오브젝트 풀링으로 생성
    IEnumerator AttackRoutine()
    {
        isAttack = true;
        if(attackForm)
        {
            Debug.Log("Attack1");
            handPos.localScale = new Vector3(handPos.localScale.x, -1, handPos.localScale.z);
            attackForm = false;
        }
        else
        {
            Debug.Log("Attack2");
            handPos.localScale = new Vector3(handPos.localScale.x, 1, handPos.localScale.z);
            attackForm = true;
        }
        yield return new WaitForSeconds(attackDuration);
        isAttack =false;
    }
}
