using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPos;
    public float shotInterval = 5f;
    public float range = 3f;

    public Rigidbody2D rb;

    private float timer;

    Transform closestTarget;
    [SerializeField] EnemyMovement enemyMovement;

    public bool shot = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Detector())
        {
            float distance = Vector2.Distance(rb.position, closestTarget.position);


            if (Detector() && distance < range)
            {
                enemyMovement.canMove = false;
                timer += Time.deltaTime;

                if (timer > shotInterval)
                {
                    timer = 0;
                    Shoot();
                }
            }
            else
            {
                enemyMovement.canMove = true;
            }
        }
        else
        {
            enemyMovement.canMove = true;
        }
    }

    void Shoot()
    {
        shot = true;
        GameObject projectile = Instantiate(bullet, bulletPos.position, Quaternion.identity);
        projectile.GetComponent<EnemyBulletScript>().currentTarget = closestTarget.gameObject;
    }

    bool Detector()
    {
        Collider2D[] colliderArray = Physics2D.OverlapCircleAll(transform.position, range);

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

}
