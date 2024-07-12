using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    public int damage = 1;
    GateHealth gatehealth;
    PlayerHealth pl;

    private void Start()
    {
        pl = FindObjectOfType<PlayerHealth>();

        gatehealth = FindObjectOfType<GateHealth>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            pl.TakeDamage(damage);
        }

        if (collision.transform.tag == "Turret")
        {
            collision.gameObject.GetComponent<TurretBehaviour>().TakeDamage(damage);
        }

        if (collision.transform.tag == "Barrel")
        {
            Debug.Log("Barrel");
            collision.gameObject.GetComponent<TrapBehaviour>().TakeDamage(damage);
        }

        if (collision.transform.tag == "Gate")
        {
            gatehealth.TakeDamage(damage);

        }
    }
}
