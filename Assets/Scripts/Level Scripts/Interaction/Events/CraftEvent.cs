using Level_Scripts.Interaction;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "InteractionEvents/Craft")]
public class CraftEvent : InteractionEvent
{
    public static event Action<PlayerController, InteractionHandler> OnTowerCraft;

    public override void Raise(PlayerController player, InteractionHandler handler)
    {
        OnTowerCraft?.Invoke(player, handler);
    }
}
