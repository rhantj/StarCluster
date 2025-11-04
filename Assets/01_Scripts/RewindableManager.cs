using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RewindableManager : MonoBehaviour
{
    public static RewindableManager Instance { get; private set; }
    List<IRewindable> rewindables = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        rewindables.Add(player.GetComponent<IRewindable>());
    }

    public void Registration(IRewindable rw)
    {
        if (!rewindables.Contains(rw))
            rewindables.Add(rw);
    }

    public void Unregister(IRewindable rw)
    {
        rewindables.Remove(rw);
    }

    public void StartRewind()
    {
        foreach (var r in rewindables)
            r.Rewind();
    }

    public void StopRewind()
    {
        foreach (var r in rewindables)
            r.StopRewind();
    }
}
