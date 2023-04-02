using Level_Scripts.Interaction;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "InteractionEvents/Upgrade")]
public class UpgradeEvent : InteractionEvent
{
    public static event Action<PlayerController, InteractionHandler, int, int> OnTowerUpgrade;
    [SerializeField] private int upgradeIndex;
    [SerializeField] private int cost;
    public override void Raise(PlayerController player, InteractionHandler handler)
    {
        OnTowerUpgrade?.Invoke(player, handler, upgradeIndex, cost);
    }
}
