using UnityEngine;
using GRD.FSM;
using BGJ14;

[CreateAssetMenu(fileName = "EnemyRobotFleeBehavior", menuName = "FSM/EnemyRobot/Flee")]
public class EnemyRobotFleeBehavior : FSM_StateBehaviour
{
    private FSM_Manager fsm_Manager;
    private EnemyRobotController enemyRobotController;

    private void MoveToSafeZone()
    {
        // Encontra todas as Safe Zones
        GameObject[] safeZones = GameObject.FindGameObjectsWithTag("Safe Zone");
        if (safeZones.Length == 0) return;

        // Pega a mais próxima
        GameObject nearest = null;
        float minDist = Mathf.Infinity;
        foreach (var sz in safeZones)
        {
            float dist = Vector3.Distance(enemyRobotController.transform.position, sz.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = sz;
            }
        }

        if (nearest != null)
            enemyRobotController.MoveTo(nearest.transform.position);
    }

    public override void Setup(FSM_Manager manager)
    {
        base.Setup(manager);
        fsm_Manager = manager;
        enemyRobotController = manager.GetComponent<EnemyRobotController>();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        MoveToSafeZone();
        Debug.Log("Entrou em Flee");
    }
}
