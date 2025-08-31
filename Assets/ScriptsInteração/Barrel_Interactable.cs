using UnityEngine;

public class Barrel_Interactable : Interactable
{
    [SerializeField] private ExpeditionSceneController _sceneController;

    public void Awake() {
        InteractionTypeValue = InteractionType.Barrel;
    }

    public override void Interact()
    {
        base.Interact();

        /*Dropar engranagem*/
    }
}
