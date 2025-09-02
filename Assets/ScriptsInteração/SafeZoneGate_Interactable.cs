using System.Collections;
using UnityEngine;

public class SafeZoneGate_Interactable : Interactable
{
    [SerializeField] private ExpeditionSceneController _sceneController;
    [SerializeField] private GameObject _gate;
    [SerializeField] private Material material;

    private bool _open;
    private bool _canInteract = true; // Flag para cooldown

    private void Awake()
    {
        InteractionTypeValue = InteractionType.Gate;

        material.SetFloat("_Power_Min", 0.28f);
        material.SetFloat("_Power_Max", 1f);
    }

    public override void Interact()
    {
        // Impede interação rápida
        if (!_canInteract) 
            return;

        StartCoroutine(InteractionCooldown(0.3f)); // Tempo de delay

        if (_open)
        {
            CloseGate();
            _sceneController.EndExpedition();
            return;
        }

        base.Interact();
        OpenGate();
        _sceneController.StartExpedition();
    }

    private void OpenGate()
    {
        _open = true;
        StartCoroutine(openGate(1f));
    }

    public void CloseGate()
    {
        _open = false;
        StartCoroutine(closeGate(0.28f, 1f, 2f));
    }

    private IEnumerator openGate(float duration)
    {
        material.SetFloat("_Power_Min", 0.28f);
        material.SetFloat("_Power_Max", 0.28f);

        material.SetFloat("_Power_Max", 4f);
        yield return new WaitForSeconds(duration);
        _gate.SetActive(false);
        material.SetFloat("_Power_Min", 4f);
    }

    private IEnumerator closeGate(float startValue, float endValue, float duration)
    {
        material.SetFloat("_Power_Min", startValue);
        yield return new WaitForSeconds(duration);
        _gate.SetActive(true);
        material.SetFloat("_Power_Max", endValue);
    }

    // Cooldown da interação
    private IEnumerator InteractionCooldown(float time)
    {
        _canInteract = false;
        yield return new WaitForSeconds(time);
        _canInteract = true;
    }
}
