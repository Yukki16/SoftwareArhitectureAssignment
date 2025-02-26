using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image _enemyHealthBar;

    private void OnEnable()
    {
        _enemyHealthBar = gameObject.GetComponent<Image>();
        Bus.Sync.Subscribe<EnemyTakesDamageEvent>(UpdateHP);
    }

    private void OnDisable()
    {
        Bus.Sync.Unsubscribe<EnemyTakesDamageEvent>(UpdateHP);
    }
    private void UpdateHP(EnemyTakesDamageEvent @event)
    {
        if(this.transform.IsChildOf(@event.target.transform))
        {
            Debug.Log("Received msg");
            _enemyHealthBar.fillAmount = @event.target.GetComponent<Enemy>().ReturnPercentHP();
        }
    }


}
