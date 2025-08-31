using UnityEngine;
using GRD.FSM;
using BGJ14;

[CreateAssetMenu(fileName = "BossDeathBehavior", menuName = "FSM/Boss/Death")]
public class BossDeathBehavior : FSM_StateBehaviour
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

        public override void OnEnter()
    {
        bossController.anim.SetBool("Dead", true);
        bossController.DestroyCharacter();
        bossController.DropGears();
        base.OnEnter();
    }
}
