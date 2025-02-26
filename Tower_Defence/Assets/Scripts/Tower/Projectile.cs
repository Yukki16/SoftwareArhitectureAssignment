using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float speed;
    private int damage;

    [SerializeField] Hitbox hitbox;
    public void SetTarget(Transform newTarget, float newSpeed, int newDamage)
    {
        target = newTarget;
        speed = newSpeed;
        damage = newDamage;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.LookAt(target.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Equals(Tags.T_Enemy))
        {
            if (hitbox != null)
            {
                for (int i = hitbox.targets.Count - 1; i >= 0; i--)
                {
                    Transform enemy = hitbox.targets[i];
                    if (enemy != null)
                    Bus.Sync.Publish(this, new EnemyTakesDamageEvent(enemy.gameObject, damage));
                }
            }
            else
            {
                if(target != null)
                Bus.Sync.Publish(this, new EnemyTakesDamageEvent(target.gameObject, damage));
            }
            Destroy(gameObject);
        }
    }
}
