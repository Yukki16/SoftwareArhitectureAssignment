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
public class Enemy : MonoBehaviour
{

    [SerializeField] EnemySO properties;

    string enemyName;
    float health;
    float speed;
    float goldValue;

    [SerializeField] NavMeshAgent agent;

    [SerializeField] List<Transform> pathTargets = new List<Transform>();

    [Header("Debuffs")]
    public bool isSlowed;
    public float slowedPower = 0;

    public Slider healthBar;

    public GameObject moneyAfterDeathDisplayCanvasPrefab;

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

    //Subscribes to it's own death so the bus has time to update before destroying itself.
    private void OnDeath(EnemyDeathEvent @event)
    {
        if(@event.target.Equals(gameObject)) 
        {
            GameManager.Instance.AddMoney(properties.killingValue);

            Destroy(gameObject);
        }
    }

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


    /*public void Damage(float damage)
    {
        health -= damage;

        healthBar.value = health / properties.health;
        //Debug.Log("Got shot for" + damage);
        if (health <= 0)
        {
            Bus.Sync.Publish(this.gameObject, new EnemyDeathEvent());
            GameManager.Instance.AddMoney(properties.killingValue);
            Instantiate(moneyAfterDeathDisplayCanvasPrefab, this.transform.position, Quaternion.Euler(90, 0, 0)).GetComponentInChildren<TMP_Text>().text = "+" + properties.killingValue.ToString();
            //Debug.Log("About to destroy myself");
            Destroy(this.gameObject);
        }
    }*/

    public void SetDestination(Vector3 destinationOfTheEnemy)
    {
        agent.SetDestination(destinationOfTheEnemy);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Tags.T_PathEnd)) //Tags.T_PathEnd is the Tags to enum asset from the Unity Asset store that I use so I don't 
                                              //misspel the tags
                                              //https://assetstore.unity.com/packages/tools/utilities/enum-tags-autocomplete-tags-247275
        {
            Bus.Sync.Publish(this.gameObject, new OnEnemyReachedBase());
            StartCoroutine(WaitForBusAndSelfDestroy());
        }
    }

    IEnumerator WaitForBusAndSelfDestroy()
    {
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }*/

    IEnumerator Move()
    {
        for (int i = 0; i < pathTargets.Count; i++)
        {
            agent.SetDestination(pathTargets[i].position);
            //Debug.Log(agent.remainingDistance);
            yield return new WaitUntil(() => !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance);
        }
    }
}
