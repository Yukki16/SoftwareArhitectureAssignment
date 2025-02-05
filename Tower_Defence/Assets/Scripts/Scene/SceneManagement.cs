using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void LoadScene(SceneNames nameOfTheScene)
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(nameOfTheScene.ToString());
        GameManager.Instance._stateMachine.UpdateState(TransitionConditions.Con_Empty);
        GameManager.Instance.StopAllCoroutines();
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(sceneName);
        GameManager.Instance.StopAllCoroutines();
        if (sceneName != "Pathfind")
        {
            GameManager.Instance._stateMachine.UpdateState(TransitionConditions.Con_Empty);
        }
        else
        {
            GameManager.Instance._stateMachine.UpdateState(TransitionConditions.Con_Build);
            GameManager.Instance.StartCoroutine(GameManager.Instance.BuildingCountDown());
        }
    }
    public void ReloadScene()
    {
        Debug.Log("Reloading Scene");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameManager.Instance.StopAllCoroutines();
        if (SceneManager.GetActiveScene().name != "Pathfind")
            GameManager.Instance._stateMachine.UpdateState(TransitionConditions.Con_Empty);
        else
        {
            GameManager.Instance._stateMachine.UpdateState(TransitionConditions.Con_Build);
            GameManager.Instance.StartCoroutine(GameManager.Instance.BuildingCountDown());
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
