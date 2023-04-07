using UnityEngine;
using System;
using Interaction;
using Interaction.Events;

[CreateAssetMenu(menuName = "InteractionEvents/Pickup")]
public class PickupEvent : InteractionEvent
{
    public static event Action<PlayerController, InteractionHandler> OnTowerPickup;

    public override void Raise(PlayerController player, InteractionHandler handler)
    {
        OnTowerPickup?.Invoke(player, handler);
    }
}