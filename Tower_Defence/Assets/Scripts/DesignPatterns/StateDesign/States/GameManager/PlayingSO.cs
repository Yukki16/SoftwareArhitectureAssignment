using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPauseState", menuName = "ScriptableObjects/StateMachine/PlayingState")]
public class PlayingSO : StateSO
{
    Coroutine spawnEnemiesCoroutine = null;
    public override void OnEnter(GameObject owner)
    {
        Bus.Sync.Publish(this, new UpdateUI());
        if(spawnEnemiesCoroutine == null)
        {
            Debug.Log("Started spawning enemies");
            spawnEnemiesCoroutine = GameManager.Instance.StartCoroutine(GameManager.Instance.SpawnEnemies());
        }
        else
        {
            Debug.LogWarning("Ups, you entered the playing state with a non null coroutine to spawn enemies.");
            //Could trigger by mistakes or resuming from pause.
        }
    }
    
    public void SetCoroutine(Coroutine coroutineToSetTo) //Most probably will be set to null by the GameManager upon finishing the coroutine.
    {
        spawnEnemiesCoroutine = coroutineToSetTo;
    }
}
