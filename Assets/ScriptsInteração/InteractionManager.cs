using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using UnityEngine;
using TMPro;

public class InteractionManager : MonoBehaviour
{
    public float interactRange = 3f;
    public GameObject interactionUI; // Arraste o Canvas aqui
    public TextMeshProUGUI interactionText;

    private Interactable currentTarget;

    [SerializeField] private ExpeditionSceneController _sceneController;
    
    void Update()
    {
        CheckForInteractable();

        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            currentTarget.Interact();
        }
    }

    void CheckForInteractable()
    {
        
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                if (_sceneController.ExpeditionIsRunning() == true && interactable.InteractionTypeValue == Interactable.InteractionType.Shop)
                {
                    // Durante expedição não usa a loja
                    return;
                    
                }
                currentTarget = interactable;
                interactionUI.SetActive(true);
                interactionText.text = interactable.interactionMessage;
                return;
            }
        }

        currentTarget = null;
        interactionUI.SetActive(false);
    }

}