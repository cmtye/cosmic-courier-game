using UnityEngine;

namespace Interaction
{
    public abstract class InteractionHandler : MonoBehaviour
    {
        public abstract void Handle(PlayerController player);
    }
}