using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    public GameObject currentTarget;
    private Rigidbody2D rb;
    public float force;

    public int damage;

    public int destroyAfterSeconds = 2;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Vector3 direction = currentTarget.transform.position - transform.position;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;

        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);

        Destroy(gameObject, destroyAfterSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            collider.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }

        if (collider.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        if (collider.CompareTag("Barrel"))
        {
            collider.gameObject.GetComponent<TrapBehaviour>().TakeDamage(damage);
            Destroy(gameObject);
        }

        if (collider.transform.tag == "Gate")
        {
            collider.GetComponent<GateHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }

        if (collider.transform.tag == "Turret")
        {
            collider.gameObject.GetComponent<TurretBehaviour>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
