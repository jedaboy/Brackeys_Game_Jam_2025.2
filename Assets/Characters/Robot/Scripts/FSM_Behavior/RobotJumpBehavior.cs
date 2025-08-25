using UnityEngine;
using GRD.FSM;
using BGJ14;

[CreateAssetMenu(fileName="RobotJumpBehavior",menuName ="FSM/Robot/Jump")]
public class RobotJumpBehavior : FSM_StateBehaviour
{
    private FSM_Manager fsm_Manager;
    private RobotController robotController;
    [SerializeField] private AnimationCurve jumpSpeed;
    float jumpTime;
    private float currentJumpSpeed;

    public override void Setup(FSM_Manager manager)
    {
        base.Setup(manager);
        fsm_Manager = manager;
        robotController = manager.GetComponent<RobotController>();
    }
    public override void OnEnter()
    {
        jumpTime = 0;
        UpdateJumpSpeed();
        base.OnEnter();
    }
    public override void OnExit()
    {
        base.OnExit();
        fsm_Manager.SetBool("Jump", false);

    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        robotController.MoveInput();
        jumpTime += Time.deltaTime;
        UpdateJumpSpeed();

        if(jumpTime >= jumpSpeed.keys[^1].time)
        {
            fsm_Manager.SetBool("Jump", false);
        }
    }
    public override void OnFixedUpdate() 
    {
        base.OnFixedUpdate();
        robotController.rigidbody.velocity = new Vector3(robotController.rigidbody.velocity.x, 
            currentJumpSpeed, 
            robotController.rigidbody.velocity.z);
    }
    private void UpdateJumpSpeed()
    {
        currentJumpSpeed = jumpSpeed.Evaluate(jumpTime);
    }

}
