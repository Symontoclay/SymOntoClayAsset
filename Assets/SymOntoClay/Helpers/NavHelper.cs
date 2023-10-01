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

using Assets.SymOntoClay.Helpers;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Interfaces;
using System;
using SymOntoClay.Monitor.Common;
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
            _instancesRegistry = InstancesRegistry.GetRegistry();
        }

        private readonly Transform _transform;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly IExecutorInMainThread _executorInMainThread;
        private readonly InstancesRegistry _instancesRegistry;

        private const int DISTANCE_INTERVAL = 50;
        private const float DISTANCE_DELTA_THRESHOLD = 1f;
        private const float DISTANCE_BETWEEN_TARGET_THRESHOLD = 2f;
        private const int ITERATION_TIMEOUT = 10;

        public Task<IGoResult> Go(IMonitorLogger logger, Vector3 targetPosition, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return NGo(logger, targetPosition, cancellationToken); }, cancellationToken);
        }

        public Task<IGoResult> Go(IMonitorLogger logger, INavTarget target, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return NGo(logger, target, cancellationToken); }, cancellationToken);
        }

        private IGoResult NGo(IMonitorLogger logger, INavTarget target, CancellationToken cancellationToken)
        {
            try
            {
                var kind = target.Kind;

                switch (kind)
                {
                    case KindOfNavTarget.ByAbsoluteCoordinates:
                        {
                            var absoluteCoordinates = target.AbcoluteCoordinates;
                            return NGo(logger, new Vector3(absoluteCoordinates.X, absoluteCoordinates.Y, absoluteCoordinates.Z), cancellationToken);
                        }

                    case KindOfNavTarget.ByEntity:
                        return NGo(logger, target.Entity, cancellationToken);

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                }
            }
            catch(Exception e)
            {
                logger.Error("8CF09945-EC52-4990-8A6C-834980554A0D", e);

                return new GoResult() { GoStatus = GoStatus.SystemError };
            }
        }

        private IGoResult NGo(IMonitorLogger logger, IEntity entity, CancellationToken cancellationToken)
        {
#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"NavHelper NGo entity.InstanceId = {entity.InstanceId}");
            //UnityEngine.Debug.Log($"NavHelper NGo entity.Position = {entity.Position}");
#endif
            var entityInstanceId = entity.InstanceId;

            IEntity rotateToEntityAfterAction = null;

            var kindOfInstance = _instancesRegistry.GetKindOfInstance(entityInstanceId);

            if(kindOfInstance == KindOfInstance.Thing)
            {
                var waypointInstanceId = _instancesRegistry.GetLinkedWaypoint(entityInstanceId);

#if UNITY_EDITOR
                //UnityEngine.Debug.Log($"NavHelper NGo waypointInstanceId = {waypointInstanceId}");
#endif

                if (waypointInstanceId.HasValue)
                {
                    var waypointEntityId = _instancesRegistry.GetId(waypointInstanceId.Value);

#if UNITY_EDITOR
                    //UnityEngine.Debug.Log($"NavHelper NGo waypointEntityId = '{waypointEntityId}'");
#endif

                    rotateToEntityAfterAction = entity;

                    entity = entity.GetNewEntity(logger, waypointEntityId);

#if UNITY_EDITOR
                    //UnityEngine.Debug.Log($"NavHelper NGo entity.InstanceId (2) = {entity.InstanceId}");
                    //UnityEngine.Debug.Log($"NavHelper NGo entity.Position (2) = {entity.Position}");
#endif
                }
            }

            if(entity == null || !entity.Position.HasValue)
            {
#if UNITY_EDITOR
                if(entity == null)
                {
                    Debug.LogError("entity == null");
                }
                else
                {
                    if(!entity.Position.HasValue)
                    {
                        Debug.LogError("!entity.Position.HasValue");
                    }
                }
#endif

                return new GoResult() { GoStatus = GoStatus.SystemError };
            }

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
                    return new GoResult() 
                    { 
                        GoStatus = GoStatus.Success, 
                        TargetEntity = rotateToEntityAfterAction 
                    };
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
                                return new GoResult() 
                                { 
                                    GoStatus = GoStatus.Success,
                                    TargetEntity = rotateToEntityAfterAction 
                                };
                            }

                            var distanceBetweenTarget = Vector3.Distance(position, targetPosition);

#if UNITY_EDITOR
                            //UnityEngine.Debug.Log($"NavHelper NGo distanceBetweenTarget = {distanceBetweenTarget}");
#endif

                            if (distanceBetweenTarget > DISTANCE_BETWEEN_TARGET_THRESHOLD)
                            {
                                return new GoResult() { GoStatus = GoStatus.BrokenByObsticle };
                            }

                            return new GoResult()
                            {
                                GoStatus = GoStatus.Success, 
                                TargetEntity = rotateToEntityAfterAction 
                            };
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

        private IGoResult NGo(IMonitorLogger logger, Vector3 targetPosition, CancellationToken cancellationToken)
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
