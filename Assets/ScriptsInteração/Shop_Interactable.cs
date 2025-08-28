using UnityEngine;

public class Shop_Interactable : Interactable
{
    [SerializeField] private ExpeditionSceneController _sceneController;
    public override void Interact()
    {
        base.Interact();

        _sceneController.OpenShop();
    }
}
