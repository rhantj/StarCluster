using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TR_VerticalPlatform : ReplayRecorder, IRewindable
{
    float speed = 5f;

    private void Start()
    {
        MoveUpdown();
    }

    public void MoveUpdown()
    {
        StartCoroutine(Co_MoveUpDown());
    }

    IEnumerator Co_MoveUpDown()
    {
        float moveDis = 10f;
        float total = 0f;
        var wait = new WaitForFixedUpdate();
        
        int side = 1;

        StartRecording();
        while (true)
        {
            while (IsRewinding) yield return null;

            float dis = rb.velocity.magnitude * Time.fixedDeltaTime;
            total += dis;

            if (total >= moveDis)
            {
                side = -side;
                total = 0f;
            }

            rb.velocity = speed * side * Vector2.up;
            yield return wait;
        }
    }

    public void Rewind()
    {
        StartReversePlayBack();
    }
}
