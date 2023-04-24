using System;
using UnityEngine;

namespace Interaction.Events
{
    [CreateAssetMenu(menuName = "InteractionEvents/Targeting")]
    public class TargetingEvent : InteractionEvent
    {
        public static event Action<PlayerController, InteractionHandler> OnTowerTargeting;

        public override void Raise(PlayerController player, InteractionHandler handler)
        {
            OnTowerTargeting?.Invoke(player, handler);
        }
    }
}
