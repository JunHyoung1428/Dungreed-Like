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


    //���콺 ���� �� ȸ��
    // ** ȸ���� �Ʒ������� �̷��� 
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
            //handPos.transform.LookAt2DLerp(dir); // x������ -1 �϶� ȸ���� �ݴ� �������� �̷���
            handPos.up = dir.normalized;
        }
    }
    /******************************************************
     *                     Input Action Events
     ******************************************************/

    //�ϴ��� �庴�� ����
    // ��¼�ٺ��� �� �����̴� ��ũ��Ʈ�� �Ǿ����...
    //���ݽ� �������� ȸ�� + ī�޶� ����ŷ + ���� ���� ���̾� �����ؼ� ĳ���� �� ��/�ڷ� ��ȯ
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
