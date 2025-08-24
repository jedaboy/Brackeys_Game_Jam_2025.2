using UnityEngine;

namespace GRD.FSM
{
    [System.Serializable]
    public class FSM_StateBehaviour : ScriptableObject
    {
        public virtual void Setup(FSM_Manager manager)
        {

        }

        public virtual void OnEnter()
        {

        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnFixedUpdate()
        {

        }

        public virtual void OnLateUpdate()
        {

        }

        public virtual void OnExit()
        {

        }

        #region Collision Events
        public virtual void OnCollisionEnter(Collision collision)
        {

        }

        public virtual void OnCollisionStay(Collision collision)
        {

        }

        public virtual void OnCollisionExit(Collision collision)
        {

        }

        public virtual void OnCollisionEnter2D(Collision2D collision)
        {

        }

        public virtual void OnCollisionStay2D(Collision2D collision)
        {

        }

        public virtual void OnCollisionExit2D(Collision2D collision)
        {

        }
        #endregion

        protected void print(object message)
        {
            Debug.Log(message);
        }
    }
}
