/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Helpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;

namespace SymOntoClay.UnityAsset.Navigation
{
    public abstract class AbstractArea : MonoBehaviour, IPlatformSupport, IExecutorInMainThread
    {
        /// <inheritdoc/>
        void IExecutorInMainThread.RunInMainThread(Action function)
        {
            RunInMainThread(function);
        }

        /// <inheritdoc/>
        TResult IExecutorInMainThread.RunInMainThread<TResult>(Func<TResult> function)
        {
            return RunInMainThread(function);
        }

        /// <summary>
        /// Executes handler in main thread context.
        /// </summary>
        /// <param name="function">Handler which should be executed in main thread context.</param>
        protected abstract void RunInMainThread(Action function);

        /// <summary>
        /// Executes handler in main thread context.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="function">Handler which should be executed in main thread context.</param>
        /// <returns>Result of the execution.</returns>
        protected abstract TResult RunInMainThread<TResult>(Func<TResult> function);

        /// <inheritdoc/>
        System.Numerics.Vector3 IPlatformSupport.ConvertFromRelativeToAbsolute(SymOntoClay.Core.RelativeCoordinate relativeCoordinate)
        {
            return PlatformSupportHelper.ConvertFromRelativeToAbsolute(transform, relativeCoordinate);
        }

        /// <inheritdoc/>
        System.Numerics.Vector3 IPlatformSupport.GetCurrentAbsolutePosition()
        {
            return RunInMainThread(() => {
                return PlatformSupportHelper.GetCurrentAbsolutePosition(transform);
            });            
        }

        /// <inheritdoc/>
        float IPlatformSupport.GetDirectionToPosition(System.Numerics.Vector3 position)
        {
            return PlatformSupportHelper.GetDirectionToPosition(transform, position);
        }

        /// <inheritdoc/>
        bool IPlatformSupport.CanBeTakenBy(IEntity subject)
        {
            return false;
        }
    }
}
