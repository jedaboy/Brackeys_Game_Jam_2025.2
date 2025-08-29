using UnityEngine;

public class PlayerSafeZoneTrigger : MonoBehaviour
{
    [SerializeField] private ExpeditionSceneController _sceneController;
    [SerializeField] private SafeZoneGate_Interactable _safeZoneGate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") 
        {
            _sceneController.EndExpedition();
            _safeZoneGate.CloseGate();
        }
    }
}
