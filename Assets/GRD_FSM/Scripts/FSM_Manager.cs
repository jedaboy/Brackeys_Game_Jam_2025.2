using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GRD.FSM
{
    public class FSM_Manager : MonoBehaviour
    {
        [SerializeField] string FSM_Name;
        [SerializeField] FSM_DataObject FSM_Data;
        private FSM_DataObject FSM_DataInstance;

        public FSM_DataObject DataInstance 
        {
            get 
            {
                if (FSM_DataInstance == null)
                    CreateDataInstance();

                return FSM_DataInstance;
            }
        }

        enum StartMethod
        {
            Awake,
            Start
        }
        [SerializeField] StartMethod _startFSMMethod;

        [System.Flags]
        enum TransitionMethod
        {
            Update = 1,
            FixedUpdate = 2,
            LateUpdate = 4
        }
        [SerializeField] TransitionMethod _transitionExecutionMethod = TransitionMethod.Update;

        [SerializeField] int _currentState = 0;

        public void CreateDataInstance()
        {
            if (FSM_Data == null)
                return;
            if (FSM_DataInstance != null)
                return;

            FSM_DataInstance = UnityEngine.Object.Instantiate(FSM_Data);
        }


        #region Get and Set Parameters
        public object GetParameterValue(int parameterIndex)
        {
            return DataInstance.parameters[parameterIndex].GetValue();
        }

        public object GetParameterValue(string parameterName)
        {
            foreach (FSM_Parameter p in DataInstance.parameters)
            {
                if (p.name == parameterName)
                    return p.GetValue();
            }

            return null;
        }

        public FSM_Parameter.ParameterType GetParameterType(int parameterIndex)
        {
            return DataInstance.parameters[parameterIndex].parameterType;
        }

        public void SetInt(int parameterIndex, int value)
        {
            FSM_Parameter parameter = DataInstance.parameters[parameterIndex];
            if (parameter.parameterType != FSM_Parameter.ParameterType.Integer)
            {
                Debug.LogError("FSM_Parameter " + parameter.name + " is not an Integer");
                return;
            }

            parameter.intValue = value;
        }

        public void SetInt(string parameterName, int value)
        {
            FSM_Parameter parameter = DataInstance.parameters.Where((p) => p.name == parameterName).FirstOrDefault();
            if (parameter == null)
            {
                Debug.LogError("Parameter " + parameterName + " not found.");
            }
            else
            {
                SetInt(DataInstance.parameters.IndexOf(parameter), value);
            }
        }

        public void SetFloat(int parameterIndex, float value)
        {
            FSM_Parameter parameter = DataInstance.parameters[parameterIndex];
            if (parameter.parameterType != FSM_Parameter.ParameterType.Float)
            {
                Debug.LogError("FSM_Parameter " + parameter.name + " is not a Float");
                return;
            }

            parameter.floatValue = value;
        }

        public void SetFloat(string parameterName, float value)
        {
            FSM_Parameter parameter = DataInstance.parameters.Where((p) => p.name == parameterName).FirstOrDefault();
            if (parameter == null)
            {
                Debug.LogError("Parameter " + parameterName + " not found.");
            }
            else
            {
                SetFloat(DataInstance.parameters.IndexOf(parameter), value);
            }
        }

        public void SetBool(int parameterIndex, bool value)
        {
            FSM_Parameter parameter = DataInstance.parameters[parameterIndex];
            if (parameter.parameterType != FSM_Parameter.ParameterType.Boolean)
            {
                Debug.LogError("FSM_Parameter " + parameter.name + " is not a Boolean");
                return;
            }

            parameter.boolValue = value;
        }

        public void SetBool(string parameterName, bool value)
        {
            FSM_Parameter parameter = DataInstance.parameters.Where((p) => p.name == parameterName).FirstOrDefault();
            if (parameter == null)
            {
                Debug.LogError("Parameter " + parameterName + " not found.");
            }
            else
            {
                SetBool(DataInstance.parameters.IndexOf(parameter), value);
            }
        }

        public void SetTrigger(int parameterIndex)
        {
            FSM_Parameter parameter = DataInstance.parameters[parameterIndex];
            if (parameter.parameterType != FSM_Parameter.ParameterType.Trigger)
            {
                Debug.LogError("FSM_Parameter " + parameter.name + " is not a Trigger");
                return;
            }

            parameter.boolValue = true;
        }

        public void SetTrigger(string parameterName)
        {
            FSM_Parameter parameter = DataInstance.parameters.Where((p) => p.name == parameterName).FirstOrDefault();
            if (parameter == null)
            {
                Debug.LogError("Parameter " + parameterName + " not found.");
            }
            else
            {
                SetTrigger(DataInstance.parameters.IndexOf(parameter));
            }
        }

        public void ResetTrigger(int parameterIndex)
        {
            FSM_Parameter parameter = DataInstance.parameters[parameterIndex];
            if (parameter.parameterType != FSM_Parameter.ParameterType.Trigger)
            {
                Debug.LogError("FSM_Parameter " + parameter.name + " is not a Trigger");
                return;
            }

            parameter.boolValue = false;
        }

        public void ResetTrigger(string parameterName)
        {
            FSM_Parameter parameter = DataInstance.parameters.Where((p) => p.name == parameterName).FirstOrDefault();
            if (parameter == null)
            {
                Debug.LogError("Parameter " + parameterName + " not found.");
            }
            else
            {
                ResetTrigger(DataInstance.parameters.IndexOf(parameter));
            }
        }

        public void ResetAllTriggers()
        {
            IEnumerable<FSM_Parameter> triggers = DataInstance.parameters.Where((parameter) => parameter.parameterType == FSM_Parameter.ParameterType.Trigger);

            foreach (FSM_Parameter trigger in triggers)
            {
                trigger.boolValue = false;
            }
        }
        #endregion

        #region Get State Info
        public int GetStateIdByName(string stateName)
        {
            if (!DataInstance.states.Exists(x => x.name == stateName))
            {
                Debug.LogError("FSM does not contain a state named as '" + stateName + "'");
                return -1;
            }
            return DataInstance.states.IndexOf(DataInstance.states.First(x => x.name == stateName));
        }

        public int GetCurrentStateId()
        {
            return _currentState;
        }

        public string GetCurrentStateName()
        {
            return DataInstance.states[_currentState].name;
        }
        #endregion

        #region Behaviour
        private void Awake()
        {
            CreateDataInstance();

            foreach (FSM_State state in DataInstance.states)
            {
                state.CreateBehaviourInstance();
                if (state.behaviour != null)
                {
                    state.behaviour.Setup(this);
                }
            }

            if (_startFSMMethod == StartMethod.Awake)
            {
                StartFSM();
            }
        }

        private void Start()
        {
            if (_startFSMMethod == StartMethod.Start)
            {
                StartFSM();
            }
        }

        private void StartFSM()
        {
            _currentState = DataInstance.defaultState;
            if (DataInstance.states[_currentState].behaviour != null)
            {
                DataInstance.states[_currentState].behaviour.OnEnter();
            }
        }

        private void Update()
        {
            if (DataInstance.states[_currentState].behaviour != null)
            {
                DataInstance.states[_currentState].behaviour.OnUpdate();
            }

            //Check Transitions
            if (_transitionExecutionMethod.HasFlag(TransitionMethod.Update))
            {
                CheckTransitions();
            }
        }

        private void FixedUpdate()
        {
            if (DataInstance.states[_currentState].behaviour != null)
            {
                DataInstance.states[_currentState].behaviour.OnFixedUpdate();
            }

            //Check Transitions
            if (_transitionExecutionMethod.HasFlag(TransitionMethod.FixedUpdate))
            {
                CheckTransitions();
            }
        }

        private void LateUpdate()
        {
            if (DataInstance.states[_currentState].behaviour != null)
            {
                DataInstance.states[_currentState].behaviour.OnLateUpdate();
            }

            //Check Transitions
            if (_transitionExecutionMethod.HasFlag(TransitionMethod.LateUpdate))
            {
                CheckTransitions();
            }

            //ResetAllTriggers();
        }

        private bool CheckTransitions()
        {
            int newStateIndex = CheckAnyStateTransitions();
            if (newStateIndex >= 0 && newStateIndex != _currentState)
            {
                ChangeState(newStateIndex);
                return true;
            }

            newStateIndex = DataInstance.states[_currentState].CheckTransitions(this);
            if (newStateIndex >= 0)
            {
                ChangeState(newStateIndex);
                return true;
            }

            return false;
        }

        private int CheckAnyStateTransitions()
        {
            foreach (FSM_Transition transition in DataInstance.anyStateTransitions)
            {
                if (transition.CheckConditions(this))
                {
                    return transition.toStateIndex;
                }
            }

            return -1;
        }

        private void ChangeState(int newStateIndex)
        {
            if (DataInstance.states[_currentState].behaviour != null)
            {
                DataInstance.states[_currentState].behaviour.OnExit();
            }

            _currentState = newStateIndex;

            if (DataInstance.states[_currentState].behaviour != null)
            {
                DataInstance.states[_currentState].behaviour.OnEnter();
            }
        }
        #endregion

        #region Collision Events
        private void OnCollisionEnter(Collision collision)
        {
            if (DataInstance.states[_currentState].behaviour != null)
            {
                DataInstance.states[_currentState].behaviour.OnCollisionEnter(collision);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (DataInstance.states[_currentState].behaviour != null)
            {
                DataInstance.states[_currentState].behaviour.OnCollisionStay(collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (DataInstance.states[_currentState].behaviour != null)
            {
                DataInstance.states[_currentState].behaviour.OnCollisionExit(collision);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (DataInstance.states[_currentState].behaviour != null)
            {
                DataInstance.states[_currentState].behaviour.OnCollisionEnter2D(collision);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (DataInstance.states[_currentState].behaviour != null)
            {
                DataInstance.states[_currentState].behaviour.OnCollisionStay2D(collision);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (DataInstance.states[_currentState].behaviour != null)
            {
                DataInstance.states[_currentState].behaviour.OnCollisionExit2D(collision);
            }
        }
        #endregion
    }
}
