using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TR_Fence : ReplayRecorder, IInteractable
{
    float speed = 8f;
    Vector3 downSidePos;
    Vector3 upSidePos;
    bool side = false; // f : down, t : up

    private void OnEnable()
    {
        downSidePos = transform.position;
        upSidePos = transform.position + Vector3.up * 3f;
    }

    void GoUpside()
    {
        StartCoroutine(Co_GoUpside());
    }

    IEnumerator Co_GoUpside( )
    {
        var wait = new WaitForFixedUpdate();
        StartRecording();

        float sidefacing = side ? -1f : 1f;
        while (true)
        {
            while (IsRewinding) yield return null;

            rb.velocity = sidefacing * speed * Vector3.up;
            yield return wait;

            if (transform.position.y >= upSidePos.y)
            {
                side = true;
                break;
            }
            else if (transform.position.y <= downSidePos.y)
            {
                side = false;
                break;
            }
        }

        StopRecording();

        rb.velocity = Vector3.zero;
    }

    public void OnInteraction(PlayerController player)
    {
        GoUpside();
    }
}
