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
            OnStart();
        }

        /// <summary>
        /// Initializes component instead of private method <b>Start</b>.
        /// </summary>
        protected virtual void OnStart()
        {
            _uHumanoidNPC = GetComponent<IUHumanoidNPC>();
            _npc = _uHumanoidNPC.NPC;
            _idForFacts = _uHumanoidNPC.IdForFacts;
        }

        private string _walkingFactId;

        /// <summary>
        /// Adds fact what the NPC has stopped itself.
        /// </summary>
        protected void AddStopFact()
        {
#if DEBUG
            var factStr = $"act({_idForFacts}, stop)";
            //Debug.Log($"ExampleSymOntoClayHumanoidNPC AddStopFact factStr = '{factStr}'");
#endif

            _npc.RemovePublicFact(_walkingFactId);
            _walkingFactId = _npc.InsertPublicFact(factStr);

#if DEBUG
            //Debug.Log($"ExampleSymOntoClayHumanoidNPC AddStopFact _walkingFactId = {_walkingFactId}");
#endif
        }

        /// <summary>
        /// Adds fact what the NPC has started walking.
        /// </summary>
        protected void AddWalkingFact()
        {
#if DEBUG
            var factStr = $"act({_idForFacts}, walk)";
            //Debug.Log($"ExampleSymOntoClayHumanoidNPC AddWalkingFact factStr = '{factStr}'");
#endif

            _npc.RemovePublicFact(_walkingFactId);
            _walkingFactId = _npc.InsertPublicFact(factStr);

#if DEBUG
            //Debug.Log($"ExampleSymOntoClayHumanoidNPC AddWalkingFact _walkingFactId = {_walkingFactId}");
#endif
        }

        private static int _methodId;

        /// <summary>
        /// Returns integer id which is unique for the component.
        /// It can be helpful for debugging host methods.
        /// </summary>
        /// <returns>Integer id which is unique for the component.</returns>
        protected int GetMethodId()
        {
            lock (_lockObj)
            {
                _methodId++;
                return _methodId;
            }
        }

        /// <summary>
        /// Executes handler in main thread context.
        /// </summary>
        /// <param name="function">Handler which should be executed in main thread context.</param>
        protected void RunInMainThread(Action function)
        {
            _npc.RunInMainThread(function);
        }

        /// <summary>
        /// Executes handler in main thread context.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="function">Handler which should be executed in main thread context.</param>
        /// <returns>Result of the execution.</returns>
        protected TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            return _npc.RunInMainThread(function);
        }
    }
}