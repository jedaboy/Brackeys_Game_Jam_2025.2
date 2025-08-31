using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactionMessage = "Press E to Interact";
    public enum InteractionType { Shop, Gate, Barrel }

    public InteractionType InteractionTypeValue;
    public virtual void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}