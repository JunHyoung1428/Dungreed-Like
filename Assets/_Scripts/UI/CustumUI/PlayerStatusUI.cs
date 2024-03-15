using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] PlayerController player;

    [SerializeField] Image hpBar;
    [SerializeField] TextMeshProUGUI hpText;

    [SerializeField] Image staminaBar;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.OnChangeHP+= UpdateHP ;
        UpdateHP(player.MaxHP,player.HP);
    }
    
    private void UpdateHP(int maxHP, int HP)
    {
        hpBar.fillAmount = HP / (float)maxHP;
        hpText.text = $"{HP.ToString()}/{maxHP.ToString()}";
    }

    private void OnDisable()
    {
        player.OnChangeHP -= UpdateHP ;
    }


    private void UpdateStamina()
    {

    }

}
