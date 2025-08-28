using UnityEngine;
using GRD.FSM;
using BGJ14;

[CreateAssetMenuAttribute(fileName ="RobotDeathBehaviour", menuName="FSM/Robot/Death")]
public class RobotDeathBehaviour : FSM_StateBehaviour
{
    private FSM_Manager fsm_Manager;
    private RobotController robotController;
   

    public override void Setup(FSM_Manager manager)
    {
        base.Setup(manager);
        fsm_Manager = manager;
        robotController = manager.GetComponent<RobotController>();
    }
    public override void OnEnter()
    {
        robotController.DestroyCharacter();
        base.OnEnter();
    }
  

}
