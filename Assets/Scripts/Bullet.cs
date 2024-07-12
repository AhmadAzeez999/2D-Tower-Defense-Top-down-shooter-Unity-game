using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float destroyAfterSeconds = 3f;
    public float damage = 5;

    Collider2D coll2d;
    Rigidbody2D rb2d;
    SpriteRenderer sprite;
    TrailRenderer trail;

    [SerializeField] ParticleSystem particles;

    AudioManager audioManager;

    public bool isArrow;
    bool hasHitEnemy = false;
    float hitCountdown = 0;
    float hitInterval = 0.1f;

    private void Start()
    {
        coll2d = GetComponent<Collider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();

        Destroy(gameObject, destroyAfterSeconds);

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        if (isArrow && hasHitEnemy)
        {
            hitCountdown -= Time.deltaTime;

            if (hitCountdown <= 0)
            {
                hitCountdown = hitInterval;
                hasHitEnemy = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            if (!isArrow)
            {
                collider.GetComponent<EnemyStats>().TakeDamage(damage);
                StartCoroutine(DestroyBullet());
            }
            else
            {
                if (!hasHitEnemy)
                {
                    collider.GetComponent<EnemyStats>().TakeDamage(damage);
                }
            }
        }

        if (collider.CompareTag("Wall"))
        {
            StartCoroutine(DestroyBullet());
        }
    }

    IEnumerator DestroyBullet()
    {
        if (!isArrow)
        {
            audioManager.PlaySFX(audioManager.bulletDestroy);
            particles.Play();
        }
        coll2d.enabled = false;
        rb2d.velocity = Vector2.zero;
        sprite.enabled = false;
        if (trail)
            trail.enabled = false;

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
