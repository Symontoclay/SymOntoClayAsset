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

            while (true)
            {
                var position = RunInMainThread(() => { 
                    return _transform.position;
                });

#if UNITY_EDITOR
                UnityEngine.Debug.Log($"NavHelper NGo position = {position}");
#endif

                if (targetPosition.x == position.x && targetPosition.z == position.z)
                {
                    return new GoResult();
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return new GoResult();
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
