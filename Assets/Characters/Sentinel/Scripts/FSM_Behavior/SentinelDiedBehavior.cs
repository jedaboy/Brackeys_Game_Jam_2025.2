using UnityEngine;
using GRD.FSM;
using BGJ14;


[CreateAssetMenu(fileName = "SentinelDiedBehavior", menuName = "FSM/Sentinel/Died")]
public class SentinelDiedBehavior : FSM_StateBehaviour
{
    private FSM_Manager fsm_Manager;
    private SentinelController sentinelController;

    public override void Setup(FSM_Manager manager)
    {
        base.Setup(manager);
        fsm_Manager = manager;
        sentinelController = manager.GetComponent<SentinelController>();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
