using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    Button _thisButton;
    [HideInInspector] public Tower _towerInfo;

    public GameObject objectRequestingUpgrading;
    private void OnEnable()
    {
        _towerInfo = objectRequestingUpgrading.GetComponent<TowerPlacement>().tower;
        StartCoroutine(CheckForAvailability());
    }

    private void Start()
    {
        _thisButton = GetComponent<Button>();
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        //_thisButton.onClick.RemoveAllListeners();
        _thisButton.interactable = false;
    }

    public void UpgradeTower()
    {
        if (objectRequestingUpgrading != null)
        {
            objectRequestingUpgrading.GetComponent<TowerPlacement>().UpgradeTower();
            _thisButton.interactable = false;
            StartCoroutine(CheckForAvailability());
            //this.transform.parent.transform.gameObject.SetActive(false);
        }
    }

    IEnumerator CheckForAvailability()
    {
        yield return new WaitForEndOfFrame();
        if (_towerInfo.towerLevel == 3)
        {
            _thisButton.interactable = false;
            yield break;
        }

        yield return new WaitUntil(() => GameManager.Instance.coins >= _towerInfo.upgradeCost);
        _thisButton.interactable = true;
    }
}
