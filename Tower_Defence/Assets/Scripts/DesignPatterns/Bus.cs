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

public class OnEnemyDeathEvent : EventArgs
{ 
    
}

public class OnEnemyReachedBase : EventArgs
{
}

public class EndOfGame : EventArgs
{ }

public class UpdateUI : EventArgs
{

}
