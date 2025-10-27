using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    bool isUpdatingRef;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }


    private void LateUpdate()
    {
        if(player == null || space == null)
        {
            SetReferences();
            return;
        }

        Following();
    }

    private void OnActiveSceneChanged(Scene arg0, Scene arg1)
    {
        StartCoroutine(Co_SetReferences());
    }


    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(Co_SetReferences());
    }

    IEnumerator Co_SetReferences()
    {
        yield return null;
        SetReferences();

        if(player == null || space == null)
        {
            if (!isUpdatingRef)
            {
                isUpdatingRef = true;

                float elapsedTime = 0f;
                while (elapsedTime < 0.5f && (player == null || space == null))
                {
                    SetReferences();
                    elapsedTime += Time.fixedDeltaTime;
                    yield return null;
                }

                isUpdatingRef = false;
            }
        }
    }

    void SetReferences()
    {
        var sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Equals("Title Scene")) return;
        if (player == null) 
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if ((int)player.transform.localScale.x == 1) Camera.main.orthographicSize = 5;
            else Camera.main.orthographicSize = 10;
        }

        if (space == null)
        {
            space = GameObject.FindGameObjectWithTag("Space").transform;
            spaceSR = space.GetComponent<SpriteRenderer>();
        }

        if (Camera.main != null)
        {
            camXSize = Camera.main.orthographicSize * Camera.main.aspect;
            camYSize = Camera.main.orthographicSize;
        }

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
