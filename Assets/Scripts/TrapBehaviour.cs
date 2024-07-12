using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBehaviour : MonoBehaviour
{
    [SerializeField] Building buildScript;
    [SerializeField] SpriteRenderer spriteRenderer;

    public bool canHurt = false;

    [SerializeField] float cooldownDuration = 1f;
    float timer;

    Collider2D coll2d;

    // Animation States
    const string TRAP_IDLE = "Trap_idle";
    const string TRAP_PLACED = "Trap_placing";
    const string TRAP_ATTACK = "Trap_attack";

    Animator animator;
    private string currentState;

    public float damage = 10;

    public bool isWallTrap = false;
    public GameObject wallTrapProjectile;
    public float fireForce = 20f;

    public float waitBeforeAttack = 0.3f;

    public int trapCost = 400;

    public bool isBomb = false;
    public GameObject explosion;

    AudioManager audioManager;

    public bool doesDamage = true;
    public int maxHealth = 3;
    public int currentHealth = 0;
    bool invincible = false;

    public Sprite[] sprites;

    private void Awake()
    {
        coll2d = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
    }

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Update()
    {
        if (!doesDamage)
            return;

        if (buildScript.Placed)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // Opaque
            canHurt = true;
        }
        else
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, .5f); // Transparent
        }

        if (timer > 0)
            timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = cooldownDuration;
            coll2d.enabled = true;
            ChangeAnimationState(TRAP_IDLE);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!doesDamage)
        {
            return;
        }

        if (canHurt && collider.CompareTag("Enemy"))
        {
            StartCoroutine(Attack(collider));
        }
    }

    void ChangeAnimationState(string newState)
    {
        // Stop the same animation from interrupting itself
        if (currentState == newState) return;

        // Play the animation
        animator.Play(newState);

        // Reassign the current state
        currentState = newState;
    }

    public void PlayPlacingAnimation()
    {
        if (animator)
            animator.Play(TRAP_PLACED);
        currentState = TRAP_PLACED;
    }

    IEnumerator Attack(Collider2D collider)
    {
        yield return new WaitForSeconds(waitBeforeAttack);

        ChangeAnimationState(TRAP_ATTACK);
        if (isWallTrap)
        {
            GameObject projectile = Instantiate(wallTrapProjectile, transform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().AddForce(-transform.up * fireForce, ForceMode2D.Impulse);
            projectile.GetComponent<Bullet>().damage = damage;
        }
        else if (isBomb)
        {
            audioManager.PlaySFX(audioManager.explosion);
            CinemachineShake.Instance.ShakeCamera(2, 0.1f);
            spriteRenderer.enabled = false;
            explosion.SetActive(true);
            explosion.GetComponent<ExplosionScript>().damage = damage;

            if (collider)
                collider.GetComponent<EnemyStats>().TakeDamage(damage);

            Debug.Log("Bomb will destroy in 1 second");
            StartCoroutine(DestroyBomb());
        }
        else
        {
            audioManager.PlaySFX(audioManager.spikeTrap);
            if (collider != null)
                collider.GetComponent<EnemyStats>().TakeDamage(damage);
        }
        coll2d.enabled = false;
    }

    IEnumerator DestroyBomb()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("Bomb should destroy");
        buildScript.DestroyBuild(false);
    }

    public void TakeDamage(int amount)
    {
        if (doesDamage || invincible)
            return;

        currentHealth -= amount;

        if (!doesDamage)
            UpdateSprite();

        if (currentHealth > 0)
        {
            invincible = true;
            StartCoroutine(SmallInvincibility());
        }
        else
        {
            buildScript.DestroyBuild(false);
        }
    }

    IEnumerator SmallInvincibility()
    {
        yield return new WaitForSeconds(0.5f);

        invincible = false;
    }

    void UpdateSprite()
    {
        if ((currentHealth <= maxHealth * 3 / 4) && (currentHealth >= maxHealth / 2))
        {
            spriteRenderer.sprite = sprites[1];
        }
        else if ((currentHealth <= maxHealth * 3 / 4) && (currentHealth >= maxHealth / 4)) 
        {
            spriteRenderer.sprite = sprites[2];
        }
        else if (currentHealth <= maxHealth / 4)
        {
            spriteRenderer.sprite = sprites[3];
        }
    }
}
