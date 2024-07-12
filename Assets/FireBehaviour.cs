using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehaviour : MonoBehaviour
{
    EnemyStats enemy;

    public float burnIterval = 1f;
    public float destroyAfterSeconds = 5f;

    public int damage = 1;

    int maxBurns = 20;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponentInParent<EnemyStats>();

        StartCoroutine(Burn());

        Destroy(gameObject, destroyAfterSeconds);
    }

    private void Update()
    {
        if (enemy)
            transform.position = enemy.transform.position;
        else
            Destroy(gameObject);
    }

    IEnumerator Burn()
    {
        int index = 0;

        while (index < maxBurns)
        {
            yield return new WaitForSeconds(burnIterval);

            enemy.TakeDamage(damage);
        }
    }
}
