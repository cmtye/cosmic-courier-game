using UnityEngine;

namespace Interaction.Events
{
    public abstract class InteractionEvent : ScriptableObject
    { 
        public abstract void Raise(PlayerController player, InteractionHandler handler);
    }
}