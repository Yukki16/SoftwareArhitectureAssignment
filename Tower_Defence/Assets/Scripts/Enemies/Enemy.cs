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

    [SerializeField] NavMeshAgent agent;

    [SerializeField] PathSO path;

    [Header("Debuffs")]
    public bool isSlowed;
    public float slowedPower = 0;

    public Slider healthBar;

    public GameObject moneyAfterDeathDisplayCanvasPrefab;


    /// <summary>
    /// DON'T FORGET TO SUBSCRIBE
    /// </summary>
    
    private void OnEnable()
    {
        //////////////////////////////////////////////////////// Subscribers
        Bus.Sync.Subscribe<EnemyTakesDamageEvent>(OnTakeDamage);
        Bus.Sync.Subscribe<EnemyDeathEvent>(OnDeath);
        ////////////////////////////////////////////////////////
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        enemyName = properties.name;
        health = properties.health; 
        speed = properties.speed;
        goldValue = properties.killingValue;

        agent.name = enemyName;
        agent.speed = speed;

        agent.stoppingDistance = 0.1f;

        StartCoroutine(Move());
    }

    /// <summary>
    /// Subscribes to it's own death so the bus has time to update before destroying itself.
    /// </summary>
    /// <param name="event"></param>
    private void OnDeath(EnemyDeathEvent @event)
    {
        if(@event.target.Equals(gameObject)) 
        {
            GameManager.Instance.AddMoney(goldValue);

            Destroy(gameObject);
        }
    }
    /// <summary>
    /// DON'T FORGET TO UNSUBSCRIBE
    /// </summary>
    private void OnDisable() 
    {
        Bus.Sync.Unsubscribe<EnemyTakesDamageEvent>(OnTakeDamage);
        Bus.Sync.Unsubscribe<EnemyDeathEvent>(OnDeath);
    }

    private void OnTakeDamage(EnemyTakesDamageEvent @event)
    {
        if(@event.target.Equals(gameObject))
        {
            health -= @event.damageAmount;

            if(health <= 0f)
            {
                Bus.Sync.Publish(this, new EnemyDeathEvent(gameObject));
            }
        }
    }

    IEnumerator Move()
    {
        for (int i = 0; i < path.pathPoints.Count; i++)
        {
            agent.SetDestination(path.pathPoints[i]);
            //Debug.Log(agent.remainingDistance);
            yield return new WaitUntil(() => !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance);
        }
    }
}
