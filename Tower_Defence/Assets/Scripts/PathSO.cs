using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PathScriptableObject", menuName = "ScriptableObjects/Path_Points")]
public class PathSO : ScriptableObject
{
    public List<Vector3> pathPoints;
}
