using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    int count = 30;
    float radius = 5f;

    private void Awake()
    {
        Spawning();
    }

    void Spawning()
    {
        for (int i = 0; i < count; ++i)
        {
            float angel = i * Mathf.PI * 2f / count;
            float x = transform.position.x + Mathf.Cos(angel) * radius;
            float y = transform.position.y + Mathf.Sin(angel) * radius;

            var pos = new Vector2 (x, y);

            var p = Instantiate(prefab, pos, Quaternion.identity);
            p.GetComponent<Rigidbody2D>().AddForce(new Vector2(x, y) * 8f, ForceMode2D.Impulse);
        }
    }
}
