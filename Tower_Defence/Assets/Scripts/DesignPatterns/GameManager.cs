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

    [HideInInspector] public StateMachine _stateMachine;

    #region States
    [Header("GameStates")]
    [SerializeField] StateSO none;
    [SerializeField] public PauseSO pause;
    [SerializeField] PlayingSO playing;
    [SerializeField] BuildingSO building;

    [Header("Transitions")]
    [SerializeField] TransitionSO[] stateTransitions;
    [SerializeField] TransitionSO[] globalStateTransitions;
    #endregion

    #region GeneralProperties
    [Header("General Properties")]
    [Tooltip("The values will be modified when entering play state to the game values. The properties are just to debug/showcase")]
    [SerializeField] GameValues gameValues;
    /*[HideInInspector]*/ public int lifes = 5;
    /*[HideInInspector]*/ public int coins = 300;
    #endregion

    #region WaveProperties
    [Header("Wave Properties")]

    [HideInInspector]
    public int waveNumber = 0;

    [Range(10f, 100f)]
    public float waveDifficultyIncrement = 10f;

    #endregion

    #region Enemies
    [Header("Enemies")]

    public int initialNumberOfEnemiesToSpawn = 10;
    public List<enemyTypePercentage> enemyTypePercentages = new List<enemyTypePercentage>();

    [SerializeField] /*[HideInInspector]*/int remainingNumberOfEnemiesToSpawn;
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


    List<GameObject> enemyList = new List<GameObject>();
    #endregion
    
    [Range(0, 60)]
    public int buildingTime = 30;
    [HideInInspector] public int timeLeft;


    #region DebugValues
    [Header("Debug values")]
    [Tooltip("The amount of money to add by pressing < L > to the total of the coins")]
    [SerializeField] bool testSpawning;
    [SerializeField] int money = 10000;
    [SerializeField] int lifesToAdd = 100;
    [SerializeField] bool invincibleBase = false;
    #endregion
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) //Adds money
        {
            AddMoney(money);
        }
        if (Input.GetKeyDown(KeyCode.K)) //Adds lifes
        {
            lifes += lifesToAdd;
            Bus.Sync.Publish(this, new UpdateUI());
        }
        if (Input.GetKeyDown(KeyCode.N)) //Ends game
        {
            Bus.Sync.Publish(this.gameObject, new EndOfGame());
            _stateMachine.UpdateState(TransitionConditions.Con_Empty);
        }
        if (Input.GetKeyDown(KeyCode.I)) //Makes the base invincible
        {
            invincibleBase = !invincibleBase;
        }

        if (testSpawning && Input.GetKeyDown(KeyCode.L))
        {
            StartGame();
        }
    }
    
    public void AddMoney(int moneyToAdd)
    {
        coins += moneyToAdd;
        Bus.Sync.Publish(this.gameObject, new UpdateUI());
    }


    public void Reset()
    {
        coins = gameValues.startingCoins;
        lifes = gameValues.lifes;

        waveNumber = 0;

        remainingNumberOfEnemiesToSpawn = initialNumberOfEnemiesToSpawn;
        lastCalculatedNumberOfEnemiesToSpawn = initialNumberOfEnemiesToSpawn;

        playing.SetCoroutine(null);
    }
    public void StartGame()
    {
        Reset();
        _stateMachine.UpdateState(TransitionConditions.Con_Play);
        Bus.Sync.Publish(this, new UpdateUI());
    }

    public IEnumerator BuildingCountDown()
    {
        timeLeft = buildingTime;
        for (int i = buildingTime; i > 0; i--)
        {
            timeLeft--;
            Bus.Sync.Publish(this, new UpdateUI());
            yield return new WaitForSeconds(1f); // stops on pause state
        }
        _stateMachine.UpdateState(TransitionConditions.Con_Play);
    }

    public IEnumerator SpawnEnemies()
    {
        if (enemySpawnpoint == null)
        {
            enemySpawnpoint = GameObject.FindGameObjectWithTag(Tags.T_SpawnPortal).transform; 
        }

        Debug.Log("Started coroutine");
        float randomNumber;
        float totalValueOfPersantages = 0;

        for (int i = 0; i < enemyTypePercentages.Count; i++)
        {
            totalValueOfPersantages += enemyTypePercentages[i].percentageToSpawn;
        }

        while (remainingNumberOfEnemiesToSpawn > 0)
        {
            remainingNumberOfEnemiesToSpawn--;
            float persantageCounter = 0;

            randomNumber = UnityEngine.Random.Range(0f, 1f);

            for (int i = 0; i < enemyTypePercentages.Count; i++)
            {
                if (enemyTypePercentages[i].percentageToSpawn / totalValueOfPersantages + persantageCounter >= randomNumber)
                {

                    yield return new WaitUntil(() => _stateMachine.currentState == playing); //Waits until the game is unpaused if needed
                                                                                            //Still allows all the way to here for the game
                                                                                            //to decide on an enemy

                    //Spawns enemy
                    var enemy = Instantiate(enemyTypePercentages[i].enemyPrefab, enemySpawnpoint.transform.position, Quaternion.identity);
                    enemyList.Add(enemy);
                    break;
                }
                else
                {
                    persantageCounter += enemyTypePercentages[i].percentageToSpawn / totalValueOfPersantages;
                }
            }


            yield return new WaitForSeconds(gameValues.spawnDelay); //Uses scaled time so the coroutine will pause if the pause state enters
        }

        

        yield return new WaitUntil(() => enemyList.Count == 0); //Waits until all enemies are killed to enter building mode.

        waveNumber++;
        if (waveNumber > gameValues.maxWaveNumber)
        {
            Bus.Sync.Publish(this.gameObject, new EndOfGame());

            _stateMachine.UpdateState(TransitionConditions.Con_Empty); //Sets the game manager back to an idle state
            yield break;
        }

        //Increase difficulty of the wave
        IncreaseDifficultyOfTheWave();

        playing.SetCoroutine(null);
        _stateMachine.UpdateState(TransitionConditions.Con_Build);
        StartCoroutine(BuildingCountDown());

        /*        //Add a timer on GUI
                Debug.Log("Building time started");
                yield return new WaitForSeconds(buildingTime);
                Debug.Log("Building time ended");*/
    }

    void IncreaseDifficultyOfTheWave()
    {
        lastCalculatedNumberOfEnemiesToSpawn += (int)(waveDifficultyIncrement % 10);
        Debug.Log(lastCalculatedNumberOfEnemiesToSpawn);
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
            SetUpStates();
            Bus.Sync.Subscribe<EnemyDeathEvent>(RemoveEnemyFromList);
            Bus.Sync.Subscribe<OnEnemyReachedBase>(EnemyReachedBase);
            Bus.Sync.Subscribe<OnEnemyReachedBase>(RemoveEnemyFromList);
            Reset();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        

        //Bus.Sync.Subscribe<OnEnemyReachedBase>(EnemyReachedBase);
    }

    private void RemoveEnemyFromList(OnEnemyReachedBase @event)
    {
        if (enemyList.Contains(@event.target))
        {
            enemyList.Remove(@event.target);
        }
    }

    private void RemoveEnemyFromList(EnemyDeathEvent @event)
    {
        if(enemyList.Contains(@event.target))
        {
            enemyList.Remove(@event.target);
        }
    }

    void SetUpStates()
    {
        if(_stateMachine == null)
        {
            _stateMachine = this.AddComponent<StateMachine>();
        }

        _stateMachine.SetState(none);
        _stateMachine.AddTransitions(stateTransitions);
        _stateMachine.AddGlobalTransitons(globalStateTransitions);
    }
    void EnemyReachedBase(OnEnemyReachedBase @event)
    {
        if (!invincibleBase)
        {
            lifes--;
            if (lifes == 0)
            {
                Bus.Sync.Publish(this.gameObject, new EndOfGame());
                _stateMachine.UpdateState(TransitionConditions.Con_Empty);
                StopAllCoroutines();
            }
        }
    }
}
