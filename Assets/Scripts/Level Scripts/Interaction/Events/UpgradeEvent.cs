using Level_Scripts.Interaction;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "InteractionEvents/Upgrade")]
public class UpgradeEvent : InteractionEvent
{
    public static event Action<PlayerController, InteractionHandler> OnTowerUpgrade;

    public override void Raise(PlayerController player, InteractionHandler handler)
    {
        OnTowerUpgrade?.Invoke(player, handler);
    }
}
