using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactionMessage = "Press E to Interact";

    public virtual void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        // Aqui entra a lógica do objeto: abrir porta, coletar item, etc.
    }
}