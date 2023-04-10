using UnityEngine;

namespace Interaction
{
    public class ItemController : MonoBehaviour
    {
        [SerializeField] private Item tier;
        public bool canPickup = true;

        public Item GetTier()
        {
            return tier;
        }
    }
}