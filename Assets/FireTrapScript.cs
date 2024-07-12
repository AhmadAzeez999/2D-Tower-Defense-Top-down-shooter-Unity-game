using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrapBehaviour))]
public class FireTrapScript : MonoBehaviour
{
    TrapBehaviour trapBehaviour;
    [SerializeField] GameObject firePrefab;

    // Start is called before the first frame update
    void Start()
    {
        trapBehaviour = GetComponent<TrapBehaviour>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent<EnemyStats>(out EnemyStats enemy) && trapBehaviour.canHurt)
        {
            GameObject fire = Instantiate(firePrefab, enemy.transform);
            fire.transform.SetParent(enemy.transform);
        }
    }
}
