using SymOntoClay.UnityAsset.Samples.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Samples.Internal
{
    public delegate void OnKeyPressAction(KeyCode keyCode);

    public class InputKeyHelper
    {
        public class InputKeyHandlers
        {
            public InputKeyHandlers(KeyCode keyCode, IPlayerCommonBus playerCommonBus)
            {
                _keyCode = keyCode;
                _playerCommonBus = playerCommonBus;
            }

            private IPlayerCommonBus _playerCommonBus;
            private KeyCode _keyCode;

            private readonly object mLockObj = new object();

            private event OnKeyPressAction mPressHandlers;
            private event OnKeyPressAction mUpHandlers;
            private event Action mPressActionsHadnlers;
            private event Action mUpActionsHandlers;
            private bool _isPressed;

            public void Update()
            {
                lock (mLockObj)
                {
                    var tmpKeyDownValue = _playerCommonBus.GetKeyDown(_keyCode);
                    var tmpKeyUpValue = _playerCommonBus.GetKeyUp(_keyCode);

                    if (tmpKeyDownValue)
                    {
                        if (_isPressed)
                        {
                            return;
                        }

                        _isPressed = true;
                        mPressHandlers?.Invoke(_keyCode);
                        mPressActionsHadnlers?.Invoke();
                    }
                    else
                    {
                        if (tmpKeyUpValue)
                        {
                            if (!_isPressed)
                            {
                                return;
                            }

                            _isPressed = false;
                            mUpHandlers?.Invoke(_keyCode);
                            mUpActionsHandlers?.Invoke();
                        }
                    }
                }
            }

            public void AddPressHandler(OnKeyPressAction action)
            {
                lock (mLockObj)
                {
                    mPressHandlers += action;
                }
            }

            public void AddUpHandler(OnKeyPressAction action)
            {
                lock (mLockObj)
                {
                    mUpHandlers += action;
                }
            }

            public void AddPressHandler(Action action)
            {
                lock (mLockObj)
                {
                    mPressActionsHadnlers += action;
                }
            }

            public void AddUpHandler(Action action)
            {
                lock (mLockObj)
                {
                    mUpActionsHandlers += action;
                }
            }
        }

        public InputKeyHelper(IPlayerCommonBus playerCommonBus)
        {
            _playerCommonBus = playerCommonBus;
        }

        private IPlayerCommonBus _playerCommonBus;
        private readonly object _lockObj = new object();
        private Dictionary<KeyCode, InputKeyHandlers> mHandlersDict = new Dictionary<KeyCode, InputKeyHandlers>();

        public void Update()
        {
            lock (_lockObj)
            {
                foreach (var handlerKVPItem in mHandlersDict)
                {
                    handlerKVPItem.Value.Update();
                }
            }
        }

        private InputKeyHandlers GetHandler(KeyCode key)
        {
            lock (_lockObj)
            {
                if (mHandlersDict.ContainsKey(key))
                {
                    return mHandlersDict[key];
                }

                var handler = new InputKeyHandlers(key, _playerCommonBus);
                mHandlersDict[key] = handler;
                return handler;
            }
        }

        public void AddPressListener(KeyCode key, OnKeyPressAction action)
        {
            var handler = GetHandler(key);
            handler.AddPressHandler(action);
        }

        public void AddPressListener(KeyCode key, Action action)
        {
            var handler = GetHandler(key);
            handler.AddPressHandler(action);
        }

        public void AddUpListener(KeyCode key, OnKeyPressAction action)
        {
            var handler = GetHandler(key);
            handler.AddUpHandler(action);
        }

        public void AddUpListener(KeyCode key, Action action)
        {
            var handler = GetHandler(key);
            handler.AddUpHandler(action);
        }
    }
}
