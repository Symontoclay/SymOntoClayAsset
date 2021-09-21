using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay
{
    public abstract class BaseBehavior : MonoBehaviour, IUHostListener
    {
        private IUHumanoidNPC _uHumanoidNPC;
        private IHumanoidNPC _npc;
        private string _idForFacts;
        private object _lockObj = new object();

        void Start()
        {

        }

        protected virtual void OnStart()
        {
            _uHumanoidNPC = GetComponent<IUHumanoidNPC>();
            _npc = _uHumanoidNPC.NPC;
            _idForFacts = _uHumanoidNPC.IdForFacts;
        }

        private string _walkingFactId;

        protected void AddStopFact()
        {
#if DEBUG
            var factStr = $"act({_idForFacts}, stop)";
            //Debug.Log($"ExampleSymOntoClayHumanoidNPC AddStopFact factStr = '{factStr}'");
#endif

            _npc.RemovePublicFact(_walkingFactId);
            _walkingFactId = _npc.InsertPublicFact($"act({_idForFacts}, stop)");

#if DEBUG
            //Debug.Log($"ExampleSymOntoClayHumanoidNPC AddStopFact _walkingFactId = {_walkingFactId}");
#endif
        }

        protected void AddWalkingFact()
        {
#if DEBUG
            var factStr = $"act({_idForFacts}, walk)";
            //Debug.Log($"ExampleSymOntoClayHumanoidNPC AddWalkingFact factStr = '{factStr}'");
#endif

            _npc.RemovePublicFact(_walkingFactId);
            _walkingFactId = _npc.InsertPublicFact($"act({_idForFacts}, walk)");

#if DEBUG
            //Debug.Log($"ExampleSymOntoClayHumanoidNPC AddWalkingFact _walkingFactId = {_walkingFactId}");
#endif
        }

        private static int _methodId;

        protected int GetMethodId()
        {
            lock (_lockObj)
            {
                _methodId++;
                return _methodId;
            }
        }

        protected void RunInMainThread(Action function)
        {
            _npc.RunInMainThread(function);
        }

        protected TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            return _npc.RunInMainThread(function);
        }
    }
}