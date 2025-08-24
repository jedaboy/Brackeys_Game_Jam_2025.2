using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace GRD.FSM
{
    public class FSM_NodeEditorWindow : EditorWindow
    {
        [SerializeField] int _instanceID;
        [SerializeField] private FSM_DataObject _data;
        [SerializeField] private SerializedObject _serializedObject;

        private FSM_State _currentTransitionStartState = null;
        private bool _creatingAnyStateTransition = false;

        private FSM_SelectionPropertiesSubWindow _selectionPropertiesSubWindow;
        private Rect _defaultSelectionPropertiesSubWindowArea = new Rect(0f, 0f, 400f, 0f);

        private FSM_ParametersSubWindow _parametersSubWindow;
        private Rect _defaultParametersSubWindowArea = new Rect(0f, 0f, 300f, 0f);

        public const int _anyStateBoxIndex = -2;

        private GUIStyle _noDataTextStyle;
        private GUIStyle _stateBoxStyle;
        private GUIStyle _anyStateBoxStyle;
        private GUIStyle _defaultStateBoxStyle;
        private GUIStyle _selectedStateBoxStyle;
        private GUIStyle _runningStateBoxStyle;

        private Vector2 offset;
        private Vector2 drag;

        public static void OpenWindow(FSM_DataObject data)
        {
            FSM_NodeEditorWindow window = GetWindow<FSM_NodeEditorWindow>();
            window.titleContent = new GUIContent("FSM_Editor");

            window._instanceID = data.GetInstanceID();
            window._data = data;
            window._serializedObject = new SerializedObject(data);

            window._defaultSelectionPropertiesSubWindowArea.position = new Vector2(window.position.width * 0.8f, 0);
            window._defaultSelectionPropertiesSubWindowArea.width = window.position.width * 0.2f;
            window._selectionPropertiesSubWindow = new FSM_SelectionPropertiesSubWindow(window._defaultSelectionPropertiesSubWindowArea);

            window._parametersSubWindow = new FSM_ParametersSubWindow(window._defaultParametersSubWindowArea);
        }

        private void OnEnable()
        {
            _noDataTextStyle = new GUIStyle();
            _noDataTextStyle.alignment = TextAnchor.MiddleCenter;

            _stateBoxStyle = new GUIStyle();
            _stateBoxStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D;
            _stateBoxStyle.border = new RectOffset(12, 12, 12, 12);
            _stateBoxStyle.alignment = TextAnchor.MiddleCenter;
            _stateBoxStyle.normal.textColor = Color.white;

            _anyStateBoxStyle = new GUIStyle();
            _anyStateBoxStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
            _anyStateBoxStyle.border = new RectOffset(12, 12, 12, 12);
            _anyStateBoxStyle.alignment = TextAnchor.MiddleCenter;
            _anyStateBoxStyle.normal.textColor = Color.white;

            _defaultStateBoxStyle = new GUIStyle();
            _defaultStateBoxStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            _defaultStateBoxStyle.border = new RectOffset(12, 12, 12, 12);
            _defaultStateBoxStyle.alignment = TextAnchor.MiddleCenter;
            _defaultStateBoxStyle.normal.textColor = Color.white;

            _selectedStateBoxStyle = new GUIStyle();
            _selectedStateBoxStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node5.png") as Texture2D;
            _selectedStateBoxStyle.border = new RectOffset(12, 12, 12, 12);
            _selectedStateBoxStyle.alignment = TextAnchor.MiddleCenter;
            _selectedStateBoxStyle.normal.textColor = Color.white;

            _runningStateBoxStyle = new GUIStyle();
            _runningStateBoxStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node4.png") as Texture2D;
            _runningStateBoxStyle.border = new RectOffset(12, 12, 12, 12);
            _runningStateBoxStyle.alignment = TextAnchor.MiddleCenter;
            _runningStateBoxStyle.normal.textColor = Color.white;

            if (_selectionPropertiesSubWindow != null)
                _selectionPropertiesSubWindow.DefineStyles();
        }

        private void OnGUI()
        {
            if (_data == null || _serializedObject == null)
            {
                if (!TryToGetSerializedObject())
                {
                    DrawNoSelectedObjectMessage();
                    return;
                }
            }

            if (_selectionPropertiesSubWindow == null)
            {
                _defaultSelectionPropertiesSubWindowArea.position = new Vector2(position.width - _defaultSelectionPropertiesSubWindowArea.width, 0);

                _selectionPropertiesSubWindow = new FSM_SelectionPropertiesSubWindow(_defaultSelectionPropertiesSubWindowArea);
            }

            if (_parametersSubWindow == null)
            {
                _parametersSubWindow = new FSM_ParametersSubWindow(_defaultParametersSubWindowArea);
            }

            offset += drag;
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            DrawAnyStateTransitions();
            DrawTransitions();
            DrawCurrentTransition(Event.current);
            DrawAnyStateBox();
            DrawStates();

            BeginWindows();
            _parametersSubWindow.Draw(position, _serializedObject);
            _selectionPropertiesSubWindow.Draw(position, _serializedObject);
            EndWindows();

            ProcessEvents();

            _serializedObject.ApplyModifiedProperties();
            _serializedObject.Update();

            if (GUI.changed || EditorApplication.isPlaying) Repaint();
        }

        void ProcessEvents()
        {
            if (ProcessSelectionPropSubWindowEvents(Event.current))
                return;

            if (ProcessParametersSubWindowEvents(Event.current))
                return;

            ProcessStateBoxEvents(Event.current);
            ProcessNodeAreaEvents(Event.current);
        }

        #region Node Area
        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing) + 1;
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing) + 1;

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height + gridSpacing, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width + gridSpacing, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void ProcessNodeAreaEvents(Event e)
        {
            drag = Vector2.zero;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                    {
                        if (_currentTransitionStartState != null || _creatingAnyStateTransition)
                        {
                            CancelTransitionCreation();
                        }
                        else
                        {
                            ProcessContextMenu(e.mousePosition);
                        }
                    }
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        OnDrag(e.delta);
                    }
                    break;
            }
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add State"), false, () => OnClickAddState(mousePosition));
            genericMenu.ShowAsContext();
        }

        private void OnClickAddState(Vector2 mousePosition)
        {
            FSM_State newState = new FSM_State("New State", new Rect(mousePosition, new Vector2(200, 50)));

            SerializedProperty statesProperty = _serializedObject.FindProperty("_states");
            List<FSM_State> stateList = (List<FSM_State>)statesProperty.GetValue();
            stateList.Add(newState);
        }

        private void OnDrag(Vector2 delta)
        {
            drag = delta;

            SerializedProperty statesProperty = _serializedObject.FindProperty("_states");
            List<FSM_State> stateList = (List<FSM_State>)statesProperty.GetValue();
            foreach (FSM_State state in stateList)
            {
                state.DragBox(delta);
            }
            ((FSM_DataObject)_serializedObject.targetObject).DragAnyStateBox(delta);

            GUI.changed = true;
        }
        #endregion

        #region No Data Found
        private void DrawNoSelectedObjectMessage()
        {
            GUILayout.Label("No Object Selected.", _noDataTextStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }

        private bool TryToGetSerializedObject()
        {
            if (_data == null)
            {
                _data = EditorUtility.InstanceIDToObject(_instanceID) as FSM_DataObject;
                if (_data == null)
                    return false;
            }

            _serializedObject = new SerializedObject(_data);
            if (_serializedObject == null)
                return false;
            return true;
        }
        #endregion

        #region State Box
        private void DrawStates()
        {
            SerializedProperty statesProperty = _serializedObject.FindProperty("_states");
            //int currentRunningStateIndex = _serializedObject.FindProperty("_currentState").intValue;
            int defaultStateIndex = _serializedObject.FindProperty("_defaultState").intValue;
            List<FSM_State> stateList = (List<FSM_State>)statesProperty.GetValue();
            for (int i = 0; i < stateList.Count; i++)
            {
                GUIStyle unselectedStyle =
                    defaultStateIndex == i ?
                    _defaultStateBoxStyle : _stateBoxStyle;
                FSM_StateBox.Draw(stateList[i], unselectedStyle, _selectedStateBoxStyle);
            }
        }

        private void ProcessStateBoxEvents(Event e)
        {
            SerializedProperty statesProperty = _serializedObject.FindProperty("_states");
            List<FSM_State> stateList = (List<FSM_State>)statesProperty.GetValue();
            bool alreadyCastBox = false;
            for (int i = stateList.Count - 1; i >= 0; i--)
            {
                alreadyCastBox = FSM_StateBox.ProcessEvents(stateList[i], i, this, e, alreadyCastBox,
                    OnStateSelected,
                    _selectionPropertiesSubWindow.OnStateDeselected) || alreadyCastBox;

                if (alreadyCastBox)
                {
                    GUI.changed = true;
                }
            }
            ProcessAnyStateEvents(Event.current, alreadyCastBox);
        }

        public void OnStateSelected(int stateIndex)
        {
            if (stateIndex == _anyStateBoxIndex)
            {
                CancelTransitionCreation();
            }

            if (_creatingAnyStateTransition)
            {
                CreateNewAnyStateTransition(stateIndex);
                CancelTransitionCreation();
            }
            else if (_currentTransitionStartState != null)
            {
                CreateNewTransition(_currentTransitionStartState, stateIndex);
                CancelTransitionCreation();
            }

            _selectionPropertiesSubWindow.OnStateSelected(stateIndex);
        }

        public void OnClickDeleteState(FSM_State state)
        {
            SerializedProperty statesProperty = _serializedObject.FindProperty("_states");
            List<FSM_State> stateList = (List<FSM_State>)statesProperty.GetValue();
            int deletedStateIndex = stateList.IndexOf(state);
            stateList.Remove(state);
            foreach (FSM_State st in stateList)
            {
                st.OnStateDeleted(deletedStateIndex);
            }
            _selectionPropertiesSubWindow.OnStateDeselected();
            ((FSM_DataObject)_serializedObject.targetObject).OnStateDeleted(deletedStateIndex);
            _serializedObject.ApplyModifiedProperties();
            _serializedObject.Update();

            int defaultStateIndex = _serializedObject.FindProperty("_defaultState").intValue;
            if (defaultStateIndex >= stateList.Count)
            {
                _serializedObject.FindProperty("_defaultState").intValue = Mathf.Max(stateList.Count - 1, 0);
            }
            else if (defaultStateIndex > deletedStateIndex)
            {
                _serializedObject.FindProperty("_defaultState").intValue--;
            }
        }

        public void OnClickCreateTransition(FSM_State state)
        {
            _currentTransitionStartState = state;
            _creatingAnyStateTransition = false;
        }

        public void OnClickSetDefaultState(FSM_State state)
        {
            List<FSM_State> stateList = (List<FSM_State>)_serializedObject.FindProperty("_states").GetValue();
            int stateIndex = stateList.IndexOf(state);
            _serializedObject.FindProperty("_defaultState").intValue = stateIndex;
        }

        private void CreateNewTransition(FSM_State startState, int endStateIndex)
        {
            startState.AddTransition(endStateIndex);
        }

        private void CancelTransitionCreation()
        {
            _currentTransitionStartState = null;
            _creatingAnyStateTransition = false;
        }
        #endregion

        #region Any State Box
        private void DrawAnyStateBox()
        {
            Rect anyStateRect = _serializedObject.FindProperty("anyStateEditorRect").rectValue;
            bool anyStateSelected = _serializedObject.FindProperty("anyStateIsSelected").boolValue;
            GUIStyle style = anyStateSelected ? _selectedStateBoxStyle : _anyStateBoxStyle;
            GUI.Box(anyStateRect, "Any State", style);
        }

        private bool ProcessAnyStateEvents(Event e, bool alreadyCastBox)
        {
            Rect anyStateRect = _serializedObject.FindProperty("anyStateEditorRect").rectValue;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (anyStateRect.Contains(e.mousePosition) && !alreadyCastBox)
                        {
                            _serializedObject.FindProperty("anyStateIsDragged").boolValue = true;
                            GUI.changed = true;
                            _serializedObject.FindProperty("anyStateIsSelected").boolValue = true;
                            OnStateSelected(_anyStateBoxIndex);
                            return true;
                        }
                        else
                        {
                            GUI.changed = true;
                            _serializedObject.FindProperty("anyStateIsSelected").boolValue = false;
                            _selectionPropertiesSubWindow.OnStateDeselected(_anyStateBoxIndex);
                        }
                    }

                    if (e.button == 1 && anyStateRect.Contains(e.mousePosition))
                    {
                        ProcessAnyStateContextMenu();
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    _serializedObject.FindProperty("anyStateIsDragged").boolValue = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && _serializedObject.FindProperty("anyStateIsDragged").boolValue)
                    {
                        ((FSM_DataObject)_serializedObject.targetObject).DragAnyStateBox(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }

        private void ProcessAnyStateContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Create Transition"), false, OnClickCreateAnyStateTransition);
            genericMenu.ShowAsContext();
        }

        private void OnClickCreateAnyStateTransition()
        {
            _currentTransitionStartState = null;
            _creatingAnyStateTransition = true;
        }

        private void CreateNewAnyStateTransition(int endStateIndex)
        {
            ((FSM_DataObject)_serializedObject.targetObject).AddAnyStateTransition(endStateIndex);
            _serializedObject.ApplyModifiedProperties();
            _serializedObject.Update();
        }
        #endregion

        #region Transitions
        private void DrawTransitions()
        {
            SerializedProperty statesProperty = _serializedObject.FindProperty("_states");
            List<FSM_State> stateList = (List<FSM_State>)statesProperty.GetValue();
            foreach (FSM_State state in stateList)
            {
                if (state.transitions == null)
                    continue;

                foreach (FSM_Transition transition in state.transitions)
                {
                    FSM_StateBox.DrawTransition(state, stateList[transition.toStateIndex]);
                }
            }
        }

        private void DrawAnyStateTransitions()
        {
            Rect anyStateRect = _serializedObject.FindProperty("anyStateEditorRect").rectValue;
            SerializedProperty statesProperty = _serializedObject.FindProperty("_states");
            SerializedProperty anyStateTransitionsProperty = _serializedObject.FindProperty("_anyStateTransitions");
            int anyStateTransitionsCount = anyStateTransitionsProperty.arraySize;

            if (anyStateTransitionsCount == 0)
                return;

            for (int i = 0; i < anyStateTransitionsCount; i++)
            {
                SerializedProperty serializedTransition = anyStateTransitionsProperty.GetArrayElementAtIndex(i);
                int endStateIndex = serializedTransition.FindPropertyRelative("_toStateIndex").intValue;
                FSM_StateBox.DrawAnyStateTransition(anyStateRect, (FSM_State)statesProperty.GetArrayElementAtIndex(endStateIndex).GetValue());
            }
        }

        private void DrawCurrentTransition(Event e)
        {
            if (_creatingAnyStateTransition)
            {
                Rect anyStateRect = _serializedObject.FindProperty("anyStateEditorRect").rectValue;
                FSM_StateBox.DrawAnyStateTransition(anyStateRect, e.mousePosition);
                GUI.changed = true;
            }
            else if (_currentTransitionStartState != null)
            {
                SerializedProperty statesProperty = _serializedObject.FindProperty("_states");
                List<FSM_State> stateList = (List<FSM_State>)statesProperty.GetValue();
                if (!stateList.Contains(_currentTransitionStartState))
                {
                    _currentTransitionStartState = null;
                    return;
                }

                FSM_StateBox.DrawTransition(_currentTransitionStartState, e.mousePosition);
                GUI.changed = true;
            }
        }
        #endregion

        #region Sub Windows
        private bool ProcessSelectionPropSubWindowEvents(Event e)
        {
            bool guiChanged = _selectionPropertiesSubWindow.ProcessEvents(e);

            if (guiChanged)
            {
                GUI.changed = true;
            }
            return guiChanged;
        }

        private bool ProcessParametersSubWindowEvents(Event e)
        {
            bool guiChanged = _parametersSubWindow.ProcessEvents(e);

            if (guiChanged)
            {
                GUI.changed = true;
            }
            return guiChanged;
        }
        #endregion
    }
}
