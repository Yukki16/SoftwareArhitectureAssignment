using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPauseState", menuName = "ScriptableObjects/StateMachine/PauseState")]
public class PauseSO : StateSO
{
    public override void OnEnter(GameObject owner)
    {
        Time.timeScale = 0f;
    }

    public override void OnExit(GameObject owner)
    {
        Time.timeScale = 1f;
    }
}
