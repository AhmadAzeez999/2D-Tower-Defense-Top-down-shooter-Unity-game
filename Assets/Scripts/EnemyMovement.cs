using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyMovement : MonoBehaviour
{
    public float currentMoveSpeed;

    PlayerHealth pl;
    GameObject gate;

    Vector2 currentTargetPosition;

    public Transform enemyGFX;

    public int damage = 1;
    public bool canMove = true;

    const string ENEMY_IDLE = "Enemy_Idle";
    const string ENEMY_RUN = "Enemy_run";
    const string ENEMY_ATTACK = "Enemy_Attack";

    [SerializeField] Animator animator;
    private string currentState;

    [SerializeField] EnemyShooting enemyShooting;
    public bool isRanged = false;

    public float attackInterval = 3f;
    public float attackRange = 1f;
    float timer = 0;

    Transform closestTarget;
    public Collider2D meleeWeaponCol;

    public bool quickEnemy = false;

    [SerializeField] bool gateIsTarget = true;

    [SerializeField] bool isBoss = false;

    // Start is called before the first frame update
    void Start()
    {

        pl = FindObjectOfType<PlayerHealth>();

        gate = GameObject.FindGameObjectWithTag("Gate");

        float randomNum = UnityEngine.Random.Range(-2f, 2f);

        if (gateIsTarget)
            currentTargetPosition = new Vector2(gate.transform.position.x, gate.transform.position.y + randomNum);
        else
            currentTargetPosition = new Vector2(pl.transform.position.x, pl.transform.position.y + randomNum);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 force = currentTargetPosition * currentMoveSpeed * Time.deltaTime;

        if (!isBoss)
        {
            if (force.x >= 0.01f)
            {
                enemyGFX.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (force.x <= 0.01f)
            {
                enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
            }
        }

        if (quickEnemy)
        {
            if (!gateIsTarget)
            {
                if (GameManager.Instance.isChatting == true)
                    return;
                currentTargetPosition = new Vector2(pl.transform.position.x, pl.transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, currentTargetPosition, currentMoveSpeed * Time.deltaTime);
            }
            else
                transform.position = Vector2.MoveTowards(transform.position, currentTargetPosition, currentMoveSpeed * Time.deltaTime);
            return;
        }

        if (!isRanged)
        {

            if (Detector())
            {
                canMove = false;
                transform.position = Vector2.MoveTowards(transform.position, closestTarget.transform.position, currentMoveSpeed * Time.deltaTime);
            }
            else
            {
                canMove = true;
            }
        }

        if (canMove)
        {
            timer = attackInterval;
            ChangeAnimationState(ENEMY_RUN);
            transform.position = Vector2.MoveTowards(transform.position, currentTargetPosition, currentMoveSpeed * Time.deltaTime);
        }
        else if (isRanged)
        {
            ChangeAnimationState(ENEMY_ATTACK);

            if (enemyShooting && enemyShooting.shot == true)
            {
                ChangeAnimationState(ENEMY_IDLE);
                enemyShooting.shot = false;
            }
        }
        else
        {
            if (currentState != ENEMY_ATTACK)
            {
                ChangeAnimationState(ENEMY_IDLE);
            }

            timer += Time.deltaTime;

            if (timer > attackInterval)
            {
                timer = 0;
                Attack();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (quickEnemy)
        {
            if (collision.gameObject.TryGetComponent<GateHealth>(out GateHealth gate))
            {
                if (isBoss)
                    gate.TakeDamage(1000);
                else
                    gate.TakeDamage(damage);

                Destroy(gameObject);
            }

            if (collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth player))
            {
                player.TakeDamage(damage);
                if (!isBoss)
                    Destroy(gameObject);
            }
        }

        if (isBoss)
        {
            if (collision.gameObject.TryGetComponent<Building>(out Building build))
            {
                build.DestroyBuild();
            }
        }
    }

    void Attack()
    {
        ChangeAnimationState(ENEMY_ATTACK);
    }

    bool Detector()
    {
        Collider2D[] colliderArray = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D collider2D in colliderArray)
        {
            if (collider2D.transform.tag == "Player" || collider2D.transform.tag == "Turret" ||
                collider2D.transform.tag == "Barrel" || collider2D.transform.tag == "Gate")
            {
                closestTarget = collider2D.transform;

                if (closestTarget.gameObject.TryGetComponent<Building>(out Building trapBuilding))
                {
                    if (!trapBuilding.Placed)
                    {
                        return false;
                    }
                }

                Debug.DrawLine(this.transform.position, closestTarget.transform.position);
                return true;
            }
        }

        return false;
    }

    public void ToggleMeleeCollider()
    {
        if (meleeWeaponCol.enabled == true)
            meleeWeaponCol.enabled = false;
        else
            meleeWeaponCol.enabled = true;
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

}
