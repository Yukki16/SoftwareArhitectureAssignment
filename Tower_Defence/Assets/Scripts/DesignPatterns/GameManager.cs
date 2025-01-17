using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// GameManager is the Singleton design pattern.
/// Manages the enemies spawning and coins/lifes of the player + building time in which the plyaer is allowed to build towers.
/// 
/// Debug: "L" => adds coins;
/// "K" => adds lifes;
/// "N" => ends game;
/// "I" => makes the base take no damage;
/// </summary>
[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("General Properties")]

    public int lifes = 5;
    public int coins = 300;

    [Header("Wave Properties")]

    [HideInInspector]
    public int waveNumber = 0;

    [Range(5, 10)]
    public int maxWaveNumber = 5;

    [Range(10f, 100f)]
    public float waveDifficultyIncrement;

    [Header("Delays")]

    [Min(0.1f)]
    public float spawnDelay;

    [Range(0, 60)]
    public int buildingTime = 30;

    [Header("Enemies")]

    public int initialNumberOfEnemiesToSpawn = 10;
    public List<enemyTypePercentage> enemyTypePercentages = new List<enemyTypePercentage>();

    int remainingNumberOfEnemiesToSpawn;
    int lastCalculatedNumberOfEnemiesToSpawn;

    //[HideInInspector]
    public Transform enemySpawnpoint;
    //[HideInInspector]
    public Transform enemyPOI;


    [Serializable]
    public struct enemyTypePercentage
    {
        public string enemyTypeName;
        public GameObject enemyPrefab;

        [Range(0, 100)] public float percentageToSpawn;
    }

    [Header("Debug values")]
    [Tooltip("The amount of money to add by pressing < L > to the total of the coins")]
    [SerializeField] int money = 10000;
    [SerializeField] int lifesToAdd = 100;
    [SerializeField] bool invincibleBase = false;
    public enum stateOfGame
    {
        None,
        BuildingTime,
        Wave,
        Paused,
        EndOfGame
    }

    public stateOfGame state = stateOfGame.None;
    public void GetEnemySpawnLocation(Transform spawnPoint)
    {
        enemySpawnpoint = spawnPoint;
    }

    public void GetEnemyPOI(Transform enemyPOI)
    {
        this.enemyPOI = enemyPOI;
    }

    public void AddMoney(int moneyToAdd)
    {
        coins += moneyToAdd;
    }
    /**
    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }
    //**/

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) //Adds money
        {
            AddMoney(money);
        }
        if (Input.GetKeyDown(KeyCode.K)) //Adds lifes
        {
            lifes += lifesToAdd;
        }
        if (Input.GetKeyDown(KeyCode.N)) //Ends game
        {
            Bus.Sync.Publish(this.gameObject, new EndOfGame());
        }
        if (Input.GetKeyDown(KeyCode.I)) //Makes the base invincible
        {
            invincibleBase = true;
        }
    }
    public IEnumerator SpawnEnemies()
    {
        float randomNumber;
        float totalValueOfPersantages = 0;

        for (int i = 0; i < enemyTypePercentages.Count; i++)
        {
            totalValueOfPersantages += enemyTypePercentages[i].percentageToSpawn;
        }

        //Add a timer on GUI
        Debug.Log("Building time started");
        state = stateOfGame.BuildingTime;
        Bus.Sync.Publish(this.gameObject, new UpdateUI());
        yield return new WaitForSeconds(buildingTime);
        state = stateOfGame.Wave;
        Bus.Sync.Publish(this.gameObject, new UpdateUI());
        Debug.Log("Building time ended");

        while (remainingNumberOfEnemiesToSpawn > 0)
        {
            remainingNumberOfEnemiesToSpawn--;
            float persantageCounter = 0;

            randomNumber = UnityEngine.Random.Range(0f, 1f);

            for (int i = 0; i < enemyTypePercentages.Count; i++)
            {
                if (enemyTypePercentages[i].percentageToSpawn / totalValueOfPersantages + persantageCounter >= randomNumber)
                {
                    //Spawns enemy
                    var enemy = Instantiate(enemyTypePercentages[i].enemyPrefab, enemySpawnpoint.transform.position, Quaternion.identity);
                    enemy.GetComponent<Enemy>().SetDestination(enemyPOI.position);
                    break;
                }
                else
                {
                    persantageCounter += enemyTypePercentages[i].percentageToSpawn / totalValueOfPersantages;
                }
            }


            yield return new WaitForSeconds(spawnDelay);
        }
        waveNumber++;
        if (waveNumber >= maxWaveNumber)
        {
            Bus.Sync.Publish(this.gameObject, new EndOfGame());
            yield break;
        }

        //Increase difficulty of the wave
        IncreaseDifficultyOfTheWave();

        StartCoroutine(SpawnEnemies());

        /*        //Add a timer on GUI
                Debug.Log("Building time started");
                yield return new WaitForSeconds(buildingTime);
                Debug.Log("Building time ended");*/
    }

    void IncreaseDifficultyOfTheWave()
    {
        lastCalculatedNumberOfEnemiesToSpawn += (int)((lastCalculatedNumberOfEnemiesToSpawn / 100) * waveDifficultyIncrement);
        remainingNumberOfEnemiesToSpawn = lastCalculatedNumberOfEnemiesToSpawn;
    }
    public void UsedCoins(int howManyCoins)
    {
        coins -= howManyCoins;
        Bus.Sync.Publish(this.gameObject, new UpdateUI());
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        remainingNumberOfEnemiesToSpawn = initialNumberOfEnemiesToSpawn;
        lastCalculatedNumberOfEnemiesToSpawn = initialNumberOfEnemiesToSpawn;

        Bus.Sync.Subscribe<OnEnemyReachedBase>(EnemyReachedBase);
    }

    void EnemyReachedBase(object sender, EventArgs args)
    {
        if (!invincibleBase)
        {
            lifes--;
            if (lifes == 0)
            {
                Bus.Sync.Publish(this.gameObject, new EndOfGame());
                state = stateOfGame.EndOfGame;
                StopAllCoroutines();
            }
        }
    }
}
