using UnityEngine;

namespace Interaction
{
    public class ItemController : MonoBehaviour
    {
        [SerializeField] private int value = 1;
        public bool canPickup = true;
        public int GetValue()
        {
            return value;
        }

    }
}