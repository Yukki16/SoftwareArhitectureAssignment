using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementPositionMouse : MonoBehaviour
{
    [SerializeField] Camera sceneCamera;

    [SerializeField] LayerMask placementLayer;

    Vector3 lastPosition;
    Vector3 lastNormal;
    

    public Vector3 GetRayHitInWorldForPlacement()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 200, placementLayer))
        {
            lastPosition = hit.point;
        }

        return lastPosition;
    }

    public Vector3 GetRayHitNormalInWorldForPlacement()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 200, placementLayer))
        {
            lastNormal = hit.normal;
        }

        return lastNormal;
    }
}
