using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class SideScrollerGenerator2D : MonoBehaviour
{
    [Header("Tilemap Reference")]
    public Tilemap tilemap; // 실제 타일맵

    [Header("Tiles")]
    public TileBase groundTile;
    public TileBase platformTile;
    public GameObject enemyTile;

    [Header("Map Settings")]
    [Range(5, 40)] public int mapWidth = 40;  // X = 0 ~ 39
    public int baseGroundY = 0;
    public int maxStepUp = 2;
    public int maxStepDown = 2;

    [Range(0f, 1f)] public float terraceJitter = 0.4f;
    [Range(0f, 1f)] public float gapChance = 0.15f;
    public Vector2Int gapLenRange = new Vector2Int(2, 5);

    [Header("Platforms")]
    [Range(0f, 1f)] public float platformChance = 0.3f;
    public Vector2Int platformLenRange = new Vector2Int(2, 4);
    public Vector2Int platformYOffsetRange = new Vector2Int(2, 5);

    [Header("Enemies")]
    [Range(0f, 1f)] public float enemyChance = 0.1f;

    [Header("Random Seed")]
    public bool useFixedSeed = true;
    public int seed = 1234;

    int[] heightField;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        if (!tilemap || !groundTile)
        {
            Debug.LogWarning("Tilemap or GroundTile not assigned!");
            return;
        }

        // 초기화
        var rand = useFixedSeed ? new System.Random(seed) : new System.Random();

        // 높이 필드 생성
        heightField = new int[mapWidth];
        int h = baseGroundY;
        for (int x = 0; x < mapWidth; x++)
        {
            if (rand.NextDouble() < terraceJitter)
            {
                double dir = rand.NextDouble();
                if (dir < 0.33) h += rand.Next(0, maxStepUp + 1);
                else if (dir < 0.66) h -= rand.Next(0, maxStepDown + 1);
            }
            h = Mathf.Max(baseGroundY - 2, h);
            heightField[x] = h;
        }

        // 타일 배치 (x >= 0 영역만)
        int i = 0;
        while (i < mapWidth)
        {
            bool makeGap = rand.NextDouble() < gapChance;
            if (makeGap)
            {
                int gapLen = rand.Next(gapLenRange.x, gapLenRange.y + 1);
                i += Mathf.Min(gapLen, mapWidth - i);
                continue;
            }

            int runLen = rand.Next(4, 10);
            runLen = Mathf.Min(runLen, mapWidth - i);

            for (int j = 0; j < runLen; j++)
            {
                int x = i + j;
                if (x < 0) continue; // 음수 방향 방지

                int y = heightField[x];

                // 바닥
                tilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
                tilemap.SetTile(new Vector3Int(x, y - 1, 0), groundTile);

                // 적
                if (enemyTile && rand.NextDouble() < enemyChance)
                    Instantiate(enemyTile, new Vector3Int(x, y + 1, 0), Quaternion.identity);
                    //tilemap.SetTile(new Vector3Int(x, y + 1, 0), enemyTile);
            }

            // 발판
            if (platformTile && rand.NextDouble() < platformChance)
            {
                int px = i + rand.Next(0, runLen);
                if (px < 0) px = 0; // 안전 보정

                int py = heightField[px] + rand.Next(platformYOffsetRange.x, platformYOffsetRange.y);
                int plen = rand.Next(platformLenRange.x, platformLenRange.y + 1);
                for (int p = 0; p < plen && px + p < mapWidth; p++)
                    tilemap.SetTile(new Vector3Int(px + p, py, 0), platformTile);
            }

            i += runLen;
        }
    }
}
