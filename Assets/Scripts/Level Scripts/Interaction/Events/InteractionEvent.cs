using UnityEngine;
using Level_Scripts.Interaction;
 
public abstract class InteractionEvent : ScriptableObject
{ 
    public abstract void Raise(PlayerController player, InteractionHandler handler);
}