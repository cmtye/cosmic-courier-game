namespace Interaction
{
    public class DepotHandler : InteractionHandler
    {
        public override void Handle(PlayerController player)
        {
            // If the player interact with the depot while full, they'll get the win screen regardless of item
            if (GameManager.Instance.CheckDepotFull())
            {
                GameManager.Instance.Deposit();
                if (player.currentlyHeld != null &&
                    player.currentlyHeld.CompareTag("Item") &&
                    player.currentlyHeld.GetComponent<ItemController>().GetTier() == Item.DarkMatter)
                {
                    var item = player.TransferHeldItem(gameObject);
                    Destroy(item, 1);
                }
                return;
            }
            
            if (player.currentlyHeld != null && 
                player.currentlyHeld.CompareTag("Item") &&
                player.currentlyHeld.GetComponent<ItemController>().GetTier() == Item.DarkMatter)
            {
                // Take from the player 
                var item = player.TransferHeldItem(gameObject);

                // Increment deposit count
                GameManager.Instance.Deposit();
                Destroy(item, 1);
            }
        }
    }
}