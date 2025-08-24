using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GRD.FSM
{
    [CreateAssetMenu(fileName = "FSM Data",
        menuName = "GRD FSM/Create FSM Data",
        order = 0)]
    public class FSM_DataObject : ScriptableObject
    {
        [SerializeField] List<FSM_State> _states;
        [SerializeField] List<FSM_Parameter> _parameters;
        [SerializeField] int _defaultState;
        [SerializeField] FSM_Transition[] _anyStateTransitions;

        public List<FSM_State> states => _states;
        public List<FSM_Parameter> parameters => _parameters;
        public int defaultState => _defaultState;
        public FSM_Transition[] anyStateTransitions => _anyStateTransitions;

        #region Any State Editor
#if UNITY_EDITOR
        [SerializeField] private Rect anyStateEditorRect = new Rect(300, 0, 200, 50);
        [SerializeField] private bool anyStateIsDragged;
        [SerializeField] private bool anyStateIsSelected;

        public void DragAnyStateBox(Vector2 delta)
        {
            anyStateEditorRect.position += delta;
        }

        public void AddAnyStateTransition(int endStateIndex)
        {
            FSM_Transition newTransition = new FSM_Transition(endStateIndex);
            if (_anyStateTransitions == null)
            {
                _anyStateTransitions = new FSM_Transition[] { newTransition };
                return;
            }

            FSM_Transition[] newArray = new FSM_Transition[_anyStateTransitions.Length + 1];
            for (int i = 0; i < _anyStateTransitions.Length; i++)
            {
                newArray[i] = _anyStateTransitions[i];
            }
            newArray[newArray.Length - 1] = newTransition;
            _anyStateTransitions = newArray;
        }

        public void ChangeAnyStateTransitionIndex(int oldIndex, int newIndex)
        {
            if (newIndex < 0 || newIndex >= _anyStateTransitions.Length)
                return;

            FSM_Transition temp = _anyStateTransitions[oldIndex];
            int currentIndex = oldIndex;

            while (currentIndex != newIndex)
            {
                int nextIndex = oldIndex > newIndex ? (currentIndex - 1) : (currentIndex + 1);
                _anyStateTransitions[currentIndex] = _anyStateTransitions[nextIndex];
                currentIndex = nextIndex;
            }

            _anyStateTransitions[newIndex] = temp;
        }

        public void OnStateDeleted(int deletedStateIndex)
        {
            if (_anyStateTransitions == null)
                return;

            FSM_Transition[] newTransitionArray = _anyStateTransitions.
                Where(transition => transition.toStateIndex != deletedStateIndex).ToArray();
            _anyStateTransitions = newTransitionArray;
            foreach (FSM_Transition t in _anyStateTransitions)
            {
                if (t.toStateIndex > deletedStateIndex)
                {
                    t.toStateIndex--;
                }
            }
        }
#endif
        #endregion

        #region Get and Set Parameters Editor
#if UNITY_EDITOR
        public void SetParameterIndex(int oldIndex, int newIndex)
        {
            if (newIndex < 0 || newIndex >= _parameters.Count)
                return;

            FSM_Parameter temp = _parameters[oldIndex];
            int currentIndex = oldIndex;

            while (currentIndex != newIndex)
            {
                int nextIndex = oldIndex > newIndex ? (currentIndex - 1) : (currentIndex + 1);
                _parameters[currentIndex] = _parameters[nextIndex];
                currentIndex = nextIndex;
            }

            _parameters[newIndex] = temp;
            UpdateParameterIndexInTransitions(oldIndex, newIndex);
        }

        public void UpdateParameterIndexInTransitions(int oldIndex, int newIndex)
        {
            foreach (FSM_State state in _states)
            {
                if (state.transitions == null)
                    continue;

                foreach (FSM_Transition transition in state.transitions)
                {
                    transition.OnParameterIndexChange(oldIndex, newIndex);
                }
            }

            foreach (FSM_Transition anyStateTransition in _anyStateTransitions)
            {
                anyStateTransition.OnParameterIndexChange(oldIndex, newIndex);
            }
        }

        public void DeleteParameterAtIndex(int parameterIndex)
        {
            DeleteTransitionConditionsWithParameter(parameterIndex);
            _parameters.RemoveAt(parameterIndex);
        }

        public void DeleteTransitionConditionsWithParameter(int parameterIndex)
        {
            foreach (FSM_State state in _states)
            {
                if (state.transitions == null)
                    continue;

                foreach (FSM_Transition transition in state.transitions)
                {
                    transition.OnParameterDeleted(parameterIndex);
                }
            }

            foreach (FSM_Transition anyStateTransition in _anyStateTransitions)
            {
                anyStateTransition.OnParameterDeleted(parameterIndex);
            }
        }
#endif
        #endregion

        public void SetData(List<FSM_State> states,
            List<FSM_Parameter> parameters,
            int defaultState,
            FSM_Transition[] anyStateTransitions) 
        {
            _states = states;
            _parameters = parameters;
            _defaultState = defaultState;
            _anyStateTransitions = anyStateTransitions;
        }
    }
}
