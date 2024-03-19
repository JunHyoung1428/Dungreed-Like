using Cinemachine;
using UnityEngine;

public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] bool autoTransfer;
    [SerializeField] Transform transferPoint;
    [SerializeField] Transform[] newAnchorPoints;
    [SerializeField] Animator animator;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] MinimapUI minimapUI;
    private PlayerController player;

    public void Transfer()
    {
        if (animator != null)
            animator.SetTrigger("Close");

        if (newAnchorPoints != null)
            minimapUI.TurnOff();
            //minimapUI.SetNewAnchor(newAnchorPoints[0], newAnchorPoints[1], newAnchorPoints[2], newAnchorPoints[3]);

        Manager.Scene.PublicFadeOut();
        if (player != null)
            player.transform.position = transferPoint.position;
        virtualCamera.Priority = 0; // 나중에 CameraManager를 두던지 해서 관리 할 것.
        Manager.Scene.PublicFadeIn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (autoTransfer)
        {
            player = collision.GetComponent<PlayerController>();
            Transfer();
        }
    }

    public void Interact(PlayerController player)
    {
        this.player = player;
        Transfer();
    }
}
