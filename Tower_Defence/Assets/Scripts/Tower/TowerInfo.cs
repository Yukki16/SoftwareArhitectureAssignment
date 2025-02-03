using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTowerInfo", menuName = "ScriptableObjects/Tower Info")]
public class TowerInfo : ScriptableObject
{
    public int costValue;
    public float damage;
    public float range;
    public float attackInterval;

    public GameObject projectilePrefab;

    public GameObject TowerPrefab;
    public GameObject Lv2UpgradePrefab;
    public GameObject Lv3UpgradePrefab;
}
