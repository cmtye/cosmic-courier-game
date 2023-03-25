using UnityEngine;

namespace Tower_Scripts.Components
{
    [CreateAssetMenu(menuName = "Towers/Components/BuzzsawAttack")]
    public class BuzzsawAttack : TowerComponent
    {
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

            foreach (Transform t in tower.gameObject.transform)
            {
                if (t.CompareTag("Rotating"))
                {
                    // Quaternions are weird, hard coded for saturn ring but should probably fix
                    var lookRotation =
                        Quaternion.LookRotation((tower.firingPoint.position - tower.targetEnemy.transform.position).normalized);
                    lookRotation *= Quaternion.Euler(0, 90, 0);
                    t.rotation = lookRotation;
                }
            }
            foreach (var e in tower.enemiesInRange)
            {
                e.GetHealth().DealDamage(towerInfo.towerDamage, towerInfo.damageType);
            }
            
            tower.attackCooldown = tower.towerData.info.attackTimer;
        }
    }
}
