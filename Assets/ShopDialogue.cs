using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopDialogue : MonoBehaviour
{
    [SerializeField] float dialogueInterval = 1f;
    float dialogueTimer = 0f;

    string[] dialogues = {"GIVE ME BONES!!!", "I NEED SOME FRESH- I mean... UNDEAD BONES!!!", 
                        "Did you know the adult human body has 206 bones?!?!", "They call you the 'hero from the future' and yet I don't see you giving me any bones",
                        "Wanna know what I'm using the bones for? Sure! it's for-", "SCRAPS FOR BONES! SCRAPS FOR BONES!", "GIVE ME BONES! YOU NUMBSKULL! See what I did there? NumbSKULL?",
                        "Derik the Boneman?! Who's the idiot that gave me such a name?!", "I like bones", "What's with the look? Can a man not have a bone collection?", 
                        "I wIlL tAkE yOuR bOnEs, haha just kidding...unless", "Why are the undead attacking us? Cause they have a bone to pick. Ha? Ha?", 
                        "I'm sure the undead feels lonely at times, cause they have no body, get it? get it?", "I NEED THE BONES MAN!!!", "..."};

    string[] buyingDialogues = { "Ah, a fine addition to my bone collection", "mmmm, bones", "Bone-appétit", "YES! BONES!!!", "Yes! Give me all the bones",
                                "Oh! This is type of bone is rear!", "More! MORE!!!"};

    string chosenDialogue;

    [SerializeField] TMP_Text dialogueDisplay;

    private void Start()
    {
        dialogueDisplay.transform.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (dialogueTimer > 0)
        {
            dialogueTimer -= Time.unscaledDeltaTime;
        }

        if (dialogueTimer <= 0)
        {
            GenerateRandomIntervalNumber();
            dialogueTimer = dialogueInterval;

            DisplayRandomDialogue();
        }
    }

    void GenerateRandomIntervalNumber()
    {
        dialogueInterval = Random.Range(8f, 20f);
    }

    public void DisplayRandomDialogue(bool justBought = false)
    {
        if (!justBought)
        {
            chosenDialogue = dialogues[Random.Range(0, dialogues.Length)];
        }
        else
        {
            chosenDialogue = buyingDialogues[Random.Range(0, buyingDialogues.Length)];
            dialogueDisplay.transform.parent.gameObject.SetActive(false);
        }

        dialogueDisplay.transform.parent.gameObject.SetActive(true);
        dialogueDisplay.text = chosenDialogue;
    }
}
