using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ReplayRecorder : MonoBehaviour
{
    public delegate void StateChangeHandler(ReplayRecorder sender);
    public event StateChangeHandler OnStateChange;

    [Header("Replay Settings")]
    public int recordFps = 60;
    public int maxFrames = 4000;
    public float rewindSpeed = 30f;

    protected Rigidbody2D rb;

    protected Stack<FrameData> recordedFrames = new();
    protected Coroutine recordCo;
    protected Coroutine playbackCo;

    public bool IsRecording { get; private set; } = false;
    public bool IsRewinding { get; private set; } = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void StartRecording()
    {
        if (IsRecording) return;
        StopPlaybackAndClearFrames(false);
        IsRecording = true;
        Debug.Log("Start Record");

        if (gameObject.activeSelf)
        {
            recordCo = StartCoroutine(RecordCoroutine());
            OnStateChange?.Invoke(this);
        }
    }

    public void StopRecording()
    {
        if (!IsRecording) return;
        IsRecording = false;

        if (recordCo != null)
            StopCoroutine(recordCo);

        recordCo = null;
        OnStateChange?.Invoke(this);
    }

    public void StartReversePlayBack()
    {
        if (IsRewinding) return;
        StopRecording();
        IsRewinding = true;

        rb.velocity = Vector2.zero;
        playbackCo = StartCoroutine(PlaybackCoroutine());
        OnStateChange?.Invoke(this);
    }

    public void StopPlaybackAndClearFrames(bool clear = true)
    {
        if(playbackCo != null)
            StopCoroutine(playbackCo);

        playbackCo = null;
        IsRewinding = false;

        if (clear) recordedFrames.Clear();
        OnStateChange?.Invoke(this);
    }

    IEnumerator RecordCoroutine()
    {
        var wait = new WaitForFixedUpdate();
        float recordInterval = 1f / recordFps;
        float elapsedTime = 0f;

        while (IsRecording)
        {
            yield return wait;
            elapsedTime += Time.fixedDeltaTime;

            if (elapsedTime < recordInterval) continue;
            elapsedTime = 0f;

            if (recordedFrames.Count >= maxFrames)
            {
                int keep = maxFrames / 2;
                var buf = new FrameData[keep];

                for (int i = 0; i < buf.Length; ++i)
                {
                    if(recordedFrames.Count > 0)
                        buf[i] = recordedFrames.Pop();
                }

                recordedFrames.Clear();

                for (int i = buf.Length - 1; i >= 0; --i)
                {
                    recordedFrames.Push(buf[i]);
                }
            }

            var f = new FrameData()
            {
                position = transform.position,
                rotation = transform.eulerAngles.z,
                localScale = transform.localScale,
                velocity = rb.velocity
            };

            recordedFrames.Push(f);
        }
    }

    IEnumerator PlaybackCoroutine()
    {
        var wait = new WaitForFixedUpdate();

        while (IsRewinding)
        {
            if (recordedFrames.Count == 0)
            {
                StopPlaybackAndClearFrames(false);
                yield break;
            }

            var first = recordedFrames.Pop();
            if (recordedFrames.Count > 0)
            {
                var second = recordedFrames.Peek();

                float elapstedTime = 0f;
                while (elapstedTime < 1f && IsRewinding)
                {
                    elapstedTime += rewindSpeed * Time.fixedDeltaTime;

                    Vector3 pos = Vector3.Lerp(first.position, second.position, elapstedTime);
                    float rot = Mathf.LerpAngle(first.rotation, second.rotation, elapstedTime);

                    transform.SetPositionAndRotation(new Vector3(pos.x, pos.y, transform.position.z),
                                                     Quaternion.Euler(0, 0, rot));
                    transform.localScale = Vector3.Lerp(first.localScale, second.localScale, elapstedTime);

                    yield return wait;
                }
            }
            else
            {
                yield return wait;
            }
        }

        StartRecording();
    }
}