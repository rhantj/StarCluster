using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Fire(Vector2 dir, float speed = 8f, bool flip = false)
    {
        StartCoroutine(Co_Fire(dir, speed, flip));
    }

    IEnumerator Co_Fire(Vector2 dir, float speed, bool flip)
    {
        spriteRenderer.flipY = flip;

        float elapsedTime = 0f;
        while (elapsedTime < 3f)
        {
            elapsedTime += Time.deltaTime;
            transform.Translate(dir * speed * Time.deltaTime, Space.Self);
            yield return null;
        }

        ObjectPoolManager.Instance.ReturnToPool(nameof(PlayerProjectile), gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int mask = 1 << 6;
        int layer = collision.gameObject.layer;
        if ((mask & layer) != 1)
        {
            ObjectPoolManager.Instance.ReturnToPool(nameof(PlayerProjectile), gameObject);
        }

        if (collision.TryGetComponent<Enemy_Wizard>(out var e))
        {
            e.TakeDamage(2);
            ObjectPoolManager.Instance.ReturnToPool(nameof(PlayerProjectile), gameObject);
        }

    }
}
