using UnityEngine;
using Utility;
using Utility.Interaction;

namespace Level_Scripts.Interaction
{
    public class DepotHandler : InteractionHandler
    {
        public override void Handle(PlayerController player)
        {
            if (player.currentlyHeld != null && player.currentlyHeld.CompareTag("Item"))
            {
                // Take from the player 
                var item = player.TransferHeldItem(gameObject);

                // Increment deposit count by item value
                var value = item.GetComponent<ItemController>().GetValue();
                GameManager.Instance.Deposit(value);
                Destroy(item, 1);
            }
        }
    }
}