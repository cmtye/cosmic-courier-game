using System.Collections;
using UnityEngine;
using UX.RadialMenu;

namespace Interaction
{
    public class CraftHandler : InteractionHandler
    {
        public Ring ringData;
        public float distanceThreshold;

        [SerializeField] private AudioClip craftNoise;

        public override void Handle(PlayerController player)
        {
            AudioManager.Instance.PlaySound(craftNoise, .45f);
            GameManager.Instance.DampenBGM(true);

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
