namespace Interaction
{
    public class DepotHandler : InteractionHandler
    {
        public override void Handle(PlayerController player)
        {
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