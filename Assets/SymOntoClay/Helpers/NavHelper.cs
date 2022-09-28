using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

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
            throw new NotImplementedException();
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
