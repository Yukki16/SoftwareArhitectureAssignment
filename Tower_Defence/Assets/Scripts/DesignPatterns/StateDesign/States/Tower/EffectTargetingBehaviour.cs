using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newEffectTargeting", menuName = "ScriptableObjects/StateMachine/Tower/Effect target")]
public class EffectTargetingBehaviour : TargetBehaviour
{
    public override void PerformAttack(Tower tower, List<Transform> targets, GameObject projectile = null)
    {
        foreach (Transform t in targets)
        {
            var enemy = t.GetComponent<Enemy>();
            //Checked just in case 2 ice towers have the same enemy in the range and it happens the same frame, any order of execution will give the highest slow value.
            if(enemy.slowedPower < tower.damage)
            {
                if(enemy.isSlowed)
                {
                    enemy.StopSlowCoroutine();
                }
                enemy.slowedPower = tower.damage;
                enemy.slowCoroutine = enemy.StartCoroutine(enemy.Slow());
            }
        }
    }
}
