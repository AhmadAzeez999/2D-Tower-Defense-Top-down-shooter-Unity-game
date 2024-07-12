using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Wave
{
    public string waveName;
    public int numOfEnemies;
    public GameObject[] typeOfEnemies;
    public float spawnInterval;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Wave[] waves;

    private Wave currentWave;
    [SerializeField] private int currentWaveNumber;
    private float nextSpawnTime;

    public Animator animator;
    public TMP_Text waveName;

    public TMP_Text permaWaveName;

    private bool canSpawn = true;
    private bool canAnimate = false;

    public Transform[] spawnPoints;

    GameObject[] totalEnemies;

    public bool gameStarted = false;
    public bool playerIsReady = false;
    //private bool bossSpawned = false;

    AudioManager audioManager;
    //[SerializeField] private AudioClip bossMusic;
    //[SerializeField] private AudioClip bossMusicRadioVersion;
    bool musicStarted = false;

    private bool gameCompleted = false;

    //[SerializeField] private GameObject gameOverUI;
    //[SerializeField] private GameObject gameCompletedUI;
    //[SerializeField] private TMP_Text waveSurvivedText;

    [SerializeField] GameObject tutorialUI;

    private void Start()
    {
        currentWave = waves[currentWaveNumber];
        permaWaveName.text = currentWave.waveName + " / " + waves.Length;

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        if (playerIsReady && !gameStarted)
        {
            animator.SetTrigger("LevelStart");
            playerIsReady = false;
        }

        if (!gameStarted)
            return;

        if (tutorialUI.activeSelf)
        {
            tutorialUI.SetActive(false);
        }

        currentWave = waves[currentWaveNumber];
        SpawnWave();
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (totalEnemies.Length == 0)
        {
            if (currentWaveNumber + 1 != waves.Length)
            {
                if (canAnimate)
                {
                    waveName.text = waves[currentWaveNumber + 1].waveName;
                    permaWaveName.text = waves[currentWaveNumber + 1].waveName + " / " + waves.Length;
                    animator.SetTrigger("WaveComplete");
                    canAnimate = false;
                }
            }
            else
            {
                if (!gameCompleted)
                {
                    GameManager.instance.LevelComplete();
                    Debug.Log("Level complete");
                    gameCompleted = true;
                }
            }
        }
    }

    private void SpawnNextWave()
    {
        if (!gameStarted)
            return;

        currentWaveNumber++;
        canSpawn = true;

        currentWave = waves[currentWaveNumber];

        if (currentWaveNumber + 1 == waves.Length)
        {
            //audioManager.ChangeMusic(bossMusicRadioVersion, bossMusic);
        }
    }

    private void SpawnWave()
    {
        if (canSpawn && nextSpawnTime < Time.time)
        {
            GameObject randomEnemy = currentWave.typeOfEnemies[Random.Range(0, currentWave.typeOfEnemies.Length)];
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(randomEnemy, randomPoint.position, Quaternion.identity);
            currentWave.numOfEnemies--;
            nextSpawnTime = Time.time + currentWave.spawnInterval;
            if (currentWave.numOfEnemies == 0)
            {
                canSpawn = false;
                canAnimate = true;
            }
        }
    }

    private void StartSpawner()
    {
        gameStarted = true;
    }

    public void PlatCountdownSFX()
    {
        audioManager.PlaySFX(audioManager.impactSound);
    }

    public void PlayLastCountdownSFX()
    {
        audioManager.PlaySFX(audioManager.impactSound2);
        if (!musicStarted)
        {
            audioManager.PlayMusic();
            musicStarted = true;
        }
    }
}
