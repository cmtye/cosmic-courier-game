using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utility;

namespace Interaction {

    [RequireComponent(typeof(InteractionHandler))]
    [RequireComponent(typeof(OutlineHighlight))]
    public class Interactable : MonoBehaviour
    {
        public OutlineHighlight OutlineHighlight { get; private set; }
        public DecalProjector DecalProjector { get; private set; }
        
        private InteractionHandler _handler;
        private void Start()
        {
            OutlineHighlight = GetComponent<OutlineHighlight>();
            DecalProjector = GetComponentInChildren<DecalProjector>();
            _handler = GetComponent<InteractionHandler>();
            if (OutlineHighlight)
            {
                OutlineHighlight.enabled = false;
            }

            if (DecalProjector)
            {
                DecalProjector.enabled = false;
            }
        }

        public void Interact(PlayerController player)
        {
            _handler.Handle(player);
        }

    }
}
