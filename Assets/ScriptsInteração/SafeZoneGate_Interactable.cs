using System.Collections;
using UnityEngine;

public class SafeZoneGate_Interactable : Interactable
{
    [SerializeField] private ExpeditionSceneController _sceneController;

    [SerializeField] private GameObject _gate;
    [SerializeField] private Material material;

    private bool _open;

    public override void Interact()
    {
        if (_open)
            return;

        base.Interact();

        //TODO: bloquear input do jogador

        _sceneController.StartExpedition();

        OpenGate();
        //TODO: fazer jogador andar para fora da safe zone
        //TODO: reabilitar input do jogador
    }

    private void OpenGate() 
    {
        //TODO: abrir port�o
        _open = true;
        StartCoroutine(openGate(1f));   
    }
    public void CloseGate()
    {
        _open = false;
        StartCoroutine(closeGate(0.28f, 1f, 1f));
    }
    private IEnumerator openGate(float duration)
    {
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
   
}
