using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level_Scripts.Interaction
{
    public class StorageHandler : InteractionHandler
    {
        public override void Handle(PlayerController player)
        {
            if (player.currentlyHeld != null && player.currentlyHeld.CompareTag("Item"))
            {
                // Take from the player 
                var item = player.TransferHeldItem(gameObject);

                // TODO: different types of resources?
                // Increment stored amount by item value
                var value = item.GetComponent<ItemController>().GetValue();
                GameManager.Instance.Store(value);
                Destroy(item, 1);
            }
        }
    }
}
