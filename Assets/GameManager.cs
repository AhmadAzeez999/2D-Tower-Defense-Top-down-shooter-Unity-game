using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance is null)
                Debug.LogError("Game Manager is NULL");

            return instance;
        }
    }
    public enum GameState
    {
        Gameplay,
        Paused,
        LevelComplete,
        GameOver
    };

    public GameState currentState;

    public GameState previousState;

    public static int coins;
    public int Coins
    {
        get { return coins; }
        set { coins = value; }
    }

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject gameOverScreen;
    public GameObject levelCompleteScreen;

    void Awake()
    {
        instance = this;
    }

    [SerializeField] TMP_Text moneyDisplay;
    [SerializeField] TMP_Text boneDisplay;

    public bool isGameOver = false;

    public static int bones = 20;
    public int Bones
    {
        get { return bones; }
        set { bones = value; }
    }

    // For combo
    private int comboMultiplier;
    private float comboCountDown;
    private float comboTime = 5;

    bool comboStarted;

    [SerializeField] GameObject comboDisplayUI;
    [SerializeField] TMP_Text comboCountDisplayUI;
    [SerializeField] Slider comboCountDownSlider;

    float elapsedTime;
    [SerializeField] float expectedTimeToFinish = 60;
    bool timeChallengeCompleted = false;
    bool gateChallengesChecked = false;

    [SerializeField] GameData gameData;

    [SerializeField] bool isLevel = true;

    [SerializeField] GateHealth gate;

    public bool isChatting = false;

    [SerializeField] QuestHandler questHandler;
    [SerializeField] Transform targetTransform;
    [SerializeField] string questTest;

    // Start is called before the first frame update
    void Start()
    {
        LoadData();

        boneDisplay.text = Bones.ToString();

        if (isLevel)
        {
            comboCountDown = comboTime;
            comboCountDownSlider.maxValue = comboTime;
        }
        else
        {
            questHandler.SetQuestTarget(targetTransform.position, questTest);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameData)
        {
            elapsedTime += Time.deltaTime;
        }

        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                break;
            case GameState.Paused:
                CheckForPauseAndResume();
                break;
            case GameState.LevelComplete:
                if (elapsedTime < expectedTimeToFinish && !timeChallengeCompleted)
                {
                    timeChallengeCompleted = true;
                    gameData.AddBoneEarned();
                }

                if (!gateChallengesChecked)
                {
                    gateChallengesChecked = true;
                    gate.CheckObjectives();
                    StartCoroutine(PauseGameAfterSeconds());
                }

                Bones += gameData.GetBonesEarned();

                levelCompleteScreen.SetActive(true);

                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    gameOverScreen.SetActive(true);
                }

                break;
            default:
                Debug.Log("Error");
                break;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerPrefs.DeleteAll();
        }

        if (comboStarted)
        {
            comboCountDown -= Time.deltaTime;

            if (comboMultiplier >= 3)
            {
                comboDisplayUI.SetActive(true);
                comboCountDownSlider.value = comboCountDown;
                comboCountDisplayUI.text = "COMBO x" + comboMultiplier;
            }

            if (comboCountDown < 0)
            {
                comboDisplayUI.GetComponent<ComboUIScript>().ComboTimeOut(10 * comboMultiplier);
                ResetCombo();
            }
        }
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        moneyDisplay.text = "$" + Coins;
    }

    public void RemoveCoins(int amount)
    {
        Coins -= amount;
        moneyDisplay.text = "$" + Coins;
    }

    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseAndResume();
        }
    }

    public void PauseAndResume()
    {
        if (currentState == GameState.Paused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f; // Stop the game
            pauseScreen.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f; // Resume the game
            pauseScreen.SetActive(false);
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    public void LevelComplete()
    {
        ChangeState(GameState.LevelComplete);
    }

    public void ExitGamePause()
    {
        Time.timeScale = 1;
    }

    public void AddBones(int amount)
    {
        Bones += amount;

        Debug.Log(amount + " bones added");
        boneDisplay.text = Bones.ToString();
    }

    public void RemoveBones(int amount)
    {
        Bones -= amount;

        Debug.Log(amount + " bones removed");
        boneDisplay.text = Bones.ToString();
    }

    public void IncreaseCombo()
    {
        comboMultiplier++;
        comboStarted = true;
        comboCountDown = comboTime;
    }

    public void ResetCombo()
    {
        comboMultiplier = 0;
        comboStarted = false;
        comboCountDown = comboTime;
    }

    IEnumerator PauseGameAfterSeconds()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
    }

    public void LoadData()
    {
        //Bones = PlayerPrefs.GetInt("Bones");
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("Bones", Bones);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
