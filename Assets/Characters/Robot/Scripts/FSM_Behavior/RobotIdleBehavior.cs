using UnityEngine;
using GRD.FSM;
using BGJ14;


[CreateAssetMenu(fileName = "RobotIdleBehavior", menuName = "FSM/Robot/Idle")]
public class RobotIdleBehavior : FSM_StateBehaviour
{
    private FSM_Manager fsm_Manager;
    private RobotController robotController;

    public override void Setup(FSM_Manager manager)
    {
        base.Setup(manager);
        fsm_Manager = manager;
        robotController = manager.GetComponent<RobotController>();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (robotController.ChecKGroundStatus())
        {
            Debug.Log(robotController.ChecKGroundStatus());
            robotController.MoveInput();
            robotController.JumpInput();
        }
    }
}
