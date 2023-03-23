using UnityEngine;

namespace Tower_Scripts.Components
{
    [CreateAssetMenu(menuName = "Towers/Components/ProjectileAttack")]
    public class ProjectileAttack : TowerComponent
    {
        [SerializeField] private GameObject projectilePrefab;

        [SerializeField] private float projectileSpeed = 20f;
        [SerializeField] private float damageDistance = 0.1f;
        public override void Enabled(BaseTower tower)
        {
            
        }
        
        public override void Execute(BaseTower tower)
        {
            var towerInfo = tower.towerData.info;
            if (tower.attackCooldown > 0)
            {
                tower.attackCooldown -= Time.deltaTime;
                return;
            }
            
            if (!tower.targetEnemy)
            {
                return;
            }

            tower.transform.rotation =
                Quaternion.LookRotation((tower.targetEnemy.transform.position - tower.transform.position).normalized);
            var projectile = Instantiate(projectilePrefab, tower.firingPoint.position, Quaternion.identity);
            var projectileBehavior = projectile.GetComponent<ProjectileBehavior>();
            projectileBehavior.target = tower.targetEnemy;
            projectileBehavior.SetParams(projectileSpeed, towerInfo.towerDamage, towerInfo.damageType, damageDistance);
            
            tower.attackCooldown = tower.towerData.info.attackTimer;
        }
    }
}
