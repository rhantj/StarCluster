using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TR_Fence : ReplayRecorder, IInteractable, IRewindable
{
    float speed = 8f;
    Vector3 downSidePos;
    Vector3 upSidePos;
    bool side = false; // f : down, t : up

    private void Start()
    {
        RewindableManager.Instance?.Registration(this);
        downSidePos = transform.position;
        upSidePos = transform.position + Vector3.up * 3f;

        StartRecording();
    }

    void OnDisable()
    {
        RewindableManager.Instance?.Unregister(this);
    }

    void GoUpside()
    {
        StartCoroutine(Co_GoUpside());
    }

    IEnumerator Co_GoUpside( )
    {
        var wait = new WaitForFixedUpdate();

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
        rb.velocity = Vector3.zero;

        yield return wait;
    }

    public void OnInteraction(PlayerController player)
    {
        GoUpside();
    }

    public void Rewind()
    {
        StartReversePlayBack();
    }
}
