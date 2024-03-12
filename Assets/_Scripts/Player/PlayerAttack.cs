using System.Collections;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Transform body;
    [SerializeField] Transform handPos;
    [SerializeField] GameObject[] Weapons;
    private Weapon[] weapons;

    [SerializeField] float attackDuration;
    [SerializeField] bool isAttack;

    [SerializeField] bool activateWeapon=true;

    private Vector3 dir;
    /******************************************************
     *                      Unity Events
     ******************************************************/
    private void Start()
    {
        weapons = new Weapon[Weapons.Length];
        for (int i = 0; i < Weapons.Length; i++)
        {
            weapons[i] = Weapons[i].GetComponent<Weapon>(); //나중에 인벤토리에서 무기 교체시에도 새로 갱신해줘야함
        }
    }


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


    void OnAttack(InputValue inputValue)
    {
        if (inputValue.isPressed && !isAttack)
        {
           if(activateWeapon)
            {
                weapons[0]!.Attack(dir);
            }
            else
            {
                weapons[1]!.Attack(dir);
            }
        }
    }

    void OnSwap(InputValue input)
    {
        if(input.isPressed)
        {
            if(activateWeapon)
            {
                activateWeapon = false;
                Weapons[0].SetActive(false);
                Weapons[1].SetActive(true);
            }
            else
            {
                activateWeapon = true;
                Weapons[0].SetActive(true);
                Weapons[1].SetActive(false);
            }
        }
    }
}
