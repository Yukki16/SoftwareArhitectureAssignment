using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] public bool hasTower;

    [SerializeField] public GameObject selectedArrow;

    [SerializeField] public Tower tower;

    public void BuildTower(GameObject towerPrefab, int goldCost)
    {
        if(!hasTower)
        {
            GameManager.Instance.UsedCoins(goldCost);
            tower = Instantiate(towerPrefab, transform.position, transform.rotation, this.transform).GetComponentInChildren<Tower>();
            hasTower = true;
        }
    }

    public void UpgradeTower()
    {
        if(hasTower)
        {
            tower.Upgrade();
        }
    }
}
