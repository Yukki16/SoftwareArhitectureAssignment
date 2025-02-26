using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
/// <summary>
/// Enemy class has all the needs for a basic enemy. Enemies differ by visuals and properties such as speed and health so there is no need for 
/// inheritance for each type of enemies.
/// </summary>
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{

    [SerializeField] EnemySO properties;

    string enemyName;
    float health;
    float speed;
    int goldValue;

    [HideInInspector] public float distanceTraveled;

    [SerializeField] NavMeshAgent agent;

    [SerializeField] PathSO path;

    [Header("Debuffs")]
    public bool isSlowed;
    public float slowedPower = 0;
    [HideInInspector] public Coroutine slowCoroutine;
    [SerializeField] GameObject slowedText;
    [SerializeField] GameObject moneyAfterDeathDisplayCanvasPrefab;

    [Header("Debug")]
    [SerializeField] bool stopMovement;


    /// <summary>
    /// DON'T FORGET TO SUBSCRIBE
    /// </summary>

    private void OnEnable()
    {
        //////////////////////////////////////////////////////// Subscribers
        Bus.Sync.Subscribe<EnemyTakesDamageEvent>(OnTakeDamage);
        Bus.Sync.Subscribe<EnemyDeathEvent>(OnDeath);
        Bus.Sync.Subscribe<TimeChanged>(CalculateSpeed);
        Bus.Sync.Subscribe<OnEnemyReachedBase>(ReachedBase);
        ////////////////////////////////////////////////////////
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        enemyName = properties.name;
        health = properties.health;
        speed = properties.speed;
        goldValue = properties.killingValue;

        agent.name = enemyName;
        CalculateSpeed(null);

        agent.stoppingDistance = 0.1f;

        if(!stopMovement)
        {
            StartCoroutine(Move());
        }
        
    }

    private void ReachedBase(OnEnemyReachedBase @event)
    {
        if(@event.target.Equals(gameObject))
        {
            StopAllCoroutines();
            StartCoroutine(DestroyNextFrame());
        }
    }

    //Recalculates the speed when the time scale changes;
    private void CalculateSpeed(TimeChanged changed)
    {
        speed = properties.speed * Time.timeScale;
        agent.speed = speed - (speed * slowedPower) / 100;
    }

    /// <summary>
    /// Subscribes to it's own death so the bus has time to update before destroying itself.
    /// </summary>
    /// <param name="event"></param>
    private void OnDeath(EnemyDeathEvent @event)
    {
        if (@event.target.Equals(gameObject))
        {
            GameManager.Instance.AddMoney(goldValue);
            Instantiate(moneyAfterDeathDisplayCanvasPrefab, this.transform.position, Quaternion.identity).GetComponentInChildren<TMP_Text>().text = "+" + goldValue.ToString();
            StopAllCoroutines();
            StartCoroutine(DestroyNextFrame());
        }
    }

    //Needed for the bus to update all the list the enemy is in.
    private IEnumerator DestroyNextFrame()
    {
        yield return new WaitForNextFrameUnit();
        Destroy(gameObject);
    }

    /// <summary>
    /// DON'T FORGET TO UNSUBSCRIBE
    /// </summary>
    private void OnDisable()
    {
        Bus.Sync.Unsubscribe<EnemyTakesDamageEvent>(OnTakeDamage);
        Bus.Sync.Unsubscribe<EnemyDeathEvent>(OnDeath);
        Bus.Sync.Unsubscribe<TimeChanged>(CalculateSpeed);
        Bus.Sync.Unsubscribe<OnEnemyReachedBase>(ReachedBase);
    }

    public void StopSlowCoroutine()
    {
        agent.speed = speed;
        if(slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            slowedText.SetActive(false);
        }
    }
    public IEnumerator Slow()
    {
        agent.speed = speed - (speed * slowedPower) / 100;
        slowedText.SetActive(true);
        yield return new WaitForSeconds(1f);

        slowedText.SetActive(false);
        slowedPower = 0;
        agent.speed = speed;
        isSlowed = false;
    }

    private void OnTakeDamage(EnemyTakesDamageEvent @event)
    {
        if (@event.target.Equals(gameObject))
        {
            health -= @event.damageAmount;

            if (health <= 0f)
            {
                Bus.Sync.Publish(this, new EnemyDeathEvent(gameObject));
            }
        }
    }

    private void Update()
    {
        if(stopMovement)
        {
            return;
        }
        if (!agent.isStopped)
        {
            distanceTraveled += agent.speed * Time.deltaTime;
        }
    }

    IEnumerator Move()
    {
        for (int i = 0; i < path.pathPoints.Count; i++)
        {
            agent.SetDestination(path.pathPoints[i]);
            //Debug.Log(agent.remainingDistance);
            if (agent != null)
            {
                yield return new WaitUntil(() => !agent.pathPending &&
                agent.remainingDistance <= agent.stoppingDistance);
            }
        }
    }

    public float ReturnPercentHP()
    {
        return health / properties.health;
    }
}
