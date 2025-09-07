using SymOntoClay.UnityAsset.Samples.Interfaces;
using SymOntoClay.UnityAsset.Samples.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Samples.Environment
{
    public class PlayerCommonBus : MonoBehaviour, IPlayerCommonBus
    {
        #region static_members
        private static object _sLockObj = new object();
        private static IPlayerCommonBus _instance;

        public static IPlayerCommonBus GetBus()
        {
            lock(_sLockObj)
            {
                if(_instance == null)
                {
                    _instance = FindFirstObjectByType<PlayerCommonBus>();
                }

                return _instance;
            }
        }
        #endregion

        private PlayerInputMode _playerInputMode =  PlayerInputMode.Player;
        private bool _needLockingCursor;
        private int _counter;
        private readonly object _lockObj = new object();

        /// <inheritdoc/>
        public float GetAxis(string name)
        {
#if DEBUG
            //Debug.Log($"GetAxis name = {name}; _playerInputMode = {_playerInputMode}");
#endif

            if (_playerInputMode == PlayerInputMode.Window)
            {
                return 0f;
            }

            return Input.GetAxis(name);
        }

        /// <inheritdoc/>
        public bool GetKeyUp(KeyCode key)
        {
            if (_playerInputMode == PlayerInputMode.Window)
            {
                return false;
            }

            return Input.GetKeyUp(key);
        }

        /// <inheritdoc/>
        public bool GetKeyDown(KeyCode key)
        {
            if (_playerInputMode == PlayerInputMode.Window)
            {
                return false;
            }

            return Input.GetKeyDown(key);
        }

        /// <inheritdoc/>
        public bool GetButtonDown(string name)
        {
            if (_playerInputMode == PlayerInputMode.Window)
            {
                return false;
            }

            return Input.GetButtonDown(name);
        }

        /// <inheritdoc/>
        public bool GetMouseButtonUp(int button)
        {
            if (_playerInputMode == PlayerInputMode.Window)
            {
                return false;
            }

            return Input.GetMouseButtonUp(button);
        }

        /// <inheritdoc/>
        public bool GetMouseButtonDown(int button)
        {
            if (_playerInputMode == PlayerInputMode.Window)
            {
                return false;
            }

            return Input.GetMouseButtonDown(button);
        }

        /// <inheritdoc/>
        public void SetCharacterMode()
        {
            lock (_lockObj)
            {
                var oldValue = _playerInputMode;

                _playerInputMode = PlayerInputMode.Player;

                if (oldValue != _playerInputMode)
                {
                    _counter = 0;
                }

                _needLockingCursor = true;
            }
        }

        /// <inheritdoc/>
        public void AddWindow()
        {
            lock (_lockObj)
            {
                if (!_needLockingCursor)
                {
                    return;
                }

                _playerInputMode = PlayerInputMode.Window;
                _counter++;
            }
        }

        /// <inheritdoc/>
        public void ReleaseWindow()
        {
            lock (_lockObj)
            {
                if (!_needLockingCursor)
                {
                    return;
                }

                _counter--;

                if (_counter == 0)
                {
                    _playerInputMode = PlayerInputMode.Player;
                }
            }
        }

        void Update()
        {
            InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if (GetKeyUp(KeyCode.Escape))
            {
#if DEBUG
                //Debug.Log("InternalLockUpdate GetKeyUp(KeyCode.Escape)");
#endif

                _playerInputMode = PlayerInputMode.Window;
            }
            else
            {
                //if(GetMouseButtonUp(0))
                //{
                //    UserClientMode = UserClientMode.Character;
                //}
            }

            if (_playerInputMode == PlayerInputMode.Player)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            //if (GetKeyUp(KeyCode.Escape))
            //{
            //    mCursorIsLocked = false;
            //}
            //else
            //{
            //    if (GetMouseButtonUp(0))
            //    {
            //        mCursorIsLocked = true;
            //    }
            //}

            //if (mCursorIsLocked)
            //{
            //    Cursor.lockState = CursorLockMode.Locked;
            //    Cursor.visible = false;
            //}
            //else
            //{
            //    if (!mCursorIsLocked)
            //    {
            //        Cursor.lockState = CursorLockMode.None;
            //        Cursor.visible = true;
            //    }
            //}
        }
    }
}
