using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToEnding : MonoBehaviour, IInteractable
{
    public void Interact(PlayerController player)
    {
        Manager.Scene.LoadScene("Ending");
    }


}
