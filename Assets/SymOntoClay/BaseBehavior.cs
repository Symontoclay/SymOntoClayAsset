/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay
{
    /// <summary>
    /// Contains base recommended behavior.
    /// </summary>
    public abstract class BaseBehavior : MonoBehaviour, IUHostListener
    {
        protected IUSocGameObject _uSocGameObject;
        private string _idForFacts;
        private object _lockObj = new object();

        protected virtual void Start()
        {
#if DEBUG
            Debug.Log($"BaseBehavior Start");
#endif

            OnStart();
        }

        /// <summary>
        /// Initializes component instead of private method <b>Start</b>.
        /// </summary>
        protected virtual void OnStart()
        {
            _uSocGameObject = GetComponent<IUSocGameObject>();

#if DEBUG
            Debug.Log($"_uSocGameObject = {_uSocGameObject}");
#endif

            _idForFacts = _uSocGameObject.IdForFacts;
        }

        private string _walkingFactId;

        /// <summary>
        /// Adds fact what the NPC has stopped itself.
        /// </summary>
        protected void AddStopFact()
        {
            var factStr = $"act({_idForFacts}, stop)";

#if DEBUG
            //Debug.Log($"BaseBehavior AddStopFact factStr = '{factStr}'");
#endif

            _uSocGameObject.RemovePublicFact(_walkingFactId);
            _walkingFactId = _uSocGameObject.InsertPublicFact(factStr);

#if DEBUG
            //Debug.Log($"BaseBehavior AddStopFact _walkingFactId = {_walkingFactId}");
#endif
        }

        /// <summary>
        /// Adds fact what the NPC has started walking.
        /// </summary>
        protected void AddWalkingFact()
        {
            var factStr = $"act({_idForFacts}, walk)";

#if DEBUG
            //Debug.Log($"BaseBehavior AddWalkingFact factStr = '{factStr}'");
#endif

            _uSocGameObject.RemovePublicFact(_walkingFactId);
            _walkingFactId = _uSocGameObject.InsertPublicFact(factStr);

#if DEBUG
            //Debug.Log($"BaseBehavior AddWalkingFact _walkingFactId = {_walkingFactId}");
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
            _uSocGameObject.RunInMainThread(function);
        }

        /// <summary>
        /// Executes handler in main thread context.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="function">Handler which should be executed in main thread context.</param>
        /// <returns>Result of the execution.</returns>
        protected TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            return _uSocGameObject.RunInMainThread(function);
        }
    }
}