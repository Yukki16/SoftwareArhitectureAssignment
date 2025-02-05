using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateUIText : MonoBehaviour
{
    [SerializeField] TMP_Text _lifeText;
    [SerializeField] TMP_Text _coinsText;
    [SerializeField] TMP_Text _infoText;
    [SerializeField] GameObject _endOfGameText;
    private void OnEnable()
    {
        Bus.Sync.Subscribe<UpdateUI>(OnLifeUpdate);
        Bus.Sync.Subscribe<UpdateUI>(OnCoinsUpdate);
        Bus.Sync.Subscribe<UpdateUI>(OnStateUpdate);
        Bus.Sync.Subscribe<EndOfGame>(GameEnded);
    }

    private void GameEnded(EndOfGame game)
    {
        _endOfGameText.SetActive(true);

        if(GameManager.Instance.lifes > 0)
        {
            _endOfGameText.GetComponentInChildren<TMP_Text>().text = "WIN";
        }
        else
        {
            _endOfGameText.GetComponentInChildren<TMP_Text>().text = "LOSE";
        }
    }

    private void OnStateUpdate(UpdateUI @event)
    {
        if(GameManager.Instance._stateMachine.currentState.StateName == "Building")
        {
            _infoText.text = "Building time: " + GameManager.Instance.timeLeft.ToString();
        }
        else
        {
            _infoText.text = "Wave " + (GameManager.Instance.waveNumber + 1).ToString();
        }
    }

    private void OnDisable()
    {
        Bus.Sync.Unsubscribe<UpdateUI>(OnLifeUpdate);
        Bus.Sync.Unsubscribe<UpdateUI>(OnCoinsUpdate);
        Bus.Sync.Unsubscribe<UpdateUI>(OnStateUpdate);
        Bus.Sync.Unsubscribe<EndOfGame>(GameEnded);
    }

    private void OnCoinsUpdate(UpdateUI @event)
    {
        _coinsText.text = GameManager.Instance.coins.ToString();
    }


    private void OnLifeUpdate(UpdateUI @event)
    {
        _lifeText.text = GameManager.Instance.lifes.ToString();
    }
}
