using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShotAtDialogue : MonoBehaviour
{
    [SerializeField] string[] dialogues;

    string chosenDialogue;

    [SerializeField] TMP_Text dialogueDisplay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("PlayerBullet"))
        {
            DisplayRandomDialogue();
        }
    }

    void DisplayRandomDialogue()
    {
        chosenDialogue = dialogues[Random.Range(0, dialogues.Length)];

        dialogueDisplay.transform.parent.gameObject.SetActive(true);
        dialogueDisplay.text = chosenDialogue;
    }
}
