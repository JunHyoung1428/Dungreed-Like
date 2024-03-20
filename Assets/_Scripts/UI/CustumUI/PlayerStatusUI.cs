using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] PlayerController player;


    [Header("HP")]
    [SerializeField] Image hpBar;
    [SerializeField] TextMeshProUGUI hpText;

    [Header("Dash")]
    [SerializeField] Image[] dashCount;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.OnChangeHP+= UpdateHP ;
        UpdateHP(player.MaxHP,player.HP);
    }

    private void Update()
    {
        UpdateDashCount();
    }

    private void UpdateHP(int maxHP, int HP)
    {
        hpBar.fillAmount = HP / (float)maxHP;
        hpText.text = $"{HP.ToString()}/{maxHP.ToString()}";
    }
    private void UpdateDashCount()
    {
        for (int i = 0; i < dashCount.Length; ++i)
        {
            float fillAmount = i >= player.dashCount ? 0f : 1f;

            if (dashCount[i].fillAmount < 1f && fillAmount == 0f)
            {
                dashCount[i].color = new Color(dashCount[i].color.r, dashCount[i].color.g, dashCount[i].color.b, 0.5f);
                dashCount[i].fillAmount = Mathf.Lerp(dashCount[i].fillAmount, fillAmount, Time.deltaTime * 10f);
            }
            else
            {
                dashCount[i].color = new Color(dashCount[i].color.r, dashCount[i].color.g, dashCount[i].color.b, 1f);
                dashCount[i].fillAmount = Mathf.MoveTowards(dashCount[i].fillAmount, fillAmount, Time.deltaTime / 1f);
            }

        }
    }


    private void OnDisable()
    {
        player.OnChangeHP -= UpdateHP ;
    }



}
