using UnityEngine;
using GRD.FSM;
using BGJ14;

[CreateAssetMenu(fileName = "BossAttackBehavior", menuName = "FSM/Boss/Attack")]
public class BossAttackBehavior : FSM_StateBehaviour
{
    private FSM_Manager fsm_Manager;
    private BossController bossController;

    public override void Setup(FSM_Manager manager)
    {
        base.Setup(manager);
        fsm_Manager = manager;
        bossController = manager.GetComponent<BossController>();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
