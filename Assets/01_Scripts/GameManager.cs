using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    [Header("Planet")]
    PlanetStateMap planetState;

    [Header("Players")]
    GameObject player;
    GameObject playerShip;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += SetPlayer;
    }

    private void SetPlayer(Scene arg0, Scene arg1)
    {
        if(arg1.name.Equals("Space Scene"))
        {
            if(player != null)
            {
                ObjectPoolManager.Instance.ReturnToPool("Player", player);
            }

            ObjectPoolManager.Instance.TrySpawnFromPool("Player_SpaceShip", Vector3.zero, out var pss);
            playerShip = pss;

        }
        else if (arg1.name.Equals("Planet"))
        {
            if(playerShip != null)
            {
                ObjectPoolManager.Instance.ReturnToPool("Player_SpaceShip", playerShip);
            }

            ObjectPoolManager.Instance.TrySpawnFromPool("Player", new Vector3(2.16f, 2.3f, 0), out var p);
            player = p;
        }
    }

    public void SetPlanetState(PlanetStateMap state)
    {
        planetState = state;
    }

    public PlanetStateMap GetPlanetState()
    {
        return planetState;
    }
}
