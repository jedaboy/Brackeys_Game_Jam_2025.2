using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BGJ_14
{
    public class CursorService : IService
    {
        private int _cursorPlayerIndex;

        private List<object> _cursorUsers = new List<object>();

        public int cursorPlayerIndex => _cursorPlayerIndex;

        public CursorService()
        {
            UpdateCursorState();
        }

        public void AddCursorUser(object owner)
        {
            RemoveNullUsers();
            if (owner == null)
            {
                UpdateCursorState();
                return;
            }

            if (_cursorUsers.Contains(owner))
            {
                return;
            }

            _cursorUsers.Add(owner);
            UpdateCursorState();
        }

        public void RemoveCursorUser(object owner)
        {
            RemoveNullUsers();
            if (owner == null)
            {
                UpdateCursorState();
                return;
            }

            if (!_cursorUsers.Contains(owner))
            {
                return;
            }

            _cursorUsers.Remove(owner);
            UpdateCursorState();
        }

        private void RemoveNullUsers()
        {
            for (int i = 0; i < _cursorUsers.Count; i++)
            {
                if (_cursorUsers[i] == null || _cursorUsers[i].IsUnityNull())
                {
                    _cursorUsers.RemoveAt(i);
                    i--;
                }
            }
        }

        public void UpdateCursorState()
        {
            if (_cursorUsers.Count > 0)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}

