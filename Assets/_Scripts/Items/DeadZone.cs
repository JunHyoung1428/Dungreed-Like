using UnityEngine;

public class DeadZonde : MonoBehaviour
{
    [SerializeField] Transform transferPoint;
    private PlayerController player;

    void Start()
    {
        GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
        player = gameObject.GetComponent<PlayerController>();   
    }

    public void Transfer()
    {
        Manager.Scene.PublicFadeOut();
        player.transform.position = transferPoint.position;
        player.TakeDamage(10);
        Manager.Scene.PublicFadeIn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transfer();
    }

}
