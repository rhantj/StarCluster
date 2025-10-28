using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerate : MonoBehaviour
{
    PlanetStateMap planetState;

    [Header("Tiles")]
    [SerializeField] Tilemap map;
    [SerializeField] TileBase groundTile;
    [SerializeField] TileBase wallTile;
    [SerializeField] TileBase platformTile;

    [Header("Elements")]
    [SerializeField] Transform returnPoint;

    Vector3Int lastGroundTilePos;

    [Header("Map")]
    [SerializeField] Vector2Int mapSize = new Vector2Int(40, 40);
    [SerializeField] ClearControl clearCanv;


    private void Awake()
    {
        planetState = GameManager.Instance.GetPlanetState();
    }

    private void OnEnable()
    {
        SetGround();
        GenerateMap();
    }

    void SetGround()
    {
        for (int i = 0; i < mapSize.x; ++i)
        {
            map.SetTile(new Vector3Int(i, 0, 0), groundTile);
            map.SetTile(new Vector3Int(i, 17, 0), groundTile);

            map.SetTile(new Vector3Int(0, i, 0), wallTile);
            map.SetTile(new Vector3Int(mapSize.x, i, 0), wallTile);
        }
    }

    void GenerateMap()
    {
        var rand = new System.Random();

        var mapWidth = mapSize.x;
        var heights = new int[mapSize.y];
        int h = 0;

        var randomWalk = .5f;
        var gapChance = 0.15f;
        var platformChance = .2f;
        var enemyChance = 0.1f;

        Vector2Int gapLenRange = new(2, 5);
        Vector2Int platformLenRange = new(2, 4);
        Vector2Int platformYOffsetRange = new(2, 5);

        for (int x = 0; x < mapWidth; ++x)
        {
            if (rand.NextDouble() < randomWalk)
            {
                var dir = rand.NextDouble();
                var p = 0.33;
                if (dir < p) h += rand.Next(0, 3);
                else if (dir < p * 2) h -= rand.Next(0, 3);
            }
            h = Mathf.Max(0, h);
            heights[x] = h;
        }

        // Set Tiles
        int i = 4;
        while (i < mapWidth)
        {
            bool makeGap = rand.NextDouble() < gapChance;
            if (makeGap)
            {
                int gap = rand.Next(gapLenRange.x, gapLenRange.y + 1);
                if (gap < 2) gap = 2;

                i += Mathf.Min(gap, mapWidth - i);
            }

            int runLen = rand.Next(4, 10);
            runLen = Mathf.Min(runLen, mapWidth - i);

            // ground
            int before = 0;
            for (int j = 0; j < runLen; ++j)
            {
                int x = i + j;
                int gap = x - before;
                if (x < 0) continue; 
                if (gap > 5) x -= 3;

                int y = heights[x];
                map.SetTile(new Vector3Int(x, y, 0), groundTile);

                if (y < 0) continue;
                map.SetTile(new Vector3Int(x, y + 1, 0), groundTile);

                before = x;
                lastGroundTilePos = new Vector3Int(x, y, 0);

                //enemy
                if (rand.NextDouble() < enemyChance && planetState == PlanetStateMap.Combat)
                {
                    if(ObjectPoolManager.Instance.SpawnFromPool("Enemy_Wizard", lastGroundTilePos + Vector3.up * 3f, out var e))
                    {
                        clearCanv.PlusEnemyCount(); // enemy += 1
                        float a = Mathf.Max(8, lastGroundTilePos.x);
                        float b = lastGroundTilePos.y;

                        var spawnPos = new Vector3(a, b + 3f, 0);
                        e.GetComponent<Enemy_Wizard>().SetStartPosition(spawnPos);
                    }
                }
            }

            // platform
            if (platformTile && rand.NextDouble() < platformChance)
            {
                int px = i + rand.Next(0, runLen);
                px = Mathf.Min(px, heights.Length - 1);
                if (px < 0) px = 0;

                int py = heights[px] + rand.Next(platformYOffsetRange.x, platformYOffsetRange.y);
                int plen = rand.Next(platformLenRange.x, platformLenRange.y + 1);

                for (int p = 0; p < plen && px + p < mapWidth; ++p)
                    map.SetTile(new Vector3Int(px + p, py, 0), platformTile);
            }

            i += runLen;
        }

        Instantiate(returnPoint, lastGroundTilePos + new Vector3Int(-2, 3, 0), Quaternion.identity);
    }
}
