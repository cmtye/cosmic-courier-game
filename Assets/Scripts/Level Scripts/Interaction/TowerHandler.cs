using System.Collections;
using UnityEngine;
using UI.RadialMenu;
using Level_Scripts.Interaction;


namespace Utility.Interaction
{
    public class TowerHandler : InteractionHandler
    {
        public Ring ringData;
        public float distanceThreshold;

        public override void Handle(PlayerController player)
        {
            player.GetMenu().Setup(ringData, player, this);
            player.GetMenu().SetActive(true);
            StartCoroutine(HideMenuIfFar(player));
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