using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    [SerializeField] Building buildScript;
    [SerializeField] Collider2D coll2d;

    [SerializeField] float cooldownDuration = 1f;
    float timer;

    bool enemiesDetected;
    Vector3 nearestEnemy;
    Vector3 direction;

    [SerializeField] float range = 2f;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootingForce = 20f;

    public int turretCost = 1000;

    public int maxHealth = 3;
    private int currentHealth;

    [SerializeField] private SpriteRenderer sprite;
    private Color ogColor;

    [SerializeField] bool isDirectional = false;

    [SerializeField] Animator animator;
    const string TURRET_IDLE = "Directional Turret Idle";
    const string TURRET_FIRE = "Directional Turret Fire";
    private string currentState;

    AudioManager audioManager;

    [SerializeField] TriggerScript trigger;

    bool invincible = false;

    public FloatingHealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        ogColor = sprite.color;

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        healthBar.UpdateHealthBar(currentHealth, maxHealth);

        healthBar.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (buildScript.Placed)
        {
            //if (coll2d.enabled == false)
                //coll2d.enabled = true;

            if (!isDirectional)
            {
                if (FindClosestEnemy() != null)
                {
                    nearestEnemy = FindClosestEnemy().transform.position;
                    direction = nearestEnemy - transform.position;
                    direction.Normalize();

                    if (timer > 0)
                        timer -= Time.deltaTime;

                    if (timer <= 0)
                    {
                        audioManager.PlaySFX(audioManager.gunShot);
                        timer = cooldownDuration;
                        GameObject projectile = Instantiate(bullet, transform);
                        projectile.GetComponent<Rigidbody2D>().AddForce(direction * shootingForce, ForceMode2D.Impulse);
                    }
                }
            }
            else
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }

                if (!enemiesDetected)
                {
                    Debug.Log("No enemies detected");
                    ChangeAnimationState(TURRET_IDLE);
                }
            }
        }
        else
        {
            //coll2d.enabled = false;
        }

        if (isDirectional && trigger.enemyDetected && buildScript.Placed)
        {
            if (timer <= 0)
            {
                audioManager.PlaySFX(audioManager.machineGunShots);
                timer = cooldownDuration;
                ChangeAnimationState(TURRET_FIRE);
                GameObject projectile = Instantiate(bullet, transform.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody2D>().AddForce(transform.right * shootingForce, ForceMode2D.Impulse);
            }
        }
    }

    public EnemyMovement FindClosestEnemy()
    {
        float distanceToClosestEnemy = Mathf.Infinity;
        EnemyMovement closestEnemy = null;
        EnemyMovement[] allEnemies = GameObject.FindObjectsOfType<EnemyMovement>();

        if (allEnemies.Length > 0)
        {
            enemiesDetected = true;

        }
        else
        {
            enemiesDetected = false;
            return null;

        }

        Collider2D[] colliderArray = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D collider2D in colliderArray)
        {
            if (collider2D.TryGetComponent<EnemyMovement>(out EnemyMovement currentEnemy))
            {
                float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;
                if (distanceToEnemy < distanceToClosestEnemy)
                {
                    distanceToClosestEnemy = distanceToEnemy;
                    closestEnemy = currentEnemy;
                }

                Debug.DrawLine(this.transform.position, closestEnemy.transform.position);
            }
        }

        return closestEnemy;
    }

    public void TakeDamage(int amount)
    {
        if (invincible)
            return;

        currentHealth -= amount;

        healthBar.gameObject.SetActive(true);
        healthBar.UpdateHealthBar(currentHealth, maxHealth);

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

    void ChangeAnimationState(string newState)
    {
        // Play the animation
        animator.Play(newState);

        // Reassign the current state
        currentState = newState;
    }

    IEnumerator SmallInvincibility()
    {
        yield return new WaitForSeconds(0.5f);

        invincible = false;
    }
}
