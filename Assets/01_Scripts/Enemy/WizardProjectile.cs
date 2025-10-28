using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardProjectile : MonoBehaviour
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
        spriteRenderer.flipX = flip;

        float elapsedTime = 0f;
        while(elapsedTime < 3f)
        {
            elapsedTime += Time.deltaTime;
            transform.Translate(dir * speed * Time.deltaTime, Space.Self);
            yield return null;
        }

        ObjectPoolManager.Instance.ReturnToPool("EnemyWizardProjectile", gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int mask = 1 << 6;
        int layer = collision.gameObject.layer;
        if ((mask & layer) != 1)
        {
            ObjectPoolManager.Instance.ReturnToPool("EnemyWizardProjectile", gameObject);
        }

        if (collision.TryGetComponent<PlayerController>(out var p))
        {
            p.TakeDamage();
        }

        ObjectPoolManager.Instance.ReturnToPool("EnemyWizardProjectile", gameObject);
    }
}
