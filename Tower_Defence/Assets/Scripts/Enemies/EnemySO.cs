using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Properties")]
public class EnemySO : ScriptableObject
{
    public string nameOfEnemy;

    public float health;
    public float speed;
    public int killingValue;

    public Material visuals;
}
