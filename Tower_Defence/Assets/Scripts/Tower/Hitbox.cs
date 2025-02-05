using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    //[HideInInspector]
    public List<Transform> targets = new List<Transform>();

    public SphereCollider hitboxCollider;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag.Equals(Tags.T_Enemy))
        {
            Debug.Log("Enemy entered range");
            targets.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag.Equals(Tags.T_Enemy))
        {
            targets.Remove(other.transform);
        }
    }

    private void OnEnable()
    {
        Bus.Sync.Subscribe<EnemyDeathEvent>(RemoveEnemy);
    }


    private void OnDisable()
    {
        Bus.Sync.Unsubscribe<EnemyDeathEvent>(RemoveEnemy);
    }

    private void RemoveEnemy(EnemyDeathEvent @event)
    {
        if (targets.Contains(@event.target.transform))
        {
            StartCoroutine(RemoveNextFrame(@event.target.transform));
        }
    }

    IEnumerator RemoveNextFrame(Transform target)
    {
        yield return new WaitForEndOfFrame();
        targets.Remove(target);
    }
}
