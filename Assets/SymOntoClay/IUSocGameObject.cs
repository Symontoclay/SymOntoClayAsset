using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay
{
    /// <summary>
    /// Represents base SymOntoClay's game object component.
    /// </summary>
    public interface IUSocGameObject
    {
        /// <summary>
        /// Gets unique Id which is prepared to using in building fact string.
        /// </summary>
        string IdForFacts { get; }
        IEntityLogger Logger { get; }
        IGameObject SocGameObject { get; }

        /// <summary>
        /// Executes handler in main thread context.
        /// </summary>
        /// <param name="function">Handler which should be executed in main thread context.</param>
        void RunInMainThread(Action function);

        /// <summary>
        /// Executes handler in main thread context.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="function">Handler which should be executed in main thread context.</param>
        /// <returns>Result of the execution.</returns>
        TResult RunInMainThread<TResult>(Func<TResult> function);

        /// <summary>
        /// Inserts public fact into storage.
        /// Another NPCs can percept the fact.
        /// </summary>
        /// <param name="text">String that represents the fact.</param>
        /// <returns>Id of inserted fact.</returns>
        string InsertPublicFact(string text);

        /// <summary>
        /// Remove public fact from storage.
        /// Another NPCs can not percept the fact.
        /// </summary>
        /// <param name="id">Id of previously inserted fact.</param>
        void RemovePublicFact(string id);

        /// <summary>
        /// Pushes fact to special delivery system which simulates hearing.
        /// </summary>
        /// <param name="power">Power of sound.</param>
        /// <param name="text">String that represents the fact.</param>
        void PushSoundFact(float power, string text);        
    }
}
