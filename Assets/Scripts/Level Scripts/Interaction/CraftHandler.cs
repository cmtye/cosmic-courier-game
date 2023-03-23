using UnityEngine;
using UI.RadialMenu;
using System.Collections;

namespace Utility.Interaction
{
    public class CraftHandler : InteractionHandler
    {
        public Ring ringData;
        public float distanceThreshold;

        private MenuController _menu;
        private GameObject _player;

        void Start()
        {
            _menu = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<MenuController>();
            _player =  GameObject.Find("Player");
        }

        public override GameObject Handle()
        {
            _menu.Setup(ringData);
            _menu.SetActive(true);
            StartCoroutine(HideMenuIfFar());
            return null;
        }

        private IEnumerator HideMenuIfFar()
        {
            for(;;)
            {
                var distanceToPlayer = Vector3.Distance (_player.transform.position, transform.position);
                if (distanceToPlayer > distanceThreshold)
                {
                    _menu.SetActive(false);
                    yield break;
                }
                yield return new WaitForSeconds(.1f);
            }
        }

    }
}
