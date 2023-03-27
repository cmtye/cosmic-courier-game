using UnityEngine;
using UI.RadialMenu;
using System.Collections;
using Level_Scripts.Interaction;

namespace Utility.Interaction
{
    public class CraftHandler : InteractionHandler
    {
        public Ring ringData;
        public float distanceThreshold;

        private MenuController _menu;

        private void Start()
        {
            // We should find a way to find the canvas without Find, both for safety and for multiplayer compatability.
            _menu = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<MenuController>();
        }

        public override GameObject Handle(PlayerController player)
        {
            _menu.Setup(ringData);
            _menu.SetActive(true);
            StartCoroutine(HideMenuIfFar(player.gameObject));
            return null;
        }

        private IEnumerator HideMenuIfFar(GameObject player)
        {
            for(;;)
            {
                var distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);
                if (distanceToPlayer > distanceThreshold)
                {
                    HideMenu();
                    yield break;
                }
                yield return new WaitForSeconds(.1f);
            }
        }

        public void HideMenu()
        {
            _menu.SetActive(false);
        }

    }
}
