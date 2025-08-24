using UnityEngine;
using System.Linq;

namespace GRD.FSM
{
    [System.Serializable]
    public class FSM_State
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private FSM_StateBehaviour _behaviour;
        private FSM_StateBehaviour _behaviourInstance;
        [SerializeField]
        FSM_Transition[] _transitions;

        public string name => _name;
        public FSM_Transition[] transitions => _transitions;
        public FSM_StateBehaviour behaviour => _behaviourInstance;

        #region Editor
#if UNITY_EDITOR
        public Rect editorRect;
        [System.NonSerialized]
        public bool isDragged;
        [System.NonSerialized]
        public bool isSelected;

        public FSM_State(string stateName, Rect stateRect)
        {
            _name = stateName;
            editorRect = stateRect;
        }

        public void DragBox(Vector2 delta)
        {
            editorRect.position += delta;
        }

        public void AddTransition(int endStateIndex)
        {
            FSM_Transition newTransition = new FSM_Transition(endStateIndex);
            if (_transitions == null)
            {
                _transitions = new FSM_Transition[] { newTransition };
                return;
            }

            FSM_Transition[] newArray = new FSM_Transition[_transitions.Length + 1];
            for (int i = 0; i < _transitions.Length; i++)
            {
                newArray[i] = _transitions[i];
            }
            newArray[newArray.Length - 1] = newTransition;
            _transitions = newArray;
        }

        public void ChangeTransitionIndex(int oldIndex, int newIndex)
        {
            if (newIndex < 0 || newIndex >= _transitions.Length)
                return;

            FSM_Transition temp = _transitions[oldIndex];
            int currentIndex = oldIndex;

            while (currentIndex != newIndex)
            {
                int nextIndex = oldIndex > newIndex ? (currentIndex - 1) : (currentIndex + 1);
                _transitions[currentIndex] = _transitions[nextIndex];
                currentIndex = nextIndex;
            }

            _transitions[newIndex] = temp;
        }

        public void OnStateDeleted(int deletedStateIndex)
        {
            if (_transitions == null)
                return;

            FSM_Transition[] newTransitionArray = _transitions.
                Where(transition => transition.toStateIndex != deletedStateIndex).ToArray();
            _transitions = newTransitionArray;
            foreach (FSM_Transition t in _transitions)
            {
                if (t.toStateIndex > deletedStateIndex)
                {
                    t.toStateIndex--;
                }
            }
        }
#endif
        #endregion

        public void CreateBehaviourInstance()
        {
            if (_behaviour == null)
                return;
            _behaviourInstance = UnityEngine.Object.Instantiate(_behaviour);
        }

        public int CheckTransitions(FSM_Manager manager)
        {
            foreach (FSM_Transition transition in _transitions)
            {
                if (transition.CheckConditions(manager))
                {
                    return transition.toStateIndex;
                }
            }

            return -1;
        }
    }
}
