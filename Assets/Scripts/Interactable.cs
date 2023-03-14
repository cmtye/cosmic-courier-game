using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Interactable : MonoBehaviour
{
    private OutlineHighlight _outlineHighlight;
    private void Start()
    {
        _outlineHighlight = GetComponent<OutlineHighlight>();
        if (_outlineHighlight)
        {
            _outlineHighlight.enabled = false;
        }
    }

    public GameObject Interact()
    {
        Debug.Log("I was just interacted with!");
        // TODO: call InteractionHandler
        return gameObject;
    }

}
