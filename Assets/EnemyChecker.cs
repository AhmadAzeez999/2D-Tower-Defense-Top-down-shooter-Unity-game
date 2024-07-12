using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChecker : MonoBehaviour
{
    GameObject[] totalEnemies;

    [SerializeField] DialogueScript dialogueScript;
    [SerializeField] bool shouldCheckEnemies = true;
    bool dialogueStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!shouldCheckEnemies)
            return;

        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (totalEnemies.Length == 0 && !dialogueStarted)
        {
            dialogueScript.gameObject.SetActive(true);
            dialogueStarted = true;
        }
    }
}
