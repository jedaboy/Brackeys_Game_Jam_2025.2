using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using UnityEngine;
using TMPro;

public class InteractionManager : MonoBehaviour
{
    public float interactRange = 3f;
    public Camera playerCamera;
    public GameObject interactionUI; // Arraste o Canvas aqui
    public TextMeshProUGUI interactionText;

    private Interactable currentTarget;

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
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
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