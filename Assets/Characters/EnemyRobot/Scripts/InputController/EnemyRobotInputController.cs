using UnityEngine;

namespace BGJ14
{
    public class EnemyRobotInputController : MonoBehaviour
    {
        public Vector3 Move { get; private set; }
        public bool Attack { get; private set; }

        public void SetMove(Vector3 move)
        {
            Move = move;
        }

        public void SetAttack(bool attack)
        {
            Attack = attack;
        }
    }
}
