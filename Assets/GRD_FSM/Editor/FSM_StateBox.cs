using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GRD.FSM
{
    public class FSM_StateBox
    {
        static FSM_State contextState;
        static FSM_NodeEditorWindow contextWindow;

        public delegate void OnStateInteraction(int stateIndex);

        public static void Draw(FSM_State state, GUIStyle defaultStyle, GUIStyle selectedStyle)
        {
            GUIStyle style = state.isSelected ? selectedStyle : defaultStyle;
            GUI.Box(state.editorRect, state.name, style);
        }

        public static bool ProcessEvents(FSM_State state, int stateIndex, FSM_NodeEditorWindow windowContext, Event e, bool alreadyCastBox, OnStateInteraction onStateSelected, OnStateInteraction onStateDeselected)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (state.editorRect.Contains(e.mousePosition) && !alreadyCastBox)
                        {
                            state.isDragged = true;
                            GUI.changed = true;
                            state.isSelected = true;
                            onStateSelected?.Invoke(stateIndex);
                            return true;
                        }
                        else
                        {
                            GUI.changed = true;
                            state.isSelected = false;
                            onStateDeselected?.Invoke(stateIndex);
                        }
                    }

                    if (e.button == 1 && state.editorRect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu(state, windowContext);
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    state.isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && state.isDragged)
                    {
                        state.DragBox(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }

        private static void ProcessContextMenu(FSM_State state, FSM_NodeEditorWindow windowContext)
        {
            contextState = state;
            contextWindow = windowContext;
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Create Transition"), false, OnClickCreateTransition);
            genericMenu.AddItem(new GUIContent("Set as Default State"), false, OnClickSetAsDefaultState);
            genericMenu.AddItem(new GUIContent("Delete State"), false, OnClickRemoveNode);
            genericMenu.ShowAsContext();
        }

        private static void OnClickCreateTransition()
        {
            if (contextState == null || contextWindow == null)
                return;

            contextWindow.OnClickCreateTransition(contextState);
        }

        private static void OnClickSetAsDefaultState()
        {
            if (contextState == null || contextWindow == null)
                return;

            contextWindow.OnClickSetDefaultState(contextState);
        }

        private static void OnClickRemoveNode()
        {
            if (contextState == null || contextWindow == null)
                return;

            contextWindow.OnClickDeleteState(contextState);
        }

        #region Transitions
        public static void DrawTransition(FSM_State startState, FSM_State endState)
        {
            if (startState == endState)
            {
                Vector3 bottomPoint = startState.editorRect.center;
                bottomPoint.y = startState.editorRect.yMax;
                Handles.DrawAAConvexPolygon(
                    bottomPoint,
                    bottomPoint + Vector3.right * 5 + Vector3.up * 10,
                    bottomPoint - Vector3.right * 5 + Vector3.up * 10);
                return;
            }

            Vector3 lineStartPoint = startState.editorRect.center;
            Vector3 lineEndPoint = endState.editorRect.center;
            Vector3 lineDirection = (lineEndPoint - lineStartPoint).normalized;
            Vector3 perpDirection = Vector3.Cross(lineDirection, Vector3.forward).normalized;
            lineStartPoint += perpDirection * 10f;
            lineEndPoint += perpDirection * 10f;

            Vector3 midPoint = new Vector3(
                (lineStartPoint.x + lineEndPoint.x) / 2,
                (lineStartPoint.y + lineEndPoint.y) / 2,
                (lineStartPoint.z + lineEndPoint.z) / 2
                );

            Handles.DrawLine(lineStartPoint, lineEndPoint);
            Handles.DrawAAConvexPolygon(midPoint + lineDirection * 10, midPoint + perpDirection * 5, midPoint - perpDirection * 5);
        }

        public static void DrawTransition(FSM_State startState, Vector2 mousePosition)
        {
            Handles.color = Color.cyan;
            Vector3 lineStartPoint = startState.editorRect.center;
            Vector3 lineEndPoint = mousePosition;
            Vector3 lineDirection = (lineEndPoint - lineStartPoint).normalized;
            Vector3 perpDirection = Vector3.Cross(lineDirection, Vector3.forward).normalized;

            Vector3 midPoint = new Vector3(
                (lineStartPoint.x + lineEndPoint.x) / 2,
                (lineStartPoint.y + lineEndPoint.y) / 2,
                (lineStartPoint.z + lineEndPoint.z) / 2
                );

            Handles.DrawLine(lineStartPoint, lineEndPoint);
            Handles.DrawAAConvexPolygon(midPoint + lineDirection * 10, midPoint + perpDirection * 5, midPoint - perpDirection * 5);
            Handles.color = Color.white;

        }

        public static void DrawAnyStateTransition(Rect anyStateRect, FSM_State endState)
        {
            Vector3 lineStartPoint = anyStateRect.center;
            Vector3 lineEndPoint = endState.editorRect.center;
            Vector3 lineDirection = (lineEndPoint - lineStartPoint).normalized;
            Vector3 perpDirection = Vector3.Cross(lineDirection, Vector3.forward).normalized;
            lineStartPoint += perpDirection * 10f;
            lineEndPoint += perpDirection * 10f;

            Vector3 midPoint = new Vector3(
                (lineStartPoint.x + lineEndPoint.x) / 2,
                (lineStartPoint.y + lineEndPoint.y) / 2,
                (lineStartPoint.z + lineEndPoint.z) / 2
                );

            Handles.DrawLine(lineStartPoint, lineEndPoint);
            Handles.DrawAAConvexPolygon(midPoint + lineDirection * 10, midPoint + perpDirection * 5, midPoint - perpDirection * 5);
        }

        public static void DrawAnyStateTransition(Rect anyStateRect, Vector2 mousePosition)
        {
            Handles.color = Color.cyan;
            Vector3 lineStartPoint = anyStateRect.center;
            Vector3 lineEndPoint = mousePosition;
            Vector3 lineDirection = (lineEndPoint - lineStartPoint).normalized;
            Vector3 perpDirection = Vector3.Cross(lineDirection, Vector3.forward).normalized;

            Vector3 midPoint = new Vector3(
                (lineStartPoint.x + lineEndPoint.x) / 2,
                (lineStartPoint.y + lineEndPoint.y) / 2,
                (lineStartPoint.z + lineEndPoint.z) / 2
                );

            Handles.DrawLine(lineStartPoint, lineEndPoint);
            Handles.DrawAAConvexPolygon(midPoint + lineDirection * 10, midPoint + perpDirection * 5, midPoint - perpDirection * 5);
            Handles.color = Color.white;

        }
        #endregion
    }
}
