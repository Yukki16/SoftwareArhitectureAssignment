using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDataConvertor : MonoBehaviour
{
    public PathSO pathData;
    public Transform[] scenePathPoints;

    [ContextMenu("Save Path Data")]
    public void SavePathData()
    {
        if (pathData == null) return;

        pathData.pathPoints.Clear();
        foreach (var point in scenePathPoints)
        {
            pathData.pathPoints.Add(point.position);
        }

        Debug.Log("Path data saved!");
    }
}
