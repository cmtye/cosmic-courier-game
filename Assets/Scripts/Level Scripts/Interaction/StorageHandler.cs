using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level_Scripts.Interaction
{
    public class StorageHandler : InteractionHandler
    {
        public override GameObject Handle(PlayerController player)
        {
            if (player.currentlyHeld != null && player.currentlyHeld.CompareTag("Item"))
            {
                // Take from the player 
                var item = player.TakeHeldItem(gameObject);

                // TODO: different types of resources?
                // Increment stored amount by item value
                var value = item.GetComponent<ItemController>().GetValue();
                GameManager.Instance.Store(value);
                Destroy(item, 1);
            }
            return null;
        }
    }
}
