using System.Collections;
using UnityEngine;

namespace Interaction
{
    public class StorageHandler : InteractionHandler
    {
        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int Close = Animator.StringToHash("Close");

        public override void Handle(PlayerController player)
        {
            if (player.currentlyHeld != null && 
                player.currentlyHeld.CompareTag("Item") && 
                player.currentlyHeld.GetComponent<ItemController>().GetTier() != Item.DarkMatter)
            {
                // Take from the player 
                var item = player.TransferHeldItem(gameObject);
                var animator = GetComponent<Animator>();
                animator.SetTrigger(Open);

                // Pass item tier to storage 
                var tier = item.GetComponent<ItemController>().GetTier();
                GameManager.Instance.Store(tier);
                StopCoroutine(nameof(CloseStorage));
                StartCoroutine(nameof(CloseStorage));
                Destroy(item, 1);
            }
        }

        private IEnumerator CloseStorage()
        {
            yield return new WaitForSeconds(2f);
            var animator = GetComponent<Animator>();
            animator.ResetTrigger(Open);
            animator.SetTrigger(Close); 
        }
    }
}
