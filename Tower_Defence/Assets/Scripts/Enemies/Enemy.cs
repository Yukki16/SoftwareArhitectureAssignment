using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
/// <summary>
/// Enemy class has all the needs for a basic enemy. Enemies differ by visuals and properties such as speed and health so there is no need for 
/// inheritance for each type of enemies.
/// </summary>
public class Enemy : MonoBehaviour
{
    public EnemySO properties;

    public float health;

    [SerializeField] GameObject objectThatContainsTheVisual;

    public NavMeshAgent agent;

    [Header("Debuffs")]
    public bool isSlowed;
    public float slowedPower = 0;

    public Slider healthBar;

    public GameObject moneyAfterDeathDisplayCanvasPrefab;

    private void OnEnable()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.speed = properties.speed;
        health = properties.health;

        //Bus.Sync.Subscribe<EndOfGame>(OnGameEnd);

        if (properties.visuals != null)
        {
            objectThatContainsTheVisual.GetComponent<MeshRenderer>().material = properties.visuals;
        }
    }

    private void OnDisable()
    {
        //Bus.Sync.Unsubscribe<EndOfGame>(OnGameEnd);
    }

    /*void OnGameEnd(object sender, EventArgs args)
    {
        StartCoroutine(WaitForBusAndSelfDestroy());
    }*/
    public void Damage(float damage)
    {
        health -= damage;

        healthBar.value = health / properties.health;
        //Debug.Log("Got shot for" + damage);
        if (health <= 0)
        {
            Bus.Sync.Publish(this.gameObject, new OnEnemyDeathEvent());
            GameManager.Instance.AddMoney(properties.killingValue);
            Instantiate(moneyAfterDeathDisplayCanvasPrefab, this.transform.position, Quaternion.Euler(90, 0, 0)).GetComponentInChildren<TMP_Text>().text = "+" + properties.killingValue.ToString();
            //Debug.Log("About to destroy myself");
            Destroy(this.gameObject);
        }
    }

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
}
