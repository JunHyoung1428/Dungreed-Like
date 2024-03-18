using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPUI : MonoBehaviour
{
    [SerializeField] Monster monster;

    [SerializeField] GameObject hpBarBG;
    [SerializeField] Image hpBar;

    // Start is called before the first frame update
    void Start()
    {
        monster = GetComponentInParent<Monster>();
        monster.OnChangeHP += UpdateHP;
    }

    private void UpdateHP(int maxHP, int HP)
    {
        if (!hpBarBG.activeSelf)
        {
            hpBarBG.SetActive(true);
        }
        hpBar.fillAmount = HP / (float)maxHP;
    }
}
