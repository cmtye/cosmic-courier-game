using UnityEngine;

namespace Level_Scripts.Interaction
{
    public abstract class InteractionHandler : MonoBehaviour
    {
        public abstract GameObject Handle(PlayerController player);
    }
}