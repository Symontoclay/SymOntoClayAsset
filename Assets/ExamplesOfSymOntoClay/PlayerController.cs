﻿using SymOntoClay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExamplesOfSymOntoClay
{
    [RequireComponent(typeof(IUSocGameObject))]
    public class PlayerController : BaseBehavior
    {
        private IPlayerCommonBus _playerCommonBus;
        private InputKeyHelper mInputKeyHelper;

        protected override void Start()
        {
#if DEBUG
            Debug.Log($"ExampleSymOntoClayPlayer Start");
#endif

            base.Start();

            _playerCommonBus = PlayerCommonBus.GetBus();

            mInputKeyHelper = new InputKeyHelper(_playerCommonBus);

            mInputKeyHelper.AddPressListener(KeyCode.R, OnRPressAction);
            mInputKeyHelper.AddPressListener(KeyCode.T, OnTPressAction);
            mInputKeyHelper.AddPressListener(KeyCode.Y, OnYPressAction);
        }

        void Update()
        {
            mInputKeyHelper.Update();
        }

        private void OnRPressAction()
        {
            Debug.Log("OnRPressAction");

            _uSocGameObject.PushSoundFact(60, "act(q1, shoot)");
            
            Debug.Log("End OnRPressAction");
        }

        private void OnTPressAction()
        {
            Debug.Log("OnTPressAction");

            _uSocGameObject.PushSoundFact(60, "act(q2, shoot)");

            Debug.Log("End OnTPressAction");
        }

        private void OnYPressAction()
        {
            Debug.Log("OnYPressAction");

            _uSocGameObject.PushSoundFact(60, "act(q3, shoot)");

            Debug.Log("End OnYPressAction");
        }
    }
}