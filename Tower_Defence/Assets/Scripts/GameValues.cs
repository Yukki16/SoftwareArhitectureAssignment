using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newGameValues", menuName = "ScriptableObjects/Game Values")]
public class GameValues : ScriptableObject
{
    public int lifes;
    public int startingCoins;

    [Range(5, 10)]
    public int maxWaveNumber = 5;

    //Spawn time between enemies
    [Range(0.1f, 10f)]
    public float spawnDelay;
}
