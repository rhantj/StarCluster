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
    [SerializeField] TileBase wallTile;
    [SerializeField] TileBase platformTile;

    [Header("Elements")]
    [SerializeField] Transform returnPoint;

    [Header("Map")]
    [SerializeField] Vector2Int mapSize = new Vector2Int(40, 40);
    [SerializeField] ClearControl clearCanv;

    private void Awake()
    {
        planetState = GameManager.Instance.GetPlanetState();
        switch (planetState)
        {
            case PlanetStateMap.Adventure:
                gameObject.AddComponent<MapGenerate2>();
                
                break;

            case PlanetStateMap.Combat:
                var generator = gameObject.AddComponent<MapGenerate>();

                generator.Initialize(map, groundTile, wallTile, platformTile, returnPoint, mapSize, clearCanv);
                break;
        }
    }
}