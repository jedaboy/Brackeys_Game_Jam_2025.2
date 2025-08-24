using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace GRD.FSM
{
    [System.Serializable]
    public class FSM_ParametersSubWindow
    {
        [SerializeField] private SerializedObject _serializedObject;

        [SerializeField] private Rect _windowRect;

        [SerializeField] private Rect _resizerRect = new Rect();
        private const int _resizerBorderSize = 20;

        [SerializeField] Vector2 _parametersScrollPos;

        string _searchNameString;
        const string _newParameterDefaultName = "New Parameter";

        GUIStyle _parameterIndexStyle;

        Texture2D _searchIcon;
        Texture2D _addIcon;
        Texture2D _removeIcon;

        public FSM_ParametersSubWindow(Rect windowArea)
        {
            _windowRect = windowArea;
            _resizerRect = new Rect(_windowRect.width - _resizerBorderSize, 0, _resizerBorderSize, _windowRect.height);
        }

        #region Draw
        public void DefineStyles()
        {
            _searchIcon = AssetDatabase.LoadAssetAtPath("Assets/GRD_FSM/Editor/Icons/Search.png", typeof(Texture2D)) as Texture2D;
            _addIcon = AssetDatabase.LoadAssetAtPath("Assets/GRD_FSM/Editor/Icons/Add.png", typeof(Texture2D)) as Texture2D;
            _removeIcon = AssetDatabase.LoadAssetAtPath("Assets/GRD_FSM/Editor/Icons/Cancel.png", typeof(Texture2D)) as Texture2D;

            _parameterIndexStyle = new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleCenter };
        }

        public void Draw(Rect mainWindowRect, SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;

            if (_searchIcon == null)
            {
                DefineStyles();
            }

            _windowRect.size = new Vector2(_windowRect.width + _windowRect.position.x, mainWindowRect.height);
            _windowRect.position = new Vector2(0, 0);

            _windowRect = GUILayout.Window(2, _windowRect, DrawWindow, "Parameters");

            _serializedObject.ApplyModifiedProperties();
            _serializedObject.Update();
        }

        private void DrawWindow(int unusedWindowID)
        {
            GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            GUILayout.BeginHorizontal();
            GUIContent searchIconGUIContent = new GUIContent(_searchIcon);
            EditorGUIUtility.labelWidth = 20;
            _searchNameString = EditorGUILayout.TextField(new GUIContent(_searchIcon), _searchNameString);
            EditorGUIUtility.labelWidth = 0;
            if (GUILayout.Button(_addIcon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
            {
                CreateParameterContextMenu();
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            DrawParameterList();
            GUILayout.EndVertical();

            _resizerRect.position = new Vector2(_windowRect.width - _resizerBorderSize, 0);
            _resizerRect.size = new Vector2(_resizerBorderSize, _windowRect.height);
            GUI.DragWindow(_resizerRect);
        }

        private void DrawParameterList()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Button("ID", GUILayout.MaxWidth(40));
            GUILayout.Button("Name", GUILayout.ExpandWidth(true));
            GUILayout.Button("Value", GUILayout.MaxWidth(80));
            GUILayout.EndHorizontal();

            SerializedProperty serializedParameterList = _serializedObject.FindProperty("_parameters");
            int parameterCount = serializedParameterList.arraySize;

            _parametersScrollPos = GUILayout.BeginScrollView(_parametersScrollPos);
            for (int i = 0; i < parameterCount; i++)
            {
                SerializedProperty serializedParameter = serializedParameterList.GetArrayElementAtIndex(i);
                if (_searchNameString == null || _searchNameString == "" ||
                    serializedParameter.FindPropertyRelative("_name").stringValue.ToLower()
                    .Contains(_searchNameString.ToLower()))
                {
                    DrawParameterBar(serializedParameter, i);
                }
            }
            GUILayout.EndScrollView();
        }

        private void DrawParameterBar(SerializedProperty serializedParameter, int parameterIndex)
        {
            Rect parameterRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(30));

            EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(parameterRect, new Color(0.45f, 0.45f, 0.45f, 1));
            GUILayout.FlexibleSpace();


            EditorGUILayout.BeginHorizontal();
            int newIndex = EditorGUILayout.DelayedIntField(parameterIndex, _parameterIndexStyle, GUILayout.Width(40));
            if (parameterIndex != newIndex)
            {
                SerializedProperty serializedParameterList = _serializedObject.FindProperty("_parameters");
                int parameterCount = serializedParameterList.arraySize;
                if (newIndex < parameterCount)
                {
                    //Change parameter index
                    ((FSM_DataObject)_serializedObject.targetObject).SetParameterIndex(parameterIndex, newIndex);
                }
            }

            DrawParameterType(serializedParameter);
            string newName = EditorGUILayout.TextField(serializedParameter.FindPropertyRelative("_name").stringValue, GUILayout.ExpandWidth(true));
            serializedParameter.FindPropertyRelative("_name").stringValue = DefineNewOriginalParameterName(newName, 0, parameterIndex);

            DrawParameterValue(serializedParameter);

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button(_removeIcon, GUILayout.Width(20), GUILayout.Height(20)))
            {
                OnClickDeleteParameter(parameterIndex);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawParameterType(SerializedProperty serializedParameter)
        {
            FSM_Parameter.ParameterType parameterType = (FSM_Parameter.ParameterType)serializedParameter.FindPropertyRelative("_parameterType").enumValueIndex;

            switch (parameterType)
            {
                case FSM_Parameter.ParameterType.Boolean:
                    EditorGUILayout.LabelField("B", GUILayout.MaxWidth(10));
                    break;
                case FSM_Parameter.ParameterType.Trigger:
                    EditorGUILayout.LabelField("T", GUILayout.MaxWidth(10));
                    break;
                case FSM_Parameter.ParameterType.Integer:
                    EditorGUILayout.LabelField("I", GUILayout.MaxWidth(10));
                    break;
                case FSM_Parameter.ParameterType.Float:
                    EditorGUILayout.LabelField("F", GUILayout.MaxWidth(10));
                    break;
            }
        }

        private void DrawParameterValue(SerializedProperty serializedParameter)
        {
            FSM_Parameter.ParameterType parameterType = (FSM_Parameter.ParameterType)serializedParameter.FindPropertyRelative("_parameterType").enumValueIndex;

            switch (parameterType)
            {
                case FSM_Parameter.ParameterType.Boolean:
                case FSM_Parameter.ParameterType.Trigger:
                    GUILayout.Space(20);
                    serializedParameter.FindPropertyRelative("_boolValue").boolValue = EditorGUILayout.Toggle(serializedParameter.FindPropertyRelative("_boolValue").boolValue, GUILayout.Width(30));
                    GUILayout.Space(10);
                    break;
                case FSM_Parameter.ParameterType.Integer:
                    serializedParameter.FindPropertyRelative("_intValue").intValue = EditorGUILayout.IntField(serializedParameter.FindPropertyRelative("_intValue").intValue, _parameterIndexStyle, GUILayout.Width(60));
                    break;
                case FSM_Parameter.ParameterType.Float:
                    serializedParameter.FindPropertyRelative("_floatValue").floatValue = EditorGUILayout.FloatField(serializedParameter.FindPropertyRelative("_floatValue").floatValue, _parameterIndexStyle, GUILayout.Width(60));
                    break;
            }
        }
        #endregion

        #region Create Parameter
        private void CreateParameterContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Boolean"), false, CreateParameter, FSM_Parameter.ParameterType.Boolean);
            genericMenu.AddItem(new GUIContent("Integer"), false, CreateParameter, FSM_Parameter.ParameterType.Integer);
            genericMenu.AddItem(new GUIContent("Float"), false, CreateParameter, FSM_Parameter.ParameterType.Float);
            genericMenu.AddItem(new GUIContent("Trigger"), false, CreateParameter, FSM_Parameter.ParameterType.Trigger);
            genericMenu.ShowAsContext();
        }

        private void CreateParameter(object parameterType)
        {
            string newParameterName = DefineNewOriginalParameterName(_newParameterDefaultName);

            FSM_Parameter newParameter = new FSM_Parameter(newParameterName, (FSM_Parameter.ParameterType)parameterType);

            SerializedProperty parametersProperty = _serializedObject.FindProperty("_parameters");
            List<FSM_Parameter> parametersList = (List<FSM_Parameter>)parametersProperty.GetValue();
            parametersList.Add(newParameter);
        }

        private string DefineNewOriginalParameterName(string baseName, int currentTry = 0, int currentParameterIndex = -1)
        {
            string newParameterName = baseName;
            if (currentTry > 0)
            {
                newParameterName += " (" + currentTry + ")";
            }

            SerializedProperty serializedParameterList = _serializedObject.FindProperty("_parameters");
            int parameterCount = serializedParameterList.arraySize;

            for (int i = 0; i < parameterCount; i++)
            {
                if (currentParameterIndex == i)
                    continue;

                string usedParameterName = serializedParameterList.GetArrayElementAtIndex(i).FindPropertyRelative("_name").stringValue;

                if (usedParameterName == newParameterName)
                {
                    return DefineNewOriginalParameterName(baseName, currentTry + 1, currentParameterIndex);
                }
            }

            return newParameterName;
        }
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

        private void OnClickDeleteParameter(int parameterIndex)
        {
            ((FSM_DataObject)_serializedObject.targetObject).DeleteParameterAtIndex(parameterIndex);
        }
        #endregion
    }
}
