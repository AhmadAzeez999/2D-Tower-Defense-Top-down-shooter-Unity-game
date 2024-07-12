using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Chatter
{
    public string characterName;
    public string dialogue;
    public Sprite characterImage;
}

public class NPCBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField] float speed = 2f;
    [SerializeField] float range = 0.3f;
    [SerializeField] float maxDistance = 6f;
    [SerializeField] float waitTime = 5f;

    [SerializeField] bool patrols = false;
    [SerializeField] bool interactable = false;

    [SerializeField] GameObject keyDisplay;

    [SerializeField] Transform sprite;

    Vector2 wayPoint;

    const string IDLE = "Idle";
    const string MOVING = "Moving";
    const string ATTACKING = "Attacking";

    [SerializeField] Animator animator;
    string currentState;

    bool hasStopped;
    bool collidedWithWall = false;

    Rigidbody2D rb2d;

    [Header("NPC Dialogue System")]
    [SerializeField] GameObject levelConfirmUI;

    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameDisplay;
    public Image imageDisplay;
    public Chatter[] dialogueData;
    private int index;

    public GameObject continueButton;
    public float wordSpeed;
    public bool npcWantsToChat;
    public bool playerWantsToChat;

    public bool isQuestTarget = false;
    public bool isQuestGiver = false;

    [SerializeField] QuestHandler questHandler;
    [SerializeField] Transform nextQuestTarget;
    bool questCompleted = false;

    [SerializeField] string questText = "";

    [SerializeField] bool shopKeeper = false;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        if (patrols)
            SetNewDestination();

        if (interactable && npcWantsToChat)
            continueButton.GetComponent<Button>().onClick.AddListener(this.NextLine);
    }

    // Update is called once per frame
    void Update()
    {
        if (patrols)
        {
            if (!hasStopped)
                transform.position = Vector2.MoveTowards(transform.position, wayPoint, speed * Time.deltaTime);

            if ((Vector2.Distance(transform.position, wayPoint) <= range || collidedWithWall) && !hasStopped)
            {
                hasStopped = true;
                StartCoroutine(WaitThenSetNewDestination());
            }
        }

        if (interactable)
        {
            if (npcWantsToChat && playerWantsToChat)
            {
                if (dialoguePanel.activeInHierarchy)
                {
                    //Debug.Log("setactive");
                }
                else
                {
                    dialoguePanel.SetActive(true);
                    StartCoroutine(Typing());
                }

                if (dialogueText.text == dialogueData[index].dialogue)
                {
                    continueButton.SetActive(true);
                }
            }
        }

        if (isQuestTarget && playerWantsToChat && !questCompleted)
        {
            questHandler.QuestCompleted();
            questCompleted = true;


            if (isQuestGiver)
            {
                questHandler.SetQuestTarget(nextQuestTarget.position, questText);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && playerWantsToChat)
        {
            EndChat();
        }
    }

    public void ZeroText()
    {
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

    void SetNewDestination()
    {
        hasStopped = false;
        collidedWithWall = false;
        ChangeAnimationState(MOVING);
        wayPoint = new Vector2(Random.Range(-maxDistance, maxDistance), Random.Range(-maxDistance, maxDistance));

        Vector2 direction = wayPoint * speed * Time.deltaTime;

        if (direction.x >= 0.01f)
        {
            sprite.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (direction.x < 0.01f)
        {
            sprite.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    IEnumerator WaitThenSetNewDestination()
    {
        ChangeAnimationState(IDLE);
        rb2d.velocity = Vector2.zero;

        yield return new WaitForSeconds(waitTime);

        SetNewDestination();
    }

    void ChangeAnimationState(string newState)
    {
        // Stop the same animation from interrupting itself
        if (currentState == newState) return;

        // Play the animation
        animator.Play(newState);

        // Reassign the current state
        currentState = newState;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall") && !hasStopped)
        {
            if (collidedWithWall == false)
                StartCoroutine(justCollidedWithWall());
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            collidedWithWall = false;
        }
    }

    IEnumerator justCollidedWithWall()
    {
        yield return new WaitForSeconds(2f);
        collidedWithWall = true;
    }

    public void Interact()
    {
        if (!interactable)
            return;

        if (npcWantsToChat)
        {
            playerWantsToChat = true;
            ZeroText();
            GameManager.Instance.isChatting = true;
        }
        else
        {
            playerWantsToChat = true;
            if (isQuestTarget && !shopKeeper)
            {
                GameManager.Instance.isChatting = true;
            }

            levelConfirmUI.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void EndChat()
    {
        ZeroText();
        playerWantsToChat = false;
        GameManager.Instance.isChatting = false;
        questHandler.SetQuestTarget(nextQuestTarget.position, questText);
    }
}
