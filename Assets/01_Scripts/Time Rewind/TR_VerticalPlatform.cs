using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TR_VerticalPlatform : ReplayRecorder, IRewindable
{
    float speed = 5f;
    float moveDis = 5f;
    Vector3 firstPos;
    Vector3 upSidePos;
    Vector3 downSidePos;

    private void Start()
    {
        firstPos = transform.position;
        upSidePos = firstPos + Vector3.up * moveDis;
        downSidePos = firstPos + Vector3.down * moveDis;

        RewindableManager.Instance?.Registration(this);
        MoveUpdown();
    }

    private void OnDisable()
    {
        RewindableManager.Instance?.Unregister(this);
    }

    public void MoveUpdown()
    {
        StartCoroutine(Co_MoveUpDown());
    }

    IEnumerator Co_MoveUpDown()
    {
        var wait = new WaitForFixedUpdate();
        
        int side = 1;

        StartRecording();
        while (true)
        {
            while (IsRewinding) yield return null;

            if (transform.position.y >= upSidePos.y)
                side = -1;
            else if (transform.position.y <= downSidePos.y)
                side = 1;

            rb.velocity = speed * side * Vector2.up;
            yield return wait;
        }
    }

    public void Rewind()
    {
        StartReversePlayBack();
    }

    public void StopRewind()
    {
        StartRecording();
    }

    protected override IEnumerator PlaybackCoroutine()
    {
        yield return base.PlaybackCoroutine();
        StartRecording();
    }
}
