using System.Collections;
using System.Collections.Generic;
using Unity.XR.Oculus.Input;
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

        Invoke(nameof(StartRewind), 5f);
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
}
