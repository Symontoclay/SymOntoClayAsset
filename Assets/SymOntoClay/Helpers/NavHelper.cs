using Assets.SymOntoClay.Helpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace SymOntoClay.UnityAsset.Helpers
{
    public class NavHelper
    {
        public NavHelper(Transform transform, NavMeshAgent navMeshAgent, IExecutorInMainThread executorInMainThread)
        {
            _transform = transform;
            _navMeshAgent = navMeshAgent;
            _executorInMainThread = executorInMainThread;
        }

        private readonly Transform _transform;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly IExecutorInMainThread _executorInMainThread;

        public Task<IGoResult> Go(Vector3 targetPosition, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return NGo(targetPosition, cancellationToken); }, cancellationToken);
        }

        private IGoResult NGo(Vector3 targetPosition, CancellationToken cancellationToken)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"NavHelper NGo targetPosition = {targetPosition}");
#endif

            RunInMainThread(() => {
                _navMeshAgent.SetDestination(targetPosition);
            });

            Vector3? oldPosition = null;

            var n = 0;

            while (true)
            {
                var position = RunInMainThread(() => { 
                    return _transform.position;
                });

#if UNITY_EDITOR
                UnityEngine.Debug.Log($"NavHelper NGo position = {position}");
                UnityEngine.Debug.Log($"NavHelper NGo oldPosition = {oldPosition}");
#endif

                if (targetPosition.x == position.x && targetPosition.z == position.z)
                {
                    return new GoResult() { GoStatus = GoStatus.Success };
                }

                n++;

                if (n > 50)
                {
                    n = 0;

                    if (oldPosition.HasValue)
                    {
                        var delta = Vector3.Distance(position, oldPosition.Value);

#if UNITY_EDITOR
                        UnityEngine.Debug.Log($"NavHelper NGo delta = {delta}");
#endif

                        if(delta < 1)
                        {
#if UNITY_EDITOR
                            UnityEngine.Debug.Log("NavHelper NGo delta < 0.1");
#endif

                            var distanceBetweenTarget = Vector3.Distance(position, targetPosition);

#if UNITY_EDITOR
                            UnityEngine.Debug.Log($"NavHelper NGo distanceBetweenTarget = {distanceBetweenTarget}");
#endif

                            if(distanceBetweenTarget > 2)
                            {
                                return new GoResult() { GoStatus = GoStatus.BrokenByObsticle };
                            }

                            return new GoResult() { GoStatus = GoStatus.Success };
                        }
                    }

                    oldPosition = position;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return new GoResult() { GoStatus = GoStatus.Cancelled };
                }

                Thread.Sleep(10);
            }
        }

        private void RunInMainThread(Action function)
        {
            _executorInMainThread.RunInMainThread(function);
        }

        private TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            return _executorInMainThread.RunInMainThread(function);
        }
    }
}
