using UnityEngine;
using GRD.FSM;
using BGJ14;

[CreateAssetMenu(fileName = "SentinelIdleBehavior", menuName = "FSM/Sentinel/Idle")]
public class SentinelIdleBehavior : FSM_StateBehaviour
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
