using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public static CameraFollowing Instance { get; private set; }

    [Header("Space")]
    [SerializeField] Transform space;
    SpriteRenderer spaceSR;

    GameObject player;
    float camXSize, camYSize;
    float camOffset = -10f;
    Vector3 targetPos;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        player = GameObject.FindGameObjectWithTag("Player");
        if ((int)player.transform.localScale.x == 1) Camera.main.orthographicSize = 5;
        else Camera.main.orthographicSize = 10;

        if (space == null)
        {
            space = GameObject.Find("Space").transform;
        }
        spaceSR = space.GetComponent<SpriteRenderer>();

        camXSize = Camera.main.orthographicSize * Camera.main.aspect;
        camYSize = Camera.main.orthographicSize;
    }

    private void Start()
    {

    }

    private void LateUpdate()
    {
        Following();
    }

    void Following()
    {
        if (player == null) return;
        var x = player.transform.position.x;
        var y = player.transform.position.y;

        Bounds bounds = spaceSR.bounds;
        var minX = bounds.min.x + camXSize;
        var maxX = bounds.max.x - camXSize;
        var minY = bounds.min.y + camYSize;
        var maxY = bounds.max.y - camYSize;

        x = Mathf.Clamp(x, minX, maxX);
        y = Mathf.Clamp(y, minY, maxY);

        targetPos = new Vector3(x, y, camOffset);

        transform.position = targetPos;
    }
}
