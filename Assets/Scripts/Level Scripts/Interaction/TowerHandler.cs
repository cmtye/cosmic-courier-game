using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.RadialMenu;
using Level_Scripts.Interaction;


namespace Utility.Interaction
{
    public class TowerHandler : InteractionHandler
    {
        public Ring ringData;

        public float distanceThreshold;

        public override GameObject Handle(PlayerController player)
        {
            player.GetMenu().Setup(ringData);
            player.GetMenu().SetActive(true);
            StartCoroutine(HideMenuIfFar(player));
            return null;
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


        public void Pickup()
        {
            Debug.Log("The pick up button was pressed");
        }

    }
}