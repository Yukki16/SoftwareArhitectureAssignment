using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReachedEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag.Equals(Tags.T_Enemy))
        {
            Bus.Sync.Publish(this, new OnEnemyReachedBase(other.gameObject));
            Bus.Sync.Publish(this, new UpdateUI());
        }
    }
}
