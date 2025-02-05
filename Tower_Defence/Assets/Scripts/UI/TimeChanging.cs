using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeChanging : MonoBehaviour
{
    float timeRemember = 1f;
    public void ReduceGameSpeed()
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale - 0.5f, 0f, Time.timeScale);
        timeRemember = Time.timeScale;
        Bus.Sync.Publish(this, new TimeChanged());
        Debug.Log(Time.timeScale);
    }

    public void IncreaseGameSpeed()
    {
        Time.timeScale = Time.timeScale + 0.5f;
        timeRemember = Time.timeScale;
        Bus.Sync.Publish(this, new TimeChanged());
        Debug.Log(Time.timeScale);
    }

    public void PauseOrUnpause()
    {
        if(GameManager.Instance._stateMachine.currentState == GameManager.Instance.pause)
        {
            GameManager.Instance._stateMachine.ResumePreviousState();
            Time.timeScale = timeRemember;
        }
        else
        {
            GameManager.Instance._stateMachine.UpdateState(TransitionConditions.Con_Pause);
            Time.timeScale = 0f;
        }
    }
}
