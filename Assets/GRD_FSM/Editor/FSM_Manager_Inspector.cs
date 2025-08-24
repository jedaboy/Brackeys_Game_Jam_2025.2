using UnityEngine;
using UnityEditor;
using System.Drawing.Printing;

namespace GRD.FSM
{
    [CustomEditor(typeof(FSM_Manager), true)]
    public class FSM_Manager_Inspector : Editor
    {
        FSM_Manager _myFSMManager;

        private void OnEnable()
        {
            _myFSMManager = (FSM_Manager)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FSM_Name"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FSM_Data"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_startFSMMethod"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_transitionExecutionMethod"));

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Open FSM Debugger"))
                {
                    OpenFSMEditorWindow();
                }

                int currentStateIndex = _myFSMManager.GetCurrentStateId();
                SerializedProperty fsmDataProperty = serializedObject.FindProperty("FSM_Data");
                FSM_State state = ((FSM_DataObject)fsmDataProperty.objectReferenceValue).states[currentStateIndex];
                string currentStateName = state.name;
                EditorGUILayout.LabelField("Current State: " + currentStateName);
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void OpenFSMEditorWindow()
        {
            if (_myFSMManager.DataInstance == null) 
            {
                Debug.LogError("Data instance is null. This value is set on object Awake.");
                return;
            }
            FSM_NodeEditorWindow.OpenWindow(_myFSMManager.DataInstance);
        }
    }
}
