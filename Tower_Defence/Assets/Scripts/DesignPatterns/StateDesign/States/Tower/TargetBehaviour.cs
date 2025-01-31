using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetBehaviour : StateSO
{     
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tower"></param> Tower that performs the attack
    /// <param name="targets"></param>  The tower list of targets
    /// <param name="projectile"></param> The tower projectile
    public virtual void PerformAttack(Tower tower, List<Transform> targets, GameObject projectile) { }
}
