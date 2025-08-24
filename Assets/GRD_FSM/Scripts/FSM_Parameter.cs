using UnityEngine;

namespace GRD.FSM
{
    [System.Serializable]
    public class FSM_Parameter
    {
        [SerializeField] string _name;

        public enum ParameterType
        {
            Boolean,
            Integer,
            Float,
            Trigger
        }

        [SerializeField] ParameterType _parameterType;

        [SerializeField] bool _boolValue;
        [SerializeField] int _intValue;
        [SerializeField] float _floatValue;

        public string name => _name;
        public ParameterType parameterType => _parameterType;

        public bool boolValue { get => _boolValue; set => _boolValue = value; }
        public int intValue { get => _intValue; set => _intValue = value; }
        public float floatValue { get => _floatValue; set => _floatValue = value; }

        public FSM_Parameter(string name, ParameterType parameterType)
        {
            _name = name;
            _parameterType = parameterType;
        }

        public object GetValue()
        {
            switch (_parameterType)
            {
                case ParameterType.Boolean:
                default:
                    return _boolValue;
                case ParameterType.Integer:
                    return _intValue;
                case ParameterType.Float:
                    return _floatValue;
            }
        }
    }
}
