using UnityEngine;
using Utility;

namespace Utility.Interaction {

    [RequireComponent(typeof(InteractionHandler))]
    [RequireComponent(typeof(OutlineHighlight))]
    public class Interactable : MonoBehaviour
    {
    private OutlineHighlight _outlineHighlight;
    private InteractionHandler _handler;
    private void Start()
    {
        _outlineHighlight = GetComponent<OutlineHighlight>();
        _handler = GetComponent<InteractionHandler>();
        if (_outlineHighlight)
        {
            _outlineHighlight.enabled = false;
        }
    }

    public GameObject Interact()
    {
        return _handler.Handle();
    }

    }
}
