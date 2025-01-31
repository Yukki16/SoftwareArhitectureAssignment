using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] TargetBehaviour targetingState;
    [SerializeField] TowerInfo towerInfo;

    [Header("Upgrades")]
    [SerializeField] int upgradeCost;
    [Range(0f, 100f), SerializeField] int damageUpgradePercentage;
    [SerializeField] int damageGoldValue;
    [Range(0f, 100f), SerializeField] int attackIntervalUpgradePercentage;
    [SerializeField] int attackIntervalGoldValue;
    [Range(0f, 100f), SerializeField] int rangeUpgradePercentage;
    [SerializeField] int rangeGoldValue;

    [Header("Tower Stats")]
    [SerializeField] int goldValue;
    [SerializeField] float attackInterval;
    [SerializeField] public float damage;
    [SerializeField] float range;

    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;
    //[HideInInspector]
    public Transform projectileSpawn;

    [Header("Hit Box")]
    [SerializeField] Hitbox hitbox;

    [Header("Tower level")]
    [SerializeField] GameObject currentUpgradeTowerPrefab;
    [SerializeField] public GameObject objectLookingAtEnemy; //For example the balista bow to look at enemy (i know it might look funky but is because of the asset)
    int towerLevel = 1;

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

        targetingState.PerformAttack(this, hitbox.targets, projectilePrefab);
        StartCoroutine(Attack());
    }

    void CalculateGoldPerStats()
    {
        damageGoldValue = goldValue / (int)damage;
        attackIntervalGoldValue = goldValue / (int)attackInterval;
        rangeGoldValue = goldValue / (int)range;
    }
    private void CalculateUpgradeCost()
    {
        upgradeCost = goldValue + (damageGoldValue * damageUpgradePercentage) / 100 + (attackIntervalGoldValue * attackIntervalUpgradePercentage) / 100 + (rangeGoldValue * rangeUpgradePercentage) / 100;
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
        projectileSpawn = new List<GameObject>(GameObject.FindGameObjectsWithTag(Tags.T_ArrowSpawn)).Find(g => g.transform.IsChildOf(currentUpgradeTowerPrefab.transform)).transform;
        objectLookingAtEnemy = new List<GameObject>(GameObject.FindGameObjectsWithTag(Tags.T_Bow)).Find(g => g.transform.IsChildOf(currentUpgradeTowerPrefab.transform));
    }
}
