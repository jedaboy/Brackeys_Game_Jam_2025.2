using UnityEngine;
using GRD.FSM;
using BGJ14;

[CreateAssetMenuAttribute(fileName = "SentinelDeathBeahaviour", menuName = "FSM/Sentinel/Death")]
public class SentinelDeathBeahaviour : FSM_StateBehaviour
{
    private FSM_Manager fsm_Manager;
    private SentinelController sentinelController;


    public override void Setup(FSM_Manager manager)
    {
        base.Setup(manager);
        fsm_Manager = manager;
        sentinelController = manager.GetComponent<SentinelController>();
    }
    public override void OnEnter()
    {
        sentinelController.anim.SetBool("Death", true);
        sentinelController.DestroyCharacter();
        base.OnEnter();
    }

    public override void OnExit()
    {
        sentinelController.anim.SetBool("Death", false);
    }
}