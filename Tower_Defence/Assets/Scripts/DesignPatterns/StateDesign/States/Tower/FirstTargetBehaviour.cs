using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "TargetFirstEnemyBehaviour", menuName = "ScriptableObjects/StateMachine/Tower/Target First")]
public class FirstTargetBehaviour : TargetBehaviour
{
    public override void PerformAttack(Tower tower, List<Transform> targets, GameObject projectile)
    {
        if(targets.Count == 0) return;

        Transform target = targets.OrderByDescending(e => e.GetComponent<Enemy>().distanceTraveled).FirstOrDefault();
        
        if(target != default || target != null)
        {
            var proj = Instantiate(projectile, tower.projectileSpawn.position, Quaternion.identity);
            proj.GetComponent<Projectile>().SetTarget(target, 10f, (int)tower.damage);
            tower.objectLookingAtEnemy.GetComponent<LookAtEnemy>().target = target.gameObject;
        }
    }
}
