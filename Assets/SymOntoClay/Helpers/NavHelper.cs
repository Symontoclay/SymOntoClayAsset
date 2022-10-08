using Assets.SymOntoClay.Helpers;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
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

        private const int DISTANCE_INTERVAL = 50;
        private const float DISTANCE_DELTA_THRESHOLD = 1f;
        private const float DISTANCE_BETWEEN_TARGET_THRESHOLD = 2f;
        private const int ITERATION_TIMEOUT = 10;

        public Task<IGoResult> Go(Vector3 targetPosition, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return NGo(targetPosition, cancellationToken); }, cancellationToken);
        }

        public Task<IGoResult> Go(INavTarget target, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return NGo(target, cancellationToken); }, cancellationToken);
        }

        private IGoResult NGo(INavTarget target, CancellationToken cancellationToken)
        {
            try
            {
                var kind = target.Kind;

                switch (kind)
                {
                    case KindOfNavTarget.ByAbsoluteCoordinates:
                        {
                            var absoluteCoordinates = target.AbcoluteCoordinates;
                            return NGo(new Vector3(absoluteCoordinates.X, absoluteCoordinates.Y, absoluteCoordinates.Z), cancellationToken);
                        }

                    case KindOfNavTarget.ByEntity:
                        return NGo(target.Entity, cancellationToken);

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                }
            }
            catch(Exception e)
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogError(e);
#endif
            }

            throw new NotImplementedException();
        }

        private IGoResult NGo(IEntity entity, CancellationToken cancellationToken)
        {
#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"NavHelper NGo entity.InstanceId = {entity.InstanceId}");
            //UnityEngine.Debug.Log($"NavHelper NGo entity.Position = {entity.Position}");
#endif
            var entityInstanceId = entity.InstanceId;
            var tpos = entity.Position.Value;

            var targetPosition = new Vector3(tpos.X, tpos.Y, tpos.Z);

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
                //UnityEngine.Debug.Log($"NavHelper NGo position = {position}");
                //UnityEngine.Debug.Log($"NavHelper NGo oldPosition = {oldPosition}");
#endif

                if (targetPosition.x == position.x && targetPosition.z == position.z)
                {
                    return new GoResult() { GoStatus = GoStatus.Success };
                }

                n++;

                if (n > DISTANCE_INTERVAL)
                {
                    n = 0;

                    if (oldPosition.HasValue)
                    {
                        var delta = Vector3.Distance(position, oldPosition.Value);

#if UNITY_EDITOR
                        //UnityEngine.Debug.Log($"NavHelper NGo delta = {delta}");
#endif

                        if (delta < DISTANCE_DELTA_THRESHOLD)
                        {
#if UNITY_EDITOR
                            //UnityEngine.Debug.Log("NavHelper NGo delta < 0.1");
#endif

                            var isTargetPlace = RunInMainThread(() =>
                            {
                                var ray = new Ray(position, Vector3.down);
                                var hits = Physics.RaycastAll(ray);

                                foreach (var hit in hits)
                                {
                                    var hitInstanceId = hit.transform.gameObject.GetInstanceID();

                                    if(hitInstanceId == entityInstanceId)
                                    {
                                        return true;
                                    }
                                }

                                return false;
                            });


#if UNITY_EDITOR
                            //UnityEngine.Debug.Log($"NavHelper NGo isTargetPlace = {isTargetPlace}");
#endif

                            if(isTargetPlace)
                            {
                                return new GoResult() { GoStatus = GoStatus.Success };
                            }

                            var distanceBetweenTarget = Vector3.Distance(position, targetPosition);

#if UNITY_EDITOR
                            //UnityEngine.Debug.Log($"NavHelper NGo distanceBetweenTarget = {distanceBetweenTarget}");
#endif

                            if (distanceBetweenTarget > DISTANCE_BETWEEN_TARGET_THRESHOLD)
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

                Thread.Sleep(ITERATION_TIMEOUT);
            }
        }

        private IGoResult NGo(Vector3 targetPosition, CancellationToken cancellationToken)
        {
#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"NavHelper NGo targetPosition = {targetPosition}");
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
                //UnityEngine.Debug.Log($"NavHelper NGo position = {position}");
                //UnityEngine.Debug.Log($"NavHelper NGo oldPosition = {oldPosition}");
#endif

                if (targetPosition.x == position.x && targetPosition.z == position.z)
                {
                    return new GoResult() { GoStatus = GoStatus.Success };
                }

                n++;

                if (n > DISTANCE_INTERVAL)
                {
                    n = 0;

                    if (oldPosition.HasValue)
                    {
                        var delta = Vector3.Distance(position, oldPosition.Value);

#if UNITY_EDITOR
                        //UnityEngine.Debug.Log($"NavHelper NGo delta = {delta}");
#endif

                        if(delta < DISTANCE_DELTA_THRESHOLD)
                        {
#if UNITY_EDITOR
                            //UnityEngine.Debug.Log("NavHelper NGo delta < 0.1");
#endif

                            var distanceBetweenTarget = Vector3.Distance(position, targetPosition);

#if UNITY_EDITOR
                            //UnityEngine.Debug.Log($"NavHelper NGo distanceBetweenTarget = {distanceBetweenTarget}");
#endif

                            if(distanceBetweenTarget > DISTANCE_BETWEEN_TARGET_THRESHOLD)
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

                Thread.Sleep(ITERATION_TIMEOUT);
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
