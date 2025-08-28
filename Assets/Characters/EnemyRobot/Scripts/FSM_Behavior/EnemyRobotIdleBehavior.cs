using UnityEngine;
using GRD.FSM;
using BGJ14;

[CreateAssetMenu(fileName = "EnemyRobotIdleBehavior", menuName = "FSM/EnemyRobot/Idle")]
public class EnemyRobotIdleBehavior : FSM_StateBehaviour
{
    private FSM_Manager fsm_Manager;
    private EnemyRobotController enemyRobotController;

    public override void Setup(FSM_Manager manager)
    {
        base.Setup(manager);
        fsm_Manager = manager;
        enemyRobotController = manager.GetComponent<EnemyRobotController>();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
