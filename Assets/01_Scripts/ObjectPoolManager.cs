using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }
    public bool IsReady { get; private set; } = false;

    [System.Serializable]
    public class Pool
    {
        public string name;
        public AssetReferenceGameObject prefab;
        public int size;
    }

    public List<Pool> pools = new();
    private Dictionary<string, Queue<GameObject>> poolDictionary = new();
    private Dictionary<string, Pool> poolDataDic = new();
    Transform poolParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        StartCoroutine(CreatePool());
    }

    IEnumerator CreatePool()
    {
        GameObject parentHolder = new("ObjectPoolParent");
        parentHolder.transform.position = Vector2.up * 100;
        DontDestroyOnLoad(parentHolder);
        poolParent = parentHolder.transform;

        foreach (var pool in pools)
        {
            Queue<GameObject> objPool = new();

            for (int i = 0; i < pool.size; ++i)
            {
                var obj = Addressables.InstantiateAsync(pool.prefab, poolParent);
                yield return obj;

                obj.Result.name = pool.name;
                obj.Result.SetActive(false);
                objPool.Enqueue(obj.Result);
            }

            poolDictionary.Add(pool.name, objPool);
            poolDataDic.Add(pool.name, pool);
        }

        IsReady = true;

        var UI = GetComponent<UIManager>();

        UI.SetButtonAction.Invoke();
    }

    public void GetObjectFromPool(string name, out GameObject obj)
    {
        obj = null;
        if (!poolDictionary.TryGetValue(name, out var q))
        {
            return;
        }

        if (q.Count > 0)
        {
            obj = q.Peek();
        }
    }

    public GameObject SpawnFromPool(string name, Vector3 position, out GameObject obj)
    {
        obj = null;
        if (!poolDictionary.TryGetValue(name, out var q))
        {
            Debug.LogError($"Key : {name} is not created in pool");
            return null;
        }

        if(q.Count > 0)
        {
            obj = q.Dequeue();
        }
        else
        {
            if (poolDataDic.TryGetValue(name, out var poolData))
            {
                var handle = Addressables.InstantiateAsync(poolData.prefab, poolParent);
                handle.Result.name = poolData.name;
                obj = handle.Result;
            }
        }

        obj.SetActive(true);
        obj.transform.position = position;
        return obj;
    }

    public void ReturnToPool(string name, GameObject obj)
    {
        if (obj == null) return;

        if (poolDictionary.TryGetValue(name, out var q))
        {
            obj.SetActive(false);
            obj.transform.SetParent(poolParent);
            q.Enqueue(obj);
        }
        else
        {
            // if you use addressables, prevent leaks with this code when a pool is not found.
            Addressables.ReleaseInstance(obj);
        }
    }
}
