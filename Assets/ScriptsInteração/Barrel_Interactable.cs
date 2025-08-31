using UnityEngine;
using BGJ_14;

public class Barrel_Interactable : Interactable
{
    [SerializeField] private Scrap scrap;

    [SerializeField] private ExpeditionSceneController _sceneController;

    public void Awake() {
        scrap = gameObject.GetComponent<Scrap>();
    }

    public override void Interact()
    {
        base.Interact();



        if (scrap != null)
        {
            scrap.Activate();
        }

        gameObject.SetActive(false);
    }
}