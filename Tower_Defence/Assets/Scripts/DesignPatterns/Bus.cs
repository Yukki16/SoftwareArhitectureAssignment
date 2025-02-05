using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// Bus design pattern. Contains all the events that can be Publish.
/// </summary>
public static class Bus
{
    public static readonly BusEvent Sync = new BusEvent();
}

public class TimeChanged : EventArgs
{

}
public class EnemyDeathEvent : EventArgs
{ 
    public GameObject target { get; }

    public EnemyDeathEvent(GameObject _target)
    {
        target = _target;
    }
}

public class OnEnemyReachedBase : EventArgs
{
    public GameObject target { get; }

    public OnEnemyReachedBase(GameObject _target)
    {
        target = _target;
    }
}

public class EnemyTakesDamageEvent : EventArgs
{
    public int damageAmount { get; }
    public GameObject target { get; }
    
    public EnemyTakesDamageEvent(GameObject _target, int _damage)
    {
        target = _target;
        damageAmount = _damage;
    }
}
public class EndOfGame : EventArgs
{ }

public class UpdateUI : EventArgs
{

}
