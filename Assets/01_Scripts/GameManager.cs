using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    [Header("Planet")]
    PlanetStateMap planetState;

    public void SetPlanetState(PlanetStateMap state)
    {
        planetState = state;
    }

    public PlanetStateMap GetPlanetState()
    {
        return planetState;
    }
}
