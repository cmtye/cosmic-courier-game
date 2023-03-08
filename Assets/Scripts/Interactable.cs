using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    public GameObject Interact()
    {
        Debug.Log("I was just interacted with!");
        // TODO: call InteractionHandler
        return gameObject;
    }

}
