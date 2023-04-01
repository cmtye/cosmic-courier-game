using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level_Scripts.Interaction
{
    public class StorageHandler : InteractionHandler
    {
        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int Close = Animator.StringToHash("Close");

        public override void Handle(PlayerController player)
        {
            if (player.currentlyHeld != null && player.currentlyHeld.CompareTag("Item"))
            {
                // Take from the player 
                var item = player.TransferHeldItem(gameObject);
                var animator = GetComponent<Animator>();
                animator.SetTrigger(Open);

                // TODO: different types of resources?
                // Increment stored amount by item value
                var value = item.GetComponent<ItemController>().GetValue();
                GameManager.Instance.Store(value);
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
