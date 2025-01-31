using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTowerDataBase", menuName = "ScriptableObjects/TowerDataBase")]
public class TowerDataBase : ScriptableObject
{
    public List<TowerData> TowerDataList;
}
[Serializable]
public class TowerData
{
    public string name;
    public string ID;
    public Vector2 size;
    public GameObject prefab;
}

