using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnPoint : MonoBehaviour, IInteractable
{
    public void OnInteraction(PlayerController player)
    {
        var clearCanv = GameObject.FindGameObjectWithTag("Canvas");
        var panel = clearCanv.GetComponent<ClearControl>();

        panel.Toggle();
    }
}
