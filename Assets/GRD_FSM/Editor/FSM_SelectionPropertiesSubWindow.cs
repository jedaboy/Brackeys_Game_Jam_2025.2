using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace GRD.FSM
{
    [Serializable]
    public class FSM_SelectionPropertiesSubWindow
    {
        private int _selectedStateIndex = -1;
        private int _selectedTransitionIndex = -1;
        [SerializeField] private SerializedObject _serializedObject;

        [SerializeField] private Rect _windowRect;

        [SerializeField] private Rect _resizerRect = new Rect();
        private const int _resizerBorderSize = 20;

        [SerializeField] Vector2 _windowScrollPos;
        [SerializeField] Vector2 _transitionsScrollPos;
        [SerializeField] Vector2 _conditionsScrollPos;

        Texture2D _upArrowIconTex;
        Texture2D _downArrowIconTex;
        Texture2D _cancelIconTex;

        Color _transitionButtonColor = new Color(0.9f, 0.9f, 0.9f, 1);
        Color _selectedTransitionButtonColor = new Color(0.5f, 0.9f, 0.5f, 1);

        public FSM_SelectionPropertiesSubWindow(Rect windowArea)
        {
            _windowRect = windowArea;
            _resizerRect = new Rect(0, 0, _resizerBorderSize, _windowRect.height);
        }

        #region Draw
        public void DefineStyles()
        {
            _upArrowIconTex = AssetDatabase.LoadAssetAtPath("Assets/GRD_FSM/Editor/Icons/Up_Arrow.png", typeof(Texture2D)) as Texture2D;
            _downArrowIconTex = AssetDatabase.LoadAssetAtPath("Assets/GRD_FSM/Editor/Icons/Down_Arrow.png", typeof(Texture2D)) as Texture2D;
            _cancelIconTex = AssetDatabase.LoadAssetAtPath("Assets/GRD_FSM/Editor/Icons/Cancel.png", typeof(Texture2D)) as Texture2D;
        }

        public void Draw(Rect mainWindowRect, SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;

            if (_upArrowIconTex == null)
            {
                DefineStyles();
            }

            _windowRect.position = new Vector2(_windowRect.position.x, 0);
            _windowRect.size = new Vector2(mainWindowRect.width - _windowRect.position.x, mainWindowRect.height);

            _windowRect = GUILayout.Window(1, _windowRect, DrawWindow, "State Properties");

            _serializedObject.ApplyModifiedProperties();
            _serializedObject.Update();
        }

        void DrawWindow(int unusedWindowID)
        {
            _windowScrollPos = GUILayout.BeginScrollView(_windowScrollPos);
            GUILayout.BeginVertical(GUILayout.ExpandHeight(true));

            if (_selectedStateIndex == FSM_NodeEditorWindow._anyStateBoxIndex)
            {
                DrawAnyStateProperties();
            }
            else if (_selectedStateIndex >= 0)
            {
                DrawStateProperties();
            }
            else
            {
                DrawNoStateSelectedWindow();
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            _resizerRect.size = new Vector2(_resizerBorderSize, _windowRect.height);
            GUI.DragWindow(_resizerRect);
        }

        #region Draw State Properties
        private void DrawStateProperties()
        {
            SerializedProperty statesArrayProp = _serializedObject.FindProperty("_states");

            int arraySize = statesArrayProp.arraySize;
            if (_selectedStateIndex >= arraySize)
            {
                _selectedStateIndex = -1;
                return;
            }

            SerializedProperty stateProp = statesArrayProp.GetArrayElementAtIndex(_selectedStateIndex);
            EditorGUILayout.PropertyField(stateProp.FindPropertyRelative("_name"));
            EditorGUILayout.PropertyField(stateProp.FindPropertyRelative("_behaviour"));

            SerializedProperty serializedTransitions = stateProp.FindPropertyRelative("_transitions");
            DrawStateTransitionListBox(serializedTransitions, stateProp, statesArrayProp);

            if (_selectedTransitionIndex >= 0)
            {
                DrawTransitionConditions(serializedTransitions.GetArrayElementAtIndex(_selectedTransitionIndex));
            }
        }

        private void DrawStateTransitionListBox(SerializedProperty serializedTransitions, SerializedProperty serializedCurrentState, SerializedProperty serializedStatesArray)
        {
            EditorGUILayout.BeginHorizontal();

            Rect transitionListRect = EditorGUILayout.BeginVertical(GUILayout.Height(300), GUILayout.MinHeight(100));
            EditorGUI.DrawRect(transitionListRect, new Color(0.45f, 0.45f, 0.45f, 1));
            GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1);
            _transitionsScrollPos = EditorGUILayout.BeginScrollView(_transitionsScrollPos);
            for (int i = 0; i < serializedTransitions.arraySize; i++)
            {
                SerializedProperty transitionProp = serializedTransitions.GetArrayElementAtIndex(i);
                int toStateIndex = transitionProp.FindPropertyRelative("_toStateIndex").intValue;
                SerializedProperty toStateProp = serializedStatesArray.GetArrayElementAtIndex(toStateIndex);

                string startStateName;
                if (serializedCurrentState != null)
                {
                    startStateName = serializedCurrentState.FindPropertyRelative("_name").stringValue;
                }
                else
                {
                    startStateName = "Any State";
                }
                string endStateName = toStateProp.FindPropertyRelative("_name").stringValue;

                if (_selectedTransitionIndex == i)
                {
                    GUI.backgroundColor = _selectedTransitionButtonColor;
                }
                else
                {
                    GUI.backgroundColor = _transitionButtonColor;
                }
                if (GUILayout.Button(startStateName + " ---> " + endStateName, "Box", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                {
                    SelectTransition(i);
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(30));
            GUI.backgroundColor = Color.white;
            if (GUILayout.Button(_upArrowIconTex, GUILayout.Width(30), GUILayout.Height(30)))
            {
                MoveTransitionIndex(-1, serializedTransitions.arraySize);
            }
            if (GUILayout.Button(_downArrowIconTex, GUILayout.Width(30), GUILayout.Height(30)))
            {
                MoveTransitionIndex(+1, serializedTransitions.arraySize);
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button(_cancelIconTex, GUILayout.Width(30), GUILayout.Height(30)))
            {
                DeleteSelectedTransition(serializedTransitions);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = Color.white;
        }

        private void DrawTransitionConditions(SerializedProperty serializedTransition)
        {
            SerializedProperty serializedConditionsArray = serializedTransition.FindPropertyRelative("_conditions");
            int conditionsArraySize = serializedConditionsArray.arraySize;

            EditorGUILayout.LabelField("Conditions");
            _conditionsScrollPos = EditorGUILayout.BeginScrollView(_conditionsScrollPos);
            for (int i = 0; i < conditionsArraySize; i++)
            {
                bool conditionDeleted;
                DrawConditionBar(serializedTransition, i, out conditionDeleted);
                if (conditionDeleted)
                    break;
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add Condition"))
            {
                AddCondition(serializedTransition);
            }
        }

        private void DrawConditionBar(SerializedProperty serializedTransition, int conditionIndex, out bool conditionDeleted)
        {
            conditionDeleted = false;

            SerializedProperty serializedConditionsArray = serializedTransition.FindPropertyRelative("_conditions");
            SerializedProperty serializedCondition = serializedConditionsArray.GetArrayElementAtIndex(conditionIndex);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("If", GUILayout.Width(30));

            SerializedProperty serializedParametersArray = _serializedObject.FindProperty("_parameters");
            int parameterArraySize = serializedParametersArray.arraySize;
            string[] parametersNameArray = new string[parameterArraySize];
            for (int i = 0; i < parameterArraySize; i++)
            {
                parametersNameArray[i] = serializedParametersArray.GetArrayElementAtIndex(i).FindPropertyRelative("_name").stringValue;
            }

            serializedCondition.FindPropertyRelative("_parameterIndex").intValue =
                EditorGUILayout.Popup(serializedCondition.FindPropertyRelative("_parameterIndex").intValue, parametersNameArray);

            int selectedParameterIndex = serializedCondition.FindPropertyRelative("_parameterIndex").intValue;
            FSM_Parameter.ParameterType parameterType = (FSM_Parameter.ParameterType)serializedParametersArray.GetArrayElementAtIndex(selectedParameterIndex).FindPropertyRelative("_parameterType").enumValueIndex;

            switch (parameterType)
            {
                case FSM_Parameter.ParameterType.Integer:
                case FSM_Parameter.ParameterType.Float:
                    serializedCondition.FindPropertyRelative("_conditionOperator").enumValueIndex = EditorGUILayout.Popup(serializedCondition.FindPropertyRelative("_conditionOperator").enumValueIndex - 2, FSM_Condition.numericConditionLabels) + 2;

                    if (serializedCondition.FindPropertyRelative("_conditionOperator").enumValueIndex < 2)
                    {
                        serializedCondition.FindPropertyRelative("_conditionOperator").enumValueIndex = 2;
                    }

                    serializedCondition.FindPropertyRelative("_referenceValue").floatValue = EditorGUILayout.FloatField(serializedCondition.FindPropertyRelative("_referenceValue").floatValue);
                    break;
                case FSM_Parameter.ParameterType.Boolean:
                    serializedCondition.FindPropertyRelative("_conditionOperator").enumValueIndex = EditorGUILayout.Popup(serializedCondition.FindPropertyRelative("_conditionOperator").enumValueIndex, FSM_Condition.booleanConditionLabels);

                    if (serializedCondition.FindPropertyRelative("_conditionOperator").enumValueIndex > 1)
                    {
                        serializedCondition.FindPropertyRelative("_conditionOperator").enumValueIndex = 0;
                    }
                    break;
                default:
                    serializedCondition.FindPropertyRelative("_conditionOperator").enumValueIndex = 0;
                    break;
            }

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button(_cancelIconTex, GUILayout.Width(20), GUILayout.Height(20)))
            {
                DeleteCondition(serializedTransition, conditionIndex);
                conditionDeleted = true;
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawNoStateSelectedWindow()
        {
            GUILayout.Label("No Data");
        }
        #endregion

        #region Draw Any State Properties
        private void DrawAnyStateProperties()
        {
            EditorGUILayout.LabelField("Any State Transitions");

            SerializedProperty serializedTransitions = _serializedObject.FindProperty("_anyStateTransitions");
            SerializedProperty statesArrayProp = _serializedObject.FindProperty("_states");

            DrawStateTransitionListBox(serializedTransitions, null, statesArrayProp);

            if (_selectedTransitionIndex >= 0)
            {
                DrawTransitionConditions(serializedTransitions.GetArrayElementAtIndex(_selectedTransitionIndex));
            }
        }
        #endregion

        #endregion

        #region Events
        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (_windowRect.Contains(e.mousePosition))
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }

        public void OnStateSelected(int stateIndex)
        {
            if (stateIndex != _selectedStateIndex)
            {
                _selectedStateIndex = stateIndex;
                _selectedTransitionIndex = -1;
            }
        }

        public void OnStateDeselected(int stateIndex = -1)
        {
            if (stateIndex == -1 || stateIndex == _selectedStateIndex)
            {
                GUI.FocusControl(null);
                _selectedStateIndex = -1;
                _selectedTransitionIndex = -1;
            }
        }

        private void SelectTransition(int transitionIndex)
        {
            _selectedTransitionIndex = transitionIndex;
        }
        #endregion

        #region Assign Behaviour
        private void AssignBehaviourContextMenu(SerializedProperty behaviourProperty)
        {
            GenericMenu genericMenu = new GenericMenu();

            List<FSM_StateBehaviour> behaviourTypes = GetAllBehaviourTypes().ToList();
            for (int i = 0; i < behaviourTypes.Count; i++)
            {

                FSM_BehaviourAttribute behaviourAttribute = (FSM_BehaviourAttribute)behaviourTypes[i].GetType().GetCustomAttributes(typeof(FSM_BehaviourAttribute), true).FirstOrDefault();
                if (behaviourAttribute != null)
                {
                    genericMenu.AddItem(new GUIContent(behaviourAttribute.Path), false, AssignBehaviour, new object[] { behaviourTypes[i], behaviourProperty });
                }
                else
                {
                    genericMenu.AddItem(new GUIContent(behaviourTypes[i].GetType().ToString()), false, AssignBehaviour, new object[] { behaviourTypes[i], behaviourProperty });
                }
            }
            genericMenu.ShowAsContext();
        }

        public static IEnumerable<FSM_StateBehaviour> GetAllBehaviourTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(FSM_StateBehaviour)))
                .Select(type => Activator.CreateInstance(type) as FSM_StateBehaviour);
        }

        private void AssignBehaviour(object data)
        {
            object[] dataArray = data as object[];
            SerializedProperty behaviourProperty = dataArray[1] as SerializedProperty;
            behaviourProperty.SetValue(dataArray[0] as FSM_StateBehaviour);
        }
        #endregion

        #region Transition Events
        private void MoveTransitionIndex(int indexIncrement, int transitionsCount)
        {
            if (_selectedTransitionIndex < 0 || (_selectedStateIndex < 0 && _selectedStateIndex != FSM_NodeEditorWindow._anyStateBoxIndex))
                return;
            if (_selectedTransitionIndex + indexIncrement < 0 ||
                _selectedTransitionIndex + indexIncrement >= transitionsCount)
                return;

            if (_selectedStateIndex >= 0)
            {
                //State Transition
                SerializedProperty serializedStateArray = _serializedObject.FindProperty("_states");
                SerializedProperty serializedState = serializedStateArray.GetArrayElementAtIndex(_selectedStateIndex);
                ((FSM_State)serializedState.GetValue()).ChangeTransitionIndex(_selectedTransitionIndex, _selectedTransitionIndex + indexIncrement);
            }
            else
            {
                //Any State Transition
                ((FSM_DataObject)_serializedObject.targetObject).ChangeAnyStateTransitionIndex(_selectedTransitionIndex, _selectedTransitionIndex + indexIncrement);
            }
            _selectedTransitionIndex += indexIncrement;
        }

        private void DeleteSelectedTransition(SerializedProperty serializedTransitionsArray)
        {
            if (_selectedTransitionIndex < 0 || (_selectedStateIndex < 0 && _selectedStateIndex != FSM_NodeEditorWindow._anyStateBoxIndex))
                return;

            serializedTransitionsArray.DeleteArrayElementAtIndex(_selectedTransitionIndex);
            if (_selectedTransitionIndex >= serializedTransitionsArray.arraySize)
            {
                _selectedTransitionIndex = serializedTransitionsArray.arraySize - 1;
            }
        }
        #endregion

        #region Condition Events
        private void AddCondition(SerializedProperty serializedTransition)
        {
            SerializedProperty serializedParametersArray = _serializedObject.FindProperty("_parameters");
            if (serializedParametersArray.arraySize == 0)
            {
                //No parameter defined
                EditorUtility.DisplayDialog(
                    "No parameter defined",
                    "You must define a parameter before creating a condition.",
                    "Ok");
                return;
            }

            SerializedProperty serializedFirstParameter = serializedParametersArray.GetArrayElementAtIndex(0);
            FSM_Parameter.ParameterType parameterType = (FSM_Parameter.ParameterType)serializedFirstParameter.FindPropertyRelative("_parameterType").enumValueIndex;

            FSM_Condition newCondition = new FSM_Condition(parameterType);

            FSM_Transition transitionObject = (FSM_Transition)serializedTransition.GetValue();
            transitionObject.AddCondition(newCondition);
        }

        private void DeleteCondition(SerializedProperty serializedTransition, int conditionIndex)
        {
            SerializedProperty serializedConditionsArray = serializedTransition.FindPropertyRelative("_conditions");
            serializedConditionsArray.DeleteArrayElementAtIndex(conditionIndex);

        }
        #endregion
    }
}
