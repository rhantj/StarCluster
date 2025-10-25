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
    [SerializeField] Transform returnPoint;

    Vector3Int lastGroundTilePos;

    [Header("Map")]
    [SerializeField] Vector2Int mapSize = new Vector2Int(40, 40);


    private void Awake()
    {
        //planetState = GameManager.Instance.GetPlanetState();
    }

    private void Start()
    {
        SetGround();
    }

    void SetGround()
    {
        for (int i = 0; i < mapSize.x; ++i)
        {
            map.SetTile(new Vector3Int(i, 0, 0), groundTile);
            map.SetTile(new Vector3Int(0, i, 0), wallTile);
            map.SetTile(new Vector3Int(mapSize.x, i, 0), wallTile);
        }

        GenerateMap();
    }

    void GenerateMap()
    {
        var rand = new System.Random();

        var mapWidth = mapSize.x;
        var heightField = new int[mapSize.y];
        int h = 0;

        var terraceJitter = .5f;
        var gapChance = 0.15f;
        var platformChance = .4f;
        //var enemyChance = 0.1f;

        Vector2Int gapLenRange = new(2, 5);
        Vector2Int platformLenRange = new(2, 4);
        Vector2Int platformYOffsetRange = new(2, 5);

        int maxStepUp = 2;
        int maxStepDown = 2;

        for (int x = 0; x < mapWidth; x++)
        {
            if (rand.NextDouble() < terraceJitter)
            {
                double dir = rand.NextDouble();
                if (dir < 0.33) h += rand.Next(0, maxStepUp + 1);
                else if (dir < 0.66) h -= rand.Next(0, maxStepDown + 1);
            }
            h = Mathf.Max(0, h);
            heightField[x] = h;
        }

        // Set Tiles
        int i = 4;
        while (i < mapWidth)
        {
            bool makeGap = rand.NextDouble() < gapChance;
            if (makeGap)
            {
                int gap = rand.Next(gapLenRange.x, gapLenRange.y + 1);
                i += Mathf.Min(gap, mapWidth - i);
                continue;
            }

            int runLen = rand.Next(4, 10);
            runLen = Mathf.Min(runLen, mapWidth - i);

            // ground
            for (int j = 0; j < runLen; j++)
            {
                int x = i + j;
                if (x < 0) continue;

                int y = heightField[x];
                map.SetTile(new Vector3Int(x, y, 0), groundTile);

                if (y < 0) continue;
                map.SetTile(new Vector3Int(x, y + 1, 0), groundTile);

                lastGroundTilePos = new Vector3Int(x - 2, y + 2, 0);
            }

            // platform
            if (platformTile && rand.NextDouble() < platformChance)
            {
                int px = i + rand.Next(0, runLen);
                if (px < 0) px = 0;

                int py = heightField[px] + rand.Next(platformYOffsetRange.x, platformYOffsetRange.y);
                int plen = rand.Next(platformLenRange.x, platformLenRange.y + 1);
                for (int p = 0; p < plen && px + p < mapWidth; p++)
                    map.SetTile(new Vector3Int(px + p, py, 0), platformTile);
            }
            i += runLen;
        }
        
        Instantiate(returnPoint, lastGroundTilePos, Quaternion.identity);
    }
}
