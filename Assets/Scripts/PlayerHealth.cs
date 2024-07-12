using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public float currentHealth;

    private SpriteRenderer sprite;
    private Color ogColor;
    [SerializeField] private Color invincColor;
    private bool isInvincible;

    private PlayerController player;

    AudioManager audioManager;

    [SerializeField] Transform respawnPoint;
    [SerializeField] float respawnTime = 5f;
    [SerializeField] GameObject respawnUI;
    [SerializeField] Slider respawnSlider;
    public bool tempDead = false;
    float respawnCountdown = 5f;

    Collider2D coll2d;

    PlayerModeChange playerMode;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        ogColor = sprite.color;
        player = GetComponent<PlayerController>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        coll2d = GetComponent<Collider2D>();

        respawnSlider.maxValue = respawnCountdown;

        playerMode = GetComponent<PlayerModeChange>();
    }

    private void Update()
    {
        if (tempDead)
        {
            respawnCountdown -= Time.deltaTime;
            respawnSlider.value = respawnCountdown;

            if (respawnCountdown <= 0)
            {
                respawnCountdown = respawnTime;
                Respawn();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isInvincible)
        {
            Physics2D.IgnoreLayerCollision(3, 7, true);
        }
        else if (!isInvincible && !player.isDodging)
        {
            Physics2D.IgnoreLayerCollision(3, 7, false);
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isInvincible)
        {
            audioManager.PlaySFX(audioManager.hurt);
            currentHealth -= amount;

            if (currentHealth > 0)
            {
                BecomeInvincible();
            }
            else
            {
                TempDeath();
            }
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void BecomeInvincible()
    {
        isInvincible = true;
        StartCoroutine(Invincibility());
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;
        sprite.color = invincColor;

        yield return new WaitForSeconds(2);
        isInvincible = false;
        sprite.color = ogColor;
    }

    void TempDeath()
    {
        GridBuildingSystem.current.ExitBuildMode();
        playerMode.CombatMode();
        coll2d.enabled = false;
        tempDead = true;
        sprite.enabled = false;
        respawnUI.SetActive(true);
    }

    void Respawn()
    {
        BecomeInvincible();
        coll2d.enabled = true;
        tempDead = false;
        transform.position = respawnPoint.position;
        sprite.enabled = true;
        currentHealth = maxHealth;
        respawnUI.SetActive(false);
    }
}
