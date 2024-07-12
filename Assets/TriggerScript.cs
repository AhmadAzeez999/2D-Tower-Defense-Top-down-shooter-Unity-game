using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    public bool enemyDetected = false;

    public bool isNextLevelLoader = false;
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] int sceneIndex = 0;

    private void Start()
    {
        InvokeRepeating("NothingDetected", 1f, 3f);
    }
    void NothingDetected()
    {
        enemyDetected = false;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            enemyDetected = true;
        }

        if (isNextLevelLoader && col.CompareTag("Player"))
        {
            levelLoader.LoadThisScene(sceneIndex);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            enemyDetected = false;
        }
    }
}
