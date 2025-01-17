using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{

    [SerializeField] GameObject mouseIndicator;

    [SerializeField] PlacementPositionMouse mousePlacement;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePostion = mousePlacement.GetRayHitInWorldForPlacement();
        mouseIndicator.transform.position = mousePostion;
        mouseIndicator.transform.rotation = Quaternion.LookRotation(mousePlacement.GetRayHitNormalInWorldForPlacement());
    }
}
