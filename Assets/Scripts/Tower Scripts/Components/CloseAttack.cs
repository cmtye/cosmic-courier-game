using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tower_Scripts.Components
{
    [CreateAssetMenu(menuName = "Towers/Components/CloseAttack")]
    public class CloseAttack : TowerComponent
    {
        private Dictionary<BaseTower, Transform> _rotatingObjects;
        public override void Enabled(BaseTower tower)
        {
            // Store the rotator when possible to avoid thousands of property calls
            _rotatingObjects ??= new Dictionary<BaseTower, Transform>();
            UpdateRotator(tower);
        }
        
        public override void Execute(BaseTower tower)
        {
            var towerInfo = tower.towerData.info;
            
            // We want to decrement the timer even if we don't have a target so that
            // the tower is ready to attack once we acquire a new one
            if (tower.attackCooldown > 0)
            {
                tower.attackCooldown -= Time.deltaTime;
                return;
            }
            
            if (!tower.targetEnemy) return;

            // The rotator may change during runtime so we need to make consistent checks on it
            if (!_rotatingObjects.TryGetValue(tower, out var rotator)) UpdateRotator(tower);
            else
            {
                var lookRotation =
                    Quaternion.LookRotation((rotator.transform.position - tower.targetEnemy.transform.position).normalized);
                
                if (rotator) tower.StartCoroutine(SmoothRotator(rotator, lookRotation));
                else UpdateRotator(tower);
            }

            // Deal damage to every enemy in the towers range
            foreach (var e in tower.enemiesInRange)
                e.GetHealth().DealDamage(towerInfo.towerDamage, towerInfo.damageType);

            tower.attackCooldown = tower.towerData.info.attackTimer;
        }

        private void UpdateRotator(BaseTower tower)
        {
            foreach (Transform t in tower.gameObject.transform)
            {
                // The body mesh that we swap out must have the tag TowerBody to be properly recognized
                if (!t.CompareTag("TowerBody")) continue;

                foreach (Transform r in t.gameObject.transform)
                {
                    // The object tagged with Rotating is the part of the mesh we want to rotate towards the enemy
                    if (!r.CompareTag("Rotating")) continue;
                    _rotatingObjects[tower] = r;
                    break;
                }
            }
        }

        private IEnumerator SmoothRotator(Transform rotator, Quaternion target)
        {
            var startRot = rotator.rotation;
            for (float timer = 0; timer < 0.1f; timer += Time.deltaTime)
            {
                // The rotator may have changed during the coroutine, so check each iteration to avoid failure
                if (!rotator) yield break;
                
                // The quaternion facing the enemy has to be converted into an angle and axis for quaternion reasons
                var rotation = Quaternion.Slerp(startRot, target, timer / 0.1f);
                rotation.ToAngleAxis(out var angle, out var axis);
                rotator.rotation = Quaternion.identity;

                // Maximum angle of inclination, for the saturn tower and its rings specifically
                angle = Mathf.Clamp(angle, -55f, 55f);
                rotator.Rotate(axis, angle);
                yield return 0;
            }
        }
    }
}
