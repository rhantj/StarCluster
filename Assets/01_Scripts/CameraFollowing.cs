using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    [Header("Space")]
    [SerializeField] Transform space;
    SpriteRenderer spaceSR;

    GameObject player;
    float camXSize, camYSize;
    float camOffset = -10f;
    Vector3 targetPos;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        spaceSR = space.GetComponent<SpriteRenderer>();

        DontDestroyOnLoad(gameObject);

        camXSize = Camera.main.orthographicSize * Camera.main.aspect;
        camYSize = Camera.main.orthographicSize;
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
