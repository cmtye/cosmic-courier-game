using System.Collections;
using Tower_Scripts;
using UnityEngine;
using UX.RadialMenu;

namespace Interaction
{
    public class TowerHandler : InteractionHandler
    {
        private Ring _ringData;
        public float distanceThreshold;

        public override void Handle(PlayerController player)
        {
            SetTargetingTooltip("Now:" + GetComponent<BaseTower>().CurrentTargeting());
            player.GetMenu().Setup(_ringData, player, this);
            player.GetMenu().SetActive(true);
            StartCoroutine(HideMenuIfFar(player));
        }

        public void SetRingData(Ring data)
        {
            _ringData = data;
        }

        private void SetTargetingTooltip(string tooltip)
        {
            foreach (var b in _ringData.buttons)
            {
                if (b.Text != "Targeting") continue;
                
                b.Tooltip = tooltip;
                break;
            }
        }
        
        private IEnumerator HideMenuIfFar(PlayerController player)
        {
            for(;;)
            {
                var distanceToPlayer = Vector3.Distance (player.gameObject.transform.position, transform.position);
                if (distanceToPlayer > distanceThreshold)
                {
                    HideMenu(player);
                    yield break;
                }
                yield return new WaitForSeconds(.1f);
            }
        }

        public void HideMenu(PlayerController player)
        {
            player.GetMenu().SetActive(false);
        }
    }
}