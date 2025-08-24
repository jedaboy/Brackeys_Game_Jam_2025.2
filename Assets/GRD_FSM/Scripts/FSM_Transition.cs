using System.Linq;
using UnityEngine;

namespace GRD.FSM
{
    [System.Serializable]
    public class FSM_Transition
    {
        [SerializeField] int _toStateIndex;
        [SerializeField] FSM_Condition[] _conditions;


        public int toStateIndex
        {
            get => _toStateIndex;
#if UNITY_EDITOR
            set => _toStateIndex = value;
#endif
        }

        public FSM_Transition(int endStateIndex)
        {
            _toStateIndex = endStateIndex;
        }

        #region Editor
#if UNITY_EDITOR
        public void AddCondition(FSM_Condition newCondition)
        {
            if (_conditions == null)
            {
                _conditions = new FSM_Condition[] { newCondition };
                return;
            }

            FSM_Condition[] newConditionsArray = new FSM_Condition[_conditions.Length + 1];
            for (int i = 0; i < _conditions.Length; i++)
            {
                newConditionsArray[i] = _conditions[i];
            }
            newConditionsArray[_conditions.Length] = newCondition;
            _conditions = newConditionsArray;
        }

        public void OnParameterIndexChange(int oldIndex, int newIndex)
        {
            if (_conditions == null)
                return;

            foreach (FSM_Condition condition in _conditions)
            {
                if (condition.parameterIndex == oldIndex)
                {
                    condition.parameterIndex = newIndex;
                    continue;
                }

                if (condition.parameterIndex > oldIndex &&
                    condition.parameterIndex <= newIndex)
                {
                    condition.parameterIndex--;
                    continue;
                }

                if (condition.parameterIndex < oldIndex &&
                    condition.parameterIndex >= newIndex)
                {
                    condition.parameterIndex++;
                    continue;
                }
            }
        }

        public void OnParameterDeleted(int parameterIndex)
        {
            if (_conditions == null)
                return;

            FSM_Condition[] newConditionArray = _conditions.
                Where(condition => condition.parameterIndex != parameterIndex).ToArray();
            _conditions = newConditionArray;
            foreach (FSM_Condition c in _conditions)
            {
                if (c.parameterIndex > parameterIndex)
                {
                    c.parameterIndex--;
                }
            }
        }
#endif
        #endregion

        public bool CheckConditions(FSM_Manager manager)
        {
            foreach (FSM_Condition condition in _conditions)
            {
                if (!condition.CheckCondition(manager))
                    return false;
            }

            foreach (FSM_Condition condition in _conditions)
            {
                if (manager.GetParameterType(condition.parameterIndex) == FSM_Parameter.ParameterType.Trigger)
                {
                    manager.ResetTrigger(condition.parameterIndex);
                }
            }

            return true;
        }
    }
}
