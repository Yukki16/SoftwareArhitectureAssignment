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
        _buttonText.text = $"{_towerInfo.name} \n Damage: {_towerInfo.damage} " +
            $"\n Attack interval: {_towerInfo.attackInterval}" +
            $"\n Range: {_towerInfo.range}" +
            $"\n COST: {_towerInfo.costValue}";
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
        yield return new WaitUntil(() => GameManager.Instance.coins >= _towerInfo.costValue);
        _thisButton .interactable = true;
    }
    public void BuyTower()
    {
        objectRequestingBuilding.GetComponent<TowerPlacement>().BuildTower(_towerInfo.TowerPrefab, _towerInfo.costValue);
    }    
}
