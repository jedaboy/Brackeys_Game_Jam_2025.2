using UnityEngine;

namespace GRD.FSM
{
    [System.Serializable]
    public class FSM_Condition
    {
        [SerializeField] int _parameterIndex;

        public enum ConditionOperator
        {
            IsTrue,
            IsFalse,
            Equals,
            NotEqual,
            Less,
            Greater,
            LessOrEqual,
            GreaterOrEqual
        }

        public int parameterIndex
        {
            get => _parameterIndex;
#if UNITY_EDITOR
            set => _parameterIndex = value;
#endif
        }

#if UNITY_EDITOR
        public static readonly string[] booleanConditionLabels =
        {
        "Is true",
        "Is false"
    };

        public static readonly string[] numericConditionLabels =
        {
        "Equals to",
        "Not equal",
        "Less than",
        "Greater than",
        "Less or equal than",
        "Greater or equal than"
    };
#endif

        [SerializeField] ConditionOperator _conditionOperator;
        [SerializeField] float _referenceValue;

        public FSM_Condition(FSM_Parameter.ParameterType parameterType)
        {
            _parameterIndex = 0;
            _referenceValue = 0;
            switch (parameterType)
            {
                case FSM_Parameter.ParameterType.Float:
                case FSM_Parameter.ParameterType.Integer:
                    _conditionOperator = ConditionOperator.Equals;
                    break;
                case FSM_Parameter.ParameterType.Boolean:
                case FSM_Parameter.ParameterType.Trigger:
                    _conditionOperator = ConditionOperator.IsTrue;
                    break;
            }
        }

        public bool CheckCondition(FSM_Manager manager)
        {
            object parameterValue = manager.GetParameterValue(_parameterIndex);

            if (parameterValue == null)
                return false;

            switch (_conditionOperator)
            {
                case ConditionOperator.IsTrue:
                    return (bool)parameterValue;
                case ConditionOperator.IsFalse:
                    return !(bool)parameterValue;
                case ConditionOperator.Equals:
                    if (parameterValue is int)
                    {
                        return (int)parameterValue == _referenceValue;
                    }
                    else
                    {
                        return (float)parameterValue == _referenceValue;
                    }
                case ConditionOperator.NotEqual:
                    if (parameterValue is int)
                    {
                        return (int)parameterValue != _referenceValue;
                    }
                    else
                    {
                        return (float)parameterValue != _referenceValue;
                    }
                case ConditionOperator.Less:
                    if (parameterValue is int)
                    {
                        return (int)parameterValue < _referenceValue;
                    }
                    else
                    {
                        return (float)parameterValue < _referenceValue;
                    }
                case ConditionOperator.Greater:
                    if (parameterValue is int)
                    {
                        return (int)parameterValue > _referenceValue;
                    }
                    else
                    {
                        return (float)parameterValue > _referenceValue;
                    }
                case ConditionOperator.LessOrEqual:
                    if (parameterValue is int)
                    {
                        return (int)parameterValue <= _referenceValue;
                    }
                    else
                    {
                        return (float)parameterValue <= _referenceValue;
                    }
                case ConditionOperator.GreaterOrEqual:
                    if (parameterValue is int)
                    {
                        return (int)parameterValue >= _referenceValue;
                    }
                    else
                    {
                        return (float)parameterValue >= _referenceValue;
                    }
            }

            return false;
        }
    }
}
