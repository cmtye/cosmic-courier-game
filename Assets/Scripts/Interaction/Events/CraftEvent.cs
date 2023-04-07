using System;
using UnityEngine;

namespace Interaction.Events
{
    [CreateAssetMenu(menuName = "InteractionEvents/Craft")]
    public class CraftEvent : InteractionEvent
    {
        public static event Action<PlayerController, InteractionHandler, GameObject> OnTowerCraft;

        public GameObject Tower;

        public override void Raise(PlayerController player, InteractionHandler handler)
        {
            OnTowerCraft?.Invoke(player, handler, Tower);
        }
    }
}
