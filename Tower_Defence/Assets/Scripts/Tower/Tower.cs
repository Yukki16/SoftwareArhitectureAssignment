using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] TargetBehaviour targetingState;
    [SerializeField] public TowerInfo towerInfo;

    [Header("Upgrades")]
    [SerializeField] public int upgradeCost;
    [Range(0f, 100f), SerializeField] public int damageUpgradePercentage;
    [SerializeField] int damageGoldValue;
    [Range(0f, 100f), SerializeField] public int attackIntervalUpgradePercentage;
    [SerializeField] int attackIntervalGoldValue;
    [Range(0f, 100f), SerializeField] public int rangeUpgradePercentage;
    [SerializeField] int rangeGoldValue;

    [Header("Tower Stats")]
    [SerializeField] int goldValue;
    [SerializeField] public float attackInterval;
    [SerializeField] public float damage;
    [SerializeField] public float range;

    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;
    //[HideInInspector]
    public Transform projectileSpawn;

    [Header("Hit Box")]
    [SerializeField] Hitbox hitbox;

    [Header("Tower level")]
    [SerializeField] GameObject currentUpgradeTowerPrefab;
    [SerializeField] public GameObject objectLookingAtEnemy; //For example the balista bow to look at enemy (i know it might look funky but is because of the asset)
    [HideInInspector] public int towerLevel = 1;

    private void OnEnable()
    {
        attackInterval = towerInfo.attackInterval;
        damage = towerInfo.damage;
        range = towerInfo.range;
        goldValue = towerInfo.costValue;
        
        hitbox.hitboxCollider.radius = range;

        projectilePrefab = towerInfo.projectilePrefab;
        UpdateProjectileSpawnAndLookAt();


        CalculateGoldPerStats();
        CalculateUpgradeCost();

        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        yield return new WaitUntil(() => hitbox.targets.Count > 0);
        yield return new WaitForSeconds(attackInterval);
        //yield return new WaitForNextFrameUnit();
        targetingState.PerformAttack(this, hitbox.targets, projectilePrefab);
        StartCoroutine(Attack());
    }

    void CalculateGoldPerStats()
    {
        float totalStatValue = damage + (1 / attackInterval) + range; // Normalized contribution
        damageGoldValue = (int)(goldValue * (damage / totalStatValue));
        attackIntervalGoldValue = (int)(goldValue * ((1 / attackInterval) / totalStatValue));
        rangeGoldValue = (int)(goldValue * (range / totalStatValue));
    }
    private void CalculateUpgradeCost()
    {
        float totalStatIncreaseFactor = (damageUpgradePercentage / 100f)
                                  + (attackIntervalUpgradePercentage / 100f)
                                  + (rangeUpgradePercentage / 100f);

        upgradeCost = (int)(goldValue * (1 + totalStatIncreaseFactor));
    }

    public void Upgrade()
    {
        GameManager.Instance.UsedCoins(upgradeCost);
        towerLevel++;
        Destroy(currentUpgradeTowerPrefab);

        goldValue = upgradeCost;
        CalculateUpgradeCost();

        damage = damage + (damageUpgradePercentage * damage) / 100;
        Mathf.Clamp(attackInterval - (attackIntervalUpgradePercentage * attackInterval) / 100, 0.1f, attackInterval);
        range = range + (range * rangeUpgradePercentage) / 100;

        hitbox.hitboxCollider.radius = range;

        switch (towerLevel)
        {
            case 2:
                currentUpgradeTowerPrefab = Instantiate(towerInfo.Lv2UpgradePrefab, transform.parent.position, Quaternion.identity, transform.parent);
                break;
            case 3:
                currentUpgradeTowerPrefab = Instantiate(towerInfo.Lv3UpgradePrefab, transform.parent.position, Quaternion.identity, transform.parent);
                break;

        }

        UpdateProjectileSpawnAndLookAt();
    }

    private void UpdateProjectileSpawnAndLookAt()
    {
        if (currentUpgradeTowerPrefab != null && !(targetingState is EffectTargetingBehaviour))
        {
            projectileSpawn = new List<GameObject>(GameObject.FindGameObjectsWithTag(Tags.T_ArrowSpawn)).Find(g => g.transform.IsChildOf(currentUpgradeTowerPrefab.transform)).transform;
            objectLookingAtEnemy = new List<GameObject>(GameObject.FindGameObjectsWithTag(Tags.T_Bow)).Find(g => g.transform.IsChildOf(currentUpgradeTowerPrefab.transform));
        }
    }
}
