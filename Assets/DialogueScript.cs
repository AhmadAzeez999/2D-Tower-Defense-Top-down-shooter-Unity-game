using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueScript : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameDisplay;
    public Image imageDisplay;
    public Chatter[] dialogueData;
    private int index;

    public GameObject continueButton;

    public float wordSpeed;

    bool dialogueDone = false;

    [SerializeField] bool activatesObject = false;
    [SerializeField] ObjectEnabler objectEnabler;

    [SerializeField] bool movesToNextScene = false;
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] int sceneIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        ZeroText();
        continueButton.GetComponent<Button>().onClick.AddListener(this.NextLine);
    }

    // Update is called once per frame
    void Update()
    {
        if (!dialoguePanel.activeInHierarchy && !dialogueDone)
        {
            dialoguePanel.SetActive(true);
            StartCoroutine(Typing());
        }

        if (dialogueText.text == dialogueData[index].dialogue)
        {
            continueButton.SetActive(true);
        }
    }

    public void ZeroText()
    {
        if (GameManager.Instance)
            GameManager.Instance.isChatting = true;
        dialogueDone = false;
        if (dialogueData[index] != null)
        {
            imageDisplay.sprite = dialogueData[index].characterImage;
            nameDisplay.text = dialogueData[index].characterName;
        }
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
    }

    IEnumerator Typing()
    {
        foreach (char letter in dialogueData[index].dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    public void NextLine()
    {
        continueButton.SetActive(false);

        Debug.Log(dialogueData[index].dialogue);

        if (index < dialogueData.Length - 1)
        {
            index++;
            imageDisplay.sprite = dialogueData[index].characterImage;
            nameDisplay.text = dialogueData[index].characterName;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            EndChat();
        }
    }

    public void EndChat()
    {
        ZeroText();
        if (GameManager.Instance)
            GameManager.Instance.isChatting = false;
        dialoguePanel.SetActive(false);
        dialogueDone = true;

        if (activatesObject)
        {
            objectEnabler.EnableObject();
        }

        if (movesToNextScene)
        {
            levelLoader.LoadThisScene(sceneIndex);
        }

        gameObject.SetActive(false);
    }
}
