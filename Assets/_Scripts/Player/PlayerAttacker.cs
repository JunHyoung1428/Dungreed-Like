using System.Collections;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttacker : MonoBehaviour
{
    [SerializeField] Transform body;
    [SerializeField] Transform handPos;
    [SerializeField] GameObject[] Weapons;
    private Weapon[] weapons;

    [SerializeField] float attackDuration;
    [SerializeField] public bool isAttack;

    [SerializeField] int activateWeaponIndex =0;

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

        if (!Weapons[0].activeSelf)
        {
            Weapons[0].SetActive(true);
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


    public void Attack()
    {
        weapons[activateWeaponIndex]!.Attack(dir);
    }

    void OnSwap(InputValue inputValue)
    {
        float input =inputValue.Get<float>();

        if (input !=0)
        {
            Weapons[activateWeaponIndex].SetActive(false);
            float newWeaponIndex = activateWeaponIndex + input;

            if (newWeaponIndex < 0)
            {
                activateWeaponIndex = weapons.Length - 1;
            }
            else if (newWeaponIndex >= weapons.Length)
            {
                activateWeaponIndex = 0;
            }
            else
            {
                activateWeaponIndex = (int)newWeaponIndex;
            }
            Weapons[activateWeaponIndex].SetActive(true);
        }
    }
}
