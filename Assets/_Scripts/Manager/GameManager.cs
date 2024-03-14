using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] FloatingDamage floatingDamageText;


    public void Start()
    {
        Manager.Pool.CreatePool(floatingDamageText, 10, 10);
    }

    public void ShowFloatingDamage(Transform transform, int damage)
    {
        FloatingDamage text = (FloatingDamage)Manager.Pool.GetPool(floatingDamageText, transform.position + new Vector3(0, 0.5f, 0), transform.rotation); 
        text.TMP.text = damage.ToString();
    }

    public void ShowFloatingDamage(Transform transform, int damage, Color color)
    {
        FloatingDamage text = (FloatingDamage)Manager.Pool.GetPool(floatingDamageText, transform.position + new Vector3(0, 0.5f, 0), transform.rotation);
        text.TMP.text = damage.ToString();
        text.TMP.color=color;   
    }

    public void Test()
    {
        Debug.Log(GetInstanceID());
    }
}
