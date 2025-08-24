using UnityEngine;
using UnityEditor;

namespace GRD.FSM
{
    [CustomEditor(typeof(FSM_DataObject), true)]
    public class FSM_DataObject_Inspector : Editor
    {
        private FSM_DataObject _myFSMData => target as FSM_DataObject;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open FSM Editor"))
            {
                OpenFSMEditorWindow();
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void OpenFSMEditorWindow()
        {
            FSM_NodeEditorWindow.OpenWindow(_myFSMData);
        }
    }
}
