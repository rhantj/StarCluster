using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    Dictionary<string, AudioClip> sfxClips = new();
    Dictionary<string, AudioClip> bgmClips = new();

    [Header("Preload Clips")]
    [SerializeField] AudioClip[] preloadSFXs;
    [SerializeField] AudioClip[] preloadBGMs;

    [Header("SFX Pool Size")]
    int sfxPoolSize = 5;
    Queue<AudioSource> sfxPool = new();
    List<AudioSource> activeSfx = new();
    GameObject root;

    [Header("BGM Settings")]
    float bgmVolume = 0.05f;
    AudioSource bgmSrc;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Preloading();
        SetBGM();
        SetSFXPool();
    }

    void Preloading()
    {
        foreach (AudioClip clip in preloadSFXs)
        {
            if (!sfxClips.ContainsKey(clip.name))
                sfxClips.Add(clip.name, clip);
        }

        foreach (AudioClip clip in preloadBGMs)
        {
            if (!bgmClips.ContainsKey(clip.name))
                bgmClips.Add(clip.name, clip);
        }
    }

    void SetBGM()
    {
        var p = new GameObject("BGM_Channels");
        p.transform.SetParent(transform);

        bgmSrc = p.AddComponent<AudioSource>();
        bgmSrc.loop = true;
        bgmSrc.volume = bgmVolume;
        bgmSrc.playOnAwake = false;
    }

    void SetSFXPool()
    {
        root = new GameObject("SFX_Pool");
        root.transform.SetParent(transform);

        for (int i = 0; i < sfxPoolSize; ++i)
        {
            var obj = new GameObject("SFX " + i);
            obj.transform.SetParent(root.transform);
            
            var src = obj.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 1f;

            obj.SetActive(false);
            sfxPool.Enqueue(src);
        }
    }

    public void PlaySFX(string clipName, Vector3 pos, float volume = 0f)
    {
        if (!sfxClips.TryGetValue(clipName, out var clip))
        {
            Debug.LogError(clipName + " is not founded");
            return;
        }

        if(sfxPool.Count == 0)
        {
            var obj = new GameObject("SFX " + (sfxPool.Count + activeSfx.Count));
            obj.transform.SetParent(root.transform);

            var src = obj.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 1f;

            obj.SetActive(false);
            sfxPool.Enqueue(src);
        }

        var sfx = sfxPool.Dequeue();
        sfx.clip = clip;
        sfx.volume = volume;

        sfx.transform.position = pos;
        sfx.gameObject.SetActive(true);
        sfx.Play();

        activeSfx.Add(sfx);
        StartCoroutine(Co_RTPafterplay(sfx));
    }

    IEnumerator Co_RTPafterplay(AudioSource src)
    {
        yield return new WaitWhile(() => src.isPlaying);

        src.Stop();
        src.clip = null;
        src.gameObject.SetActive(false);

        activeSfx.Remove(src);
        sfxPool.Enqueue(src);
    }

    public void PlayBGM(string clipName)
    {
        if (!bgmClips.TryGetValue(clipName, out var clip))
        {
            Debug.LogError(clipName + " not found");
            return;
        }

        bgmSrc.clip = clip;
        bgmSrc.Play();
    }

    public void StopAllSFX()
    {
        foreach (var src in activeSfx)
        {
            src.Stop();
            src.gameObject.SetActive(false);
            sfxPool.Enqueue(src);
        }

        activeSfx.Clear();
    }

    public void StopBGM()
    {
        bgmSrc.Stop();
    }

    public void SetBGMVolume(float val)
    {
        bgmSrc.volume = val;
    }

    public void SetSFXVolume(float val)
    {
        foreach(var src in activeSfx)
            src.volume = val;

        foreach(var src in sfxPool)
            src.volume = val;
    }
}
