using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float damage = 0;
    private void OnTriggerStay2D(Collider2D col)
    {
        col.GetComponent<EnemyStats>().TakeDamage(damage);
    }
}
