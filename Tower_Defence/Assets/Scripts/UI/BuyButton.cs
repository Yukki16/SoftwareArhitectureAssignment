using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour
{
    Button _thisButton;
    [SerializeField] TMP_Text _buttonText;
    [SerializeField] TowerInfo _towerInfo;

    public GameObject objectRequestingBuilding;

    private void Start()
    {
        _thisButton = GetComponent<Button>();
        if (_towerInfo.towerName != "IceTower")
        {
            _buttonText.text = $"{_towerInfo.name} \n Damage: {_towerInfo.damage} " +
            $"\n Attack interval: {_towerInfo.attackInterval}" +
            $"\n Range: {_towerInfo.range}" +
            $"\n COST: {_towerInfo.costValue}";
        }
        else
        {
            _buttonText.text = $"{_towerInfo.name} \n Slow Power: {_towerInfo.damage} " +
            $"\n Attack interval: {_towerInfo.attackInterval}" +
            $"\n Range: {_towerInfo.range}" +
            $"\n COST: {_towerInfo.costValue}";
        }

    }
    private void OnEnable()
    {
        StartCoroutine(CheckForAvailability());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        //_thisButton.onClick.RemoveAllListeners();
        _thisButton.interactable = false;
    }

    IEnumerator CheckForAvailability()
    {
        yield return new WaitForEndOfFrame();
        _thisButton.interactable = false;
        yield return new WaitUntil(() => GameManager.Instance.coins >= _towerInfo.costValue);
        _thisButton.interactable = true;
    }
    public void BuyTower()
    {
        if (objectRequestingBuilding != null)
        {
            objectRequestingBuilding.GetComponent<TowerPlacement>().BuildTower(_towerInfo.TowerPrefab, _towerInfo.costValue);
            this.transform.parent.transform.gameObject.SetActive(false);
        }
    }
}
