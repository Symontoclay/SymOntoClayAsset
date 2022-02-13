/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Interfaces
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
