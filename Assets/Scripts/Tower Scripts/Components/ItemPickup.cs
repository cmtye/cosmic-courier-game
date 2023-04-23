using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interaction;
using UnityEngine;
using Utility;

namespace Tower_Scripts.Components
{
    [CreateAssetMenu(menuName = "Towers/Components/ItemPickup")]
    public class ItemPickup : TowerComponent
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

            if (tower.itemsInRange.Count == 0) return;

            var targetItem = tower.itemsInRange.FirstOrDefault();
            if (!targetItem) return;

            if (targetItem.canPickup == false) return;
            
            var item = targetItem.gameObject;
            targetItem.canPickup = false;
            item.GetComponent<OutlineHighlight>().enabled = false;
            // The rotator may change during runtime so we need to make consistent checks on it
            if (!_rotatingObjects.TryGetValue(tower, out var rotator)) UpdateRotator(tower);
            else
            {
                var transform = rotator.transform;
                var lookDirection = (transform.position - targetItem.transform.position).normalized;
                var lookRotation =
                    Quaternion.FromToRotation(transform.forward, lookDirection);

                if (rotator) tower.StartCoroutine(SmoothRotator(rotator, lookRotation, towerInfo.attackTimer));
                else UpdateRotator(tower);
            }

            if (rotator)
            {
                var transform = rotator.transform;
                var targetPosition = transform.position;
                var duration = .6f;
                item.transform.SetParent(transform);
                tower.StartCoroutine(MoveToPosition(item, targetItem, targetPosition, duration));
                tower.itemsInRange.Remove(targetItem);
            }

            tower.attackCooldown = tower.towerData.info.attackTimer;
        }

        private IEnumerator MoveToPosition(GameObject toMove, ItemController controller, Vector3 position, float timeToMove)
        {
            var currentPosition = toMove.transform.position;
            var t = 0f;
            while(t < 1)
            {
                if (!toMove) yield break;
                t += Time.deltaTime / timeToMove;
                toMove.transform.position = Vector3.Lerp(currentPosition, position, t);
                yield return null;
            }
            var tier = controller.GetTier();
            GameManager.Instance.Store(tier);
            Destroy(toMove);
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

        private IEnumerator SmoothRotator(Transform rotator, Quaternion target, float time)
        {
            var startRot = rotator.rotation;
            for (float timer = 0; timer < time; timer += Time.deltaTime)
            {
                // The rotator may have changed during the coroutine, so check each iteration to avoid failure
                if (!rotator) yield break;

                // The quaternion facing the enemy has to be converted into an angle and axis for quaternion reasons
                var rotation = Quaternion.Slerp(startRot, target, timer / time);
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
