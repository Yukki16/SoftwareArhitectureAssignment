using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITowerCheck : MonoBehaviour
{
    [SerializeField] GameObject _buyPanel;
    [SerializeField] GameObject _upgradePanel;
    List<BuyButton> _buttons = new List<BuyButton>();
    UpgradeButton _upgradeButton;
    TMP_Text _upgradeText;
    TowerPlacement _placement;

    [SerializeField] LayerMask _layerToCheckOn;


    private void Start()
    {
        Bus.Sync.Subscribe<UpdateUI>(SetUpgradeText);
        _buttons.AddRange(_buyPanel.GetComponentsInChildren<BuyButton>());
        _upgradeButton = _upgradePanel.GetComponentInChildren<UpgradeButton>();
        _upgradeText = _upgradePanel.GetComponentInChildren<TMP_Text>();
    }

    

    void Update()
    {
        if (!(GameManager.Instance._stateMachine.currentState is BuildingSO))
        {
            _upgradePanel.SetActive(false);
            _buyPanel.SetActive(false);
            return;
        }
            //if mouse button (left hand side) pressed instantiate a raycast
            if (Input.GetMouseButtonDown(0))
        {
            
                //create a ray cast and set it to the mouses cursor position in game
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerToCheckOn))
                {
                    if(_placement != null)
                        _placement.selectedArrow.SetActive(false);
                    if (hit.transform.tag.Equals(Tags.T_TowerPlacement))
                    {
                        _placement = hit.transform.gameObject.GetComponent<TowerPlacement>();
                        _placement.selectedArrow.SetActive(true);
                        if (!_placement.hasTower)
                        {
                            foreach (var button in _buttons)
                            {
                                button.objectRequestingBuilding = hit.transform.gameObject;
                            }
                            _buyPanel.SetActive(true);
                            _upgradePanel.SetActive(false);
                        }
                        else
                        {
                            _upgradeButton.objectRequestingUpgrading = hit.transform.gameObject;
                            _buyPanel.SetActive(false);
                            _upgradePanel.SetActive(true);
                            
                            SetUpgradeText(_placement);
                        }
                    }

                }
                else
                {
                    _placement.selectedArrow.SetActive(false);
                    _buyPanel.SetActive(false);
                    _upgradePanel.SetActive(false);
                }
            }
            
        }
    }

    private void SetUpgradeText(UpdateUI @event)
    {
        if (_placement == null || !_placement.hasTower)
            return;
        if (_placement.tower.towerInfo.towerName != "IceTower")
        {
            _upgradeText.text = $"{_placement.tower.towerInfo.towerName} " +
            $"\n Damage: {_placement.tower.damage}" +
            $"\n Attack Interval: {_placement.tower.attackInterval}" +
            $"\n Range: {_placement.tower.range}" +
            $"\n" +
            $"\n Upgrade for: {(_placement.tower.towerLevel == 3 ? "Max Level" : _placement.tower.upgradeCost)}" +
            $"\n Damage +{_placement.tower.damageUpgradePercentage}%" +
            $"\n Attack Interval +{_placement.tower.attackIntervalUpgradePercentage}%" +
            $"\n Range +{_placement.tower.rangeUpgradePercentage}%";
        }
        else
        {
            _upgradeText.text = $"{_placement.tower.towerInfo.towerName} " +
            $"\n Slow %: {_placement.tower.damage}" +
            $"\n Attack Interval: {_placement.tower.attackInterval}" +
            $"\n Range: {_placement.tower.range}" +
            $"\n" +
            $"\n Upgrade for: {(_placement.tower.towerLevel == 3 ? "Max Level" : _placement.tower.upgradeCost)}" +
            $"\n Slow +{_placement.tower.damageUpgradePercentage}%" +
            $"\n Attack Interval +{_placement.tower.attackIntervalUpgradePercentage}%" +
            $"\n Range +{_placement.tower.rangeUpgradePercentage}%";
        }
    }

    void SetUpgradeText(TowerPlacement towerPlacement)
    {
        if (towerPlacement.tower.towerInfo.towerName != "IceTower")
        {
            _upgradeText.text = $"{towerPlacement.tower.towerInfo.towerName} " +
            $"\n Damage: {towerPlacement.tower.damage}" +
            $"\n Attack Interval: {towerPlacement.tower.attackInterval}" +
            $"\n Range: {towerPlacement.tower.range}" +
            $"\n" +
            $"\n Upgrade for: {(towerPlacement.tower.towerLevel == 3 ? "Max Level" : towerPlacement.tower.upgradeCost)}" +
            $"\n Damage +{towerPlacement.tower.damageUpgradePercentage}%" +
            $"\n Attack Interval +{towerPlacement.tower.attackIntervalUpgradePercentage}%" +
            $"\n Range +{towerPlacement.tower.rangeUpgradePercentage}%";
        }
        else
        {
            _upgradeText.text = $"{towerPlacement.tower.towerInfo.towerName} " +
            $"\n Slow %: {towerPlacement.tower.damage}" +
            $"\n Attack Interval: {towerPlacement.tower.attackInterval}" +
            $"\n Range: {towerPlacement.tower.range}" +
            $"\n" +
            $"\n Upgrade for: {(towerPlacement.tower.towerLevel == 3 ? "Max Level" : towerPlacement.tower.upgradeCost)}" +
            $"\n Slow +{towerPlacement.tower.damageUpgradePercentage}%" +
            $"\n Attack Interval +{towerPlacement.tower.attackIntervalUpgradePercentage}%" +
            $"\n Range +{towerPlacement.tower.rangeUpgradePercentage}%";
        }
    }

}
