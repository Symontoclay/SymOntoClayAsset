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

            mInputKeyHelper.AddPressListener(KeyCode.R, OnRPressAction);//NPC takes gun from ground
            mInputKeyHelper.AddPressListener(KeyCode.T, OnTPressAction);//NPC becomes ready for fire
            mInputKeyHelper.AddPressListener(KeyCode.Y, OnYPressAction);//NPC starts fire
            mInputKeyHelper.AddPressListener(KeyCode.U, OnUPressAction);//NPC stops fire
            mInputKeyHelper.AddPressListener(KeyCode.I, OnIPressAction);//NPC becomes not ready for fire
            mInputKeyHelper.AddPressListener(KeyCode.O, OnOPressAction);//NPC throws gun to ground
            mInputKeyHelper.AddPressListener(KeyCode.F, OnFPressAction);//NPC aims to something
            mInputKeyHelper.AddPressListener(KeyCode.Z, OnZPressAction);//NPC rotates
            mInputKeyHelper.AddPressListener(KeyCode.X, OnXPressAction);//NPC rotates head
            mInputKeyHelper.AddPressListener(KeyCode.C, OnCPressAction);//NPC rotates to entity
            mInputKeyHelper.AddPressListener(KeyCode.B, OnBPressAction);//NPC puts to backpack
            mInputKeyHelper.AddPressListener(KeyCode.N, OnNPressAction);//NPC takes gun from backpack
            mInputKeyHelper.AddPressListener(KeyCode.M, OnMPressAction);
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

        private void OnUPressAction()
        {
            Debug.Log("OnUPressAction");

            _uSocGameObject.PushSoundFact(60, "act(q4, shoot)");

            Debug.Log("End OnUPressAction");
        }

        private void OnIPressAction()
        {
            Debug.Log("OnIPressAction");

            _uSocGameObject.PushSoundFact(60, "act(q5, shoot)");

            Debug.Log("End OnIPressAction");
        }

        private void OnOPressAction()
        {
            Debug.Log("OnOPressAction");

            _uSocGameObject.PushSoundFact(60, "act(q6, shoot)");

            Debug.Log("End OnOPressAction");
        }

        private void OnFPressAction()
        {
            Debug.Log("OnFPressAction");

            _uSocGameObject.PushSoundFact(60, "act(q7, shoot)");

            Debug.Log("End OnFPressAction");
        }

        private void OnZPressAction()
        {
            Debug.Log("OnZPressAction");

            _uSocGameObject.PushSoundFact(60, "act(q8, shoot)");

            Debug.Log("End OnZPressAction");
        }

        private void OnXPressAction()
        {
            Debug.Log("OnXPressAction");

            _uSocGameObject.PushSoundFact(60, "act(q9, shoot)");

            Debug.Log("End OnXPressAction");
        }

        private void OnCPressAction()
        {
            Debug.Log("OnCPressAction");

            _uSocGameObject.PushSoundFact(60, "act(q10, shoot)");

            Debug.Log("End OnCPressAction");
        }

        private void OnBPressAction()
        {
            Debug.Log("OnBPressAction");

            _uSocGameObject.PushSoundFact(60, "act(q12, shoot)");

            Debug.Log("End OnBPressAction");
        }

        private void OnNPressAction()
        {
            Debug.Log("OnNPressAction");

            _uSocGameObject.PushSoundFact(60, "act(q13, shoot)");

            Debug.Log("End OnNPressAction");
        }

        private void OnMPressAction()
        {
            Debug.Log("OnMPressAction");

            _uSocGameObject.PushSoundFact(60, "act(q14, shoot)");

            Debug.Log("End OnMPressAction");
        }
    }
}
