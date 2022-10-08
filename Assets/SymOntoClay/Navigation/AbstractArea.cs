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
