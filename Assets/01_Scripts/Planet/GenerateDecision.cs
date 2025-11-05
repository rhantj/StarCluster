using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateDecision : MonoBehaviour
{
    [SerializeField] PlanetStateMap planetState;

    [Header("Tiles")]
    [SerializeField] Tilemap map;
    [SerializeField] TileBase groundTile;
    [SerializeField] TileBase platformTile;

    [Header("Elements")]
    [SerializeField] Transform returnPoint;

    [Header("Map")]
    [SerializeField] Vector2Int mapSize = new Vector2Int(40, 40);
    [SerializeField] ClearControl clearCanv;

    [Header("Adventure Mode Tile Map")]
    [SerializeField] GameObject adventureTilemap;

    private void Awake()
    {
        planetState = GameManager.Instance.GetPlanetState();
        switch (planetState)
        {
            case PlanetStateMap.Adventure:
                adventureTilemap.SetActive(true);

                break;

            case PlanetStateMap.Combat:
                var generator = gameObject.AddComponent<MapGenerate>();

                generator.Initialize(map, groundTile, platformTile, returnPoint, mapSize, clearCanv);
                break;
        }
    }

    private void Start()
    {
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGM($"Planet_{planetState}");
    }
}