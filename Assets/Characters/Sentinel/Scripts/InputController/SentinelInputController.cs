using UnityEngine;

namespace BGJ14
{
    public class SentinelInputController : MonoBehaviour
    {
        public Vector3 Move { get; private set; }
        public bool Pursuit { get; private set; }

        public void SetMove(Vector3 move)
        {
            Move = move;
        }

        public void SetPursuit(bool pursuit)
        {
            Pursuit = pursuit;
        }
    }
}
