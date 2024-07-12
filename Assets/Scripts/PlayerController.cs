using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // For movement
    Vector2 moveDirection;
    Vector2 mousePosition;

    // Animation States
    const string PLAYER_IDLE = "Player_idle";
    const string PLAYER_RUNNING = "Player_run";
    const string PLAYER_DODGING = "Player_dodge";

    Animator animator;
    private string currentState;

    // For dodging
    private bool canDodge = true;
    [HideInInspector] public bool isDodging;
    private float dodgeCooldown = 1f;
    private Collider2D coll2d;

    // Player mode
    PlayerModeChange playerMode;
    PlayerHealth health;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Rigidbody2D rb;

    [Header("Dodging")]
    [SerializeField] private float dodgeSpeed = 24f;
    [SerializeField] private float dodgeTime = 0.2f;
    [SerializeField] private TrailRenderer tr;

    [Header("Weapon")]
    [SerializeField] Weapon weapon;
    [SerializeField] SpriteRenderer weaponSprite;

    [SerializeField] int startingCoins = 2000;

    [SerializeField] EnemySpawner enemySpawner;

    AudioManager audioManager;

    Vector2 prevPosition;

    [SerializeField] bool trapsAvailable = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        coll2d = GetComponent<Collider2D>();

        playerMode = GetComponent<PlayerModeChange>();
        health = GetComponent<PlayerHealth>();

        GameManager.instance.AddCoins(startingCoins);

        audioManager = audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDodging)
        {
            return;
        }

        if(health.tempDead)
        {
            transform.position = prevPosition;
            return;
        }

        prevPosition = transform.position;
        
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - transform.position;

        moveDirection = new Vector2(moveX, moveY).normalized;

        if (currentState != PLAYER_DODGING)
        {
            weaponSprite.enabled = true;

            if (moveDirection.x != 0 || moveDirection.y != 0)
            {
                ChangeAnimationState(PLAYER_RUNNING);
            }
            else
            {
                ChangeAnimationState(PLAYER_IDLE);
            }
        }

        // To make player face move direction
        Vector2 scale = transform.localScale;
        if (direction.x < 0)
        {
            scale.x = -1f;
        }
        else if (direction.x > 0)
        {
            scale.x = 1f;
        }

        transform.localScale = scale;

        if (Input.GetKeyDown(KeyCode.Space) && canDodge)
        {
            if (moveDirection.x < 0)
            {
                scale.x = -1f;
            }
            else if (moveDirection.x > 0)
            {
                scale.x = 1f;
            }
            transform.localScale = scale;

            ChangeAnimationState(PLAYER_DODGING);
            StartCoroutine(Dodge());
        }

        if (Input.GetKeyDown(KeyCode.E) && trapsAvailable)
        {
            if (playerMode.currentMode != PlayerModeChange.PlayerMode.Sell)
                playerMode.SellMode();
            else
                playerMode.CombatMode();
        }

        if (enemySpawner && trapsAvailable)
        {
            if (Input.GetKeyDown(KeyCode.G) && !enemySpawner.gameStarted)
            {
                enemySpawner.playerIsReady = true;
            }
        }

    }

    private void FixedUpdate()
    {
        if (isDodging)
        {
            return;
        }

        // To move the player around
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
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

    private IEnumerator Dodge()
    {
        weaponSprite.enabled = false;
        Physics2D.IgnoreLayerCollision(3, 2, true);
        Physics2D.IgnoreLayerCollision(3, 6, true);
        Physics2D.IgnoreLayerCollision(3, 7, true);
        Physics2D.IgnoreLayerCollision(3, 9, true);
        canDodge = false;
        isDodging = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(moveDirection.x * dodgeSpeed, moveDirection.y * dodgeSpeed);
        tr.emitting = true;

        yield return new WaitForSeconds(dodgeTime);
        Physics2D.IgnoreLayerCollision(3, 2, false);
        Physics2D.IgnoreLayerCollision(3, 6, false);
        Physics2D.IgnoreLayerCollision(3, 7, false);
        Physics2D.IgnoreLayerCollision(3, 9, false);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDodging = false;

        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }
}
