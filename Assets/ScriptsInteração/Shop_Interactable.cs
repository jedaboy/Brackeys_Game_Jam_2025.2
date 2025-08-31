using UnityEngine;

public class Shop_Interactable : Interactable
{
    [SerializeField] private ExpeditionSceneController _sceneController;

    public void Awake() {
        InteractionTypeValue = InteractionType.Shop;
    }
    public override void Interact()
    {
        base.Interact();

        if (_sceneController.ExpeditionIsRunning() == false)
        {
            _sceneController.OpenShop();
        }
    }
}
