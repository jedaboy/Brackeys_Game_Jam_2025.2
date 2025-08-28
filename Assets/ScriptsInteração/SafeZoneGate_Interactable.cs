using UnityEngine;

public class SafeZoneGate_Interactable : Interactable
{
    [SerializeField] private ExpeditionSceneController _sceneController;

    [SerializeField] private Collider _gateCollider;
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
        //TODO: abrir portão

        _gateCollider.enabled = false;
        _open = true;
    }

    public void CloseGate() 
    {
        //TODO: fechar portão
        _gateCollider.enabled = true;
        _open = false;
    }
}
