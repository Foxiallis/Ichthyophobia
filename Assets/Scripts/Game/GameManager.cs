using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Sardine, MagicSardine, SpiderSardine, FishJenga }
public class GameManager : MonoBehaviour
{
    public static PlayerController player;
    public static int waveAmount = 5;
    public int currentWave;
    public int maxEnemies = 10;
    public GameObject sardinePrefab;
    public GameObject magicSardinePrefab;
    public GameObject spiderSardinePrefab;
    public GameObject fishJengaPrefab;
    public List<SpawnPoint> spawnLocations;
    public List<GameObject> spawnedEnemies;
    public Dictionary<EnemyType, int> enemyAmounts; //stores how many enemies of the current type are now spawned

    private bool finishedSpawning;
    private bool paused;
    private bool hasWon;
    private UIManager uiManager;
    private Tutorial tutorial;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        uiManager = GetComponent<UIManager>();
        tutorial = GetComponent<Tutorial>();
        enemyAmounts = new Dictionary<EnemyType, int>();
        spawnedEnemies = new List<GameObject>();
        enemyAmounts.Add(EnemyType.Sardine, 0);
        enemyAmounts.Add(EnemyType.MagicSardine, 0);
        enemyAmounts.Add(EnemyType.SpiderSardine, 0);
        enemyAmounts.Add(EnemyType.FishJenga, 0);
    }

    private void Update()
    {
        if (finishedSpawning && spawnedEnemies.Count == 0)
        {
            if (currentWave < waveAmount || waveAmount == 0)
            {
                StartWave();
            }
            else if (!paused && !hasWon)
            {
                hasWon = true;
                Pause();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !player.respawning)
        {
            if (!paused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    private void Pause()
    {
        paused = true;
        player.Pause();
        foreach (GameObject enemy in spawnedEnemies)
        {
            enemy.GetComponent<EnemyBehaviour>().Pause();
        }
        if (hasWon)
        {
            uiManager.ActivatePauseMenu(true, "Victory!");
        }
        else
        {
            uiManager.ActivatePauseMenu(true, "Paused");
        }
    }

    private void Resume()
    {
        paused = false;
        player.Resume();
        foreach (GameObject enemy in spawnedEnemies)
        {
            enemy.GetComponent<EnemyBehaviour>().Resume();
        }
        uiManager.ActivatePauseMenu(false);
    }

    public void StartWave()
    {
        currentWave++;
        if (currentWave == 1)
        {
            player.PlayMusic();
        }
        uiManager.Alert("Wave " + currentWave, 3);
        enemyAmounts[EnemyType.Sardine] = 0;
        enemyAmounts[EnemyType.MagicSardine] = 0;
        enemyAmounts[EnemyType.SpiderSardine] = 0;
        enemyAmounts[EnemyType.FishJenga] = 0;
        StartCoroutine(SpawnEnemies());
    }

    public void HandleDead(GameObject dead)
    {
        spawnedEnemies.Remove(dead);
        foreach(SpawnPoint spawn in spawnLocations) //so that when enemies die in the spawn they won't lock it
        {
            spawn.entitiesIn.Remove(dead);
        }
    }

    public void Reset()
    {
        StopAllCoroutines();
        uiManager.ActivatePauseMenu(false);
        enemyAmounts[EnemyType.Sardine] = 0;
        enemyAmounts[EnemyType.MagicSardine] = 0;
        enemyAmounts[EnemyType.SpiderSardine] = 0;
        enemyAmounts[EnemyType.FishJenga] = 0;
        currentWave = 0;
        finishedSpawning = false;
        hasWon = false;
        foreach (GameObject enemy in spawnedEnemies)
        {
            Destroy(enemy);
        }
        spawnedEnemies = new List<GameObject>();
        tutorial.gameStarted = false;
        tutorial.readyDone = false;
        tutorial.UpdateText();
    }

    IEnumerator SpawnEnemies()
    {
        finishedSpawning = false;
        int sardineAmount = 4 + currentWave;
        int magicSardineAmount = Mathf.FloorToInt(currentWave / 2f) * 2;
        int spiderSardineAmount = currentWave >= 3 ? Mathf.RoundToInt(currentWave / 3f) : 0;
        int fishJengaAmount = currentWave % 5 == 0 ? Mathf.FloorToInt(currentWave / 5f) : 0;

        yield return new WaitForSecondsRealtime(2); //wait a bit before spawning to give player rest

        while (enemyAmounts[EnemyType.Sardine] < sardineAmount)
        {
            if (spawnedEnemies.Count < maxEnemies && !paused) //check if there is space in the arena
            {
                SpawnPoint randomPoint = spawnLocations[Random.Range(0, spawnLocations.Count)];
                if (randomPoint.canSpawn)
                {
                    GameObject enemy = Instantiate(sardinePrefab, randomPoint.transform);
                    enemy.transform.localPosition = new Vector3(0, 0.2f, 0);
                    enemy.transform.SetParent(transform);
                    spawnedEnemies.Add(enemy);
                    enemyAmounts[EnemyType.Sardine]++;
                    randomPoint.canSpawn = false;
                }
                //if the spawnpoint is taken just randomize another one
            }
            yield return null;
        }

        while (enemyAmounts[EnemyType.MagicSardine] < magicSardineAmount)
        {
            if (spawnedEnemies.Count < maxEnemies && !paused) //check if there is space in the arena
            {
                SpawnPoint randomPoint = spawnLocations[Random.Range(0, spawnLocations.Count)];
                if (randomPoint.canSpawn)
                {
                    GameObject enemy = Instantiate(magicSardinePrefab, randomPoint.transform);
                    enemy.transform.localPosition = new Vector3(0, 0.2f, 0);
                    enemy.transform.SetParent(transform);
                    spawnedEnemies.Add(enemy);
                    enemyAmounts[EnemyType.MagicSardine]++;
                    randomPoint.canSpawn = false;
                }
                //if the spawnpoint is taken just randomize another one
            }
            yield return null;
        }

        while (enemyAmounts[EnemyType.SpiderSardine] < spiderSardineAmount)
        {
            if (spawnedEnemies.Count < maxEnemies && !paused) //check if there is space in the arena and pause isn't on
            {
                SpawnPoint randomPoint = spawnLocations[Random.Range(0, spawnLocations.Count)];
                if (randomPoint.canSpawn)
                {
                    GameObject enemy = Instantiate(spiderSardinePrefab, randomPoint.transform);
                    enemy.transform.localPosition = new Vector3(0, 0.2f, 0);
                    enemy.transform.SetParent(transform);
                    spawnedEnemies.Add(enemy);
                    enemyAmounts[EnemyType.SpiderSardine]++;
                    randomPoint.canSpawn = false;
                }
                //if the spawnpoint is taken just randomize another one
            }
            yield return null;
        }

        while (enemyAmounts[EnemyType.FishJenga] < fishJengaAmount)
        {
            if (spawnedEnemies.Count < maxEnemies && !paused) //check if there is space in the arena
            {
                SpawnPoint randomPoint = spawnLocations[Random.Range(0, spawnLocations.Count)];
                if (randomPoint.canSpawn)
                {
                    GameObject enemy = Instantiate(fishJengaPrefab, randomPoint.transform);
                    enemy.transform.localPosition = new Vector3(0, 0.2f, 0);
                    enemy.transform.SetParent(transform);
                    spawnedEnemies.Add(enemy);
                    if (waveAmount == 5)
                    {
                        uiManager.ShowBoss(enemy.GetComponent<EnemyBehaviour>());
                    }
                    enemyAmounts[EnemyType.FishJenga]++;
                    randomPoint.canSpawn = false;
                }
                //if the spawnpoint is taken just randomize another one
            }
            yield return null;
        }
        finishedSpawning = true;
    }
}
