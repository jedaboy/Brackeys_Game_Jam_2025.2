using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactionMessage = "Press E to Interact";

    public virtual void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}