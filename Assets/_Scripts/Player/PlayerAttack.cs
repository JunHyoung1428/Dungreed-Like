using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Transform handPos;
    [SerializeField] GameObject weapon;
    [SerializeField] SpriteRenderer body;

    [SerializeField] float rotationAngle = 120.0f;
    [SerializeField] float attackDuration;
    [SerializeField] bool attackForm;
    [SerializeField] bool isAttack;

    /******************************************************
     *                      Unity Events
     ******************************************************/


    //마우스 따라서 손 회전
    // ** 회전이 아래에서만 이뤄짐 
    void Update()
    {
        if (!isAttack)
            Aim();
    }

    void Aim()
    {
        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - handPos.position;
        if (dir != Vector3.zero)
        {
            dir.z = 0f;
            //handPos.transform.LookAt2DLerp(dir); // x스케일 -1 일때 회전이 반대 방향으로 이뤄짐
            handPos.up = dir.normalized;
        }
    }
    /******************************************************
     *                     Input Action Events
     ******************************************************/

    //일단은 장병기 구현
    // 어쩌다보니 손 움직이는 스크립트가 되어버림...
    //공격시 일정각도 회전 + 카메라 쉐이킹 + 폼에 따라 레이어 변경해서 캐릭터 몸 앞/뒤로 전환
    void OnAttack(InputValue inputValue)
    {
        if (inputValue.isPressed && !isAttack)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttack = true;
        float currentZRotation = handPos.rotation.eulerAngles.z;
        float targetZRotation;
        if (attackForm)
        {
            targetZRotation = currentZRotation + rotationAngle;
            attackForm = false;
        }
        else
        {
            targetZRotation = currentZRotation - rotationAngle;
            attackForm = true;
        }
        handPos.rotation = Quaternion.Euler(handPos.rotation.eulerAngles.x, handPos.rotation.eulerAngles.y, targetZRotation);
        //handPos.rotation = Quaternion.Slerp(handPos.rotation, targetZRotation, Time.deltaTime);

        yield return new WaitForSeconds(attackDuration);
        isAttack = false;
    }
}
