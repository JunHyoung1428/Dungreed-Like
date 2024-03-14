using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] PooledObject floatingDamageText;


    public void Start()
    {
        Manager.Pool.CreatePool(floatingDamageText, 10, 10);
    }

    public void ShowFloatingDamage(Transform transform, int damage)
    {
        PooledObject text = Manager.Pool.GetPool(floatingDamageText, transform.position + new Vector3(0, 0.5f, 0), transform.rotation); 
        text.GetComponent<TextMeshPro>().text = damage.ToString();
    }

    public void Test()
    {
        Debug.Log(GetInstanceID());
    }
}
