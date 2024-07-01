/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.Threading;

namespace SymOntoClay.UnityAsset.Helpers
{
    public class NavHelper
    {
        public NavHelper(Transform transform, NavMeshAgent navMeshAgent, IExecutorInMainThread executorInMainThread, ICustomThreadPool threadPool)
        {
            _transform = transform;
            _navMeshAgent = navMeshAgent;
            _executorInMainThread = executorInMainThread;
            _threadPool = threadPool;
            _instancesRegistry = InstancesRegistry.GetRegistry();
        }

        private readonly Transform _transform;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly IExecutorInMainThread _executorInMainThread;
        private readonly InstancesRegistry _instancesRegistry;
        protected ICustomThreadPool _threadPool;

        private const int CHECKING_DISTANCE_ITERATINONS_COUNT = 50;
        private const float DISTANCE_DELTA_THRESHOLD = 1f;
        private const float DISTANCE_BETWEEN_TARGET_THRESHOLD = 2f;
        private const int ITERATION_TIMEOUT = 10;

        public Task<IGoResult> GoAsync(IMonitorLogger logger, Vector3 targetPosition, CancellationToken cancellationToken)
        {
            return ThreadTask<IGoResult>.Run(() => { return NGo(logger, targetPosition, cancellationToken); }, _threadPool, cancellationToken).StandardTaskWithResult;
        }

        public Task<IGoResult> GoAsync(IMonitorLogger logger, INavTarget target, CancellationToken cancellationToken)
        {
            return ThreadTask<IGoResult>.Run(() => { return NGo(logger, target, cancellationToken); }, _threadPool, cancellationToken).StandardTaskWithResult;
        }

        public IGoResult Go(IMonitorLogger logger, Vector3 targetPosition, CancellationToken cancellationToken)
        {
            return NGo(logger, targetPosition, cancellationToken);
        }

        public IGoResult Go(IMonitorLogger logger, INavTarget target, CancellationToken cancellationToken)
        {
            return NGo(logger, target, cancellationToken);
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
#if DEBUG
            logger.Info("CB029D52-E067-44D2-AA39-B8819DF7E3C8", $"NavHelper NGo entity.InstanceId = {entity.InstanceId}");
            logger.Info("B1D15131-DA78-444B-844C-3CD6DF243463", $"NavHelper NGo entity.Position = {entity.Position}");
#endif
            var entityInstanceId = entity.InstanceId;

            IEntity rotateToEntityAfterAction = null;

            var kindOfInstance = _instancesRegistry.GetKindOfInstance(entityInstanceId);

#if DEBUG
            logger.Info("4F354F59-6871-4C33-AFA9-912DA7A5A253", $"NavHelper NGo kindOfInstance = {kindOfInstance}");
#endif

            if (kindOfInstance == KindOfInstance.Thing)
            {
                var waypointInstanceId = _instancesRegistry.GetLinkedWaypoint(entityInstanceId);

#if DEBUG
                logger.Info("36983AB2-6F78-4094-8BDD-6C85B9B6295E", $"NavHelper NGo waypointInstanceId = {waypointInstanceId}");
#endif

                if (waypointInstanceId.HasValue)
                {
                    var waypointEntityId = _instancesRegistry.GetId(waypointInstanceId.Value);

#if DEBUG
                    logger.Info("916C007A-77A2-4ECC-9612-824C8980B7B9", $"NavHelper NGo waypointEntityId = '{waypointEntityId}'");
#endif

                    rotateToEntityAfterAction = entity;

                    entity = entity.GetNewEntity(logger, waypointEntityId);

#if DEBUG
                    logger.Info("D8EC0C1D-26FE-46D9-AE0E-3A70542CAD2F", $"NavHelper NGo entity.InstanceId (2) = {entity.InstanceId}");
                    logger.Info("43756F6E-E736-4967-ACE4-7DACA4934F37", $"NavHelper NGo entity.Position (2) = {entity.Position}");
#endif
                }
            }

            if(entity == null || !entity.Position.HasValue)
            {
#if DEBUG
                if (entity == null)
                {
                    logger.Error("505FF421-EFAE-4DBB-9C0D-17E72B98A5A9", "entity == null");
                }
                else
                {
                    if(!entity.Position.HasValue)
                    {
                        logger.Error("6439A787-1768-4231-8BE4-83D3A07AE368", "!entity.Position.HasValue");
                    }
                }
#endif

                return new GoResult() { GoStatus = GoStatus.SystemError };
            }

            var tpos = entity.Position.Value;

            var targetPosition = new Vector3(tpos.X, tpos.Y, tpos.Z);

#if DEBUG
            logger.Info("E065C81F-3471-4007-89D0-52B38D06A453", $"NavHelper NGo targetPosition = {targetPosition}");
#endif

            RunInMainThread(() => {
                _navMeshAgent.SetDestination(targetPosition);
            });

            Vector3? oldPosition = null;

            var current_checking_distance_iteration = 0;

            while (true)
            {
                var position = RunInMainThread(() => {
                    return _transform.position;
                });

#if DEBUG
                logger.Info("B5B48200-34DA-41D5-B8B2-C81D3FE0B85C", $"NavHelper NGo position = {position}");
                logger.Info("553047DA-6C89-4AA3-8C74-08E7542086CC", $"NavHelper NGo oldPosition = {oldPosition}");
                var ditanceToTarget = Vector3.Distance(position, targetPosition);
                logger.Info("553047DA-6C89-4AA3-8C74-08E7542086CC", $"NavHelper NGo ditanceToTarget = {ditanceToTarget}");
#endif

                if (targetPosition.x == position.x && targetPosition.z == position.z)
                {
#if DEBUG
                    logger.Info("D25FD47D-4451-4518-9B52-255CFA55B02A", "NavHelper NGo targetPosition.x == position.x && targetPosition.z == position.z");
#endif

                    return new GoResult() 
                    { 
                        GoStatus = GoStatus.Success, 
                        TargetEntity = rotateToEntityAfterAction 
                    };
                }

                current_checking_distance_iteration++;

#if DEBUG
                logger.Info("CEB9E4E3-F3A2-4DCC-B1E7-AE7004CBA0F1", $"NavHelper NGo current_checking_distance_iteration = {current_checking_distance_iteration}");
#endif

                if (current_checking_distance_iteration > CHECKING_DISTANCE_ITERATINONS_COUNT)
                {
#if DEBUG
                    logger.Info("B2A30E00-8D2F-487C-82F3-E60D56CB4731", "NavHelper NGo current_checking_distance_iteration > CHECKING_DISTANCE_ITERATINONS_COUNT");
#endif

                    current_checking_distance_iteration = 0;

                    if (oldPosition.HasValue)
                    {
                        var delta = Vector3.Distance(position, oldPosition.Value);

#if DEBUG
                        logger.Info("D53900BF-2C6B-41CA-82EC-021A863CE7CD", $"NavHelper NGo delta = {delta}");
#endif

                        if (delta < DISTANCE_DELTA_THRESHOLD)
                        {
#if DEBUG
                            logger.Info("A0669AD8-01C9-4A9B-B117-B83FA675E7FA", "NavHelper NGo delta < delta < DISTANCE_DELTA_THRESHOLD");
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


#if DEBUG
                            logger.Info("0F479173-0DEC-402B-A275-360EE943BD67", $"NavHelper NGo isTargetPlace = {isTargetPlace}");
#endif

                            if (isTargetPlace)
                            {
#if DEBUG
                                logger.Info("78E99437-85B8-47F7-8652-41A09634B4CC", "NavHelper NGo if (isTargetPlace)");
#endif

                                return new GoResult() 
                                {
                                    GoStatus = GoStatus.Success,
                                    TargetEntity = rotateToEntityAfterAction 
                                };
                            }

                            var distanceBetweenTarget = Vector3.Distance(position, targetPosition);

#if DEBUG
                            logger.Info("DD7AEF5A-ED58-411E-A29D-96A01BD36C91", $"NavHelper NGo distanceBetweenTarget = {distanceBetweenTarget}");
#endif

                            if (distanceBetweenTarget > DISTANCE_BETWEEN_TARGET_THRESHOLD)
                            {
#if DEBUG
                                logger.Info("86049261-1C68-45C2-AA65-BCE5B0DFF6D8", "NavHelper NGo distanceBetweenTarget > DISTANCE_BETWEEN_TARGET_THRESHOLD");
#endif

                                return new GoResult() { GoStatus = GoStatus.BrokenByObsticle };
                            }

#if DEBUG
                            logger.Info("A2982D14-98BE-44D1-9D69-BEC689E1A107", "NavHelper NGo return new GoResult() {GoStatus = GoStatus.Success, TargetEntity = rotateToEntityAfterAction}");
#endif

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
#if DEBUG
                    logger.Info("38F4CAED-A45B-4FA7-8AA5-A22062B8E863", "NavHelper NGo ");
#endif

                    return new GoResult() { GoStatus = GoStatus.Cancelled };
                }

                Thread.Sleep(ITERATION_TIMEOUT);
            }
        }

        private IGoResult NGo(IMonitorLogger logger, Vector3 targetPosition, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.Info("023E4E47-3629-4CB2-86C2-6591D58B05DD", $"NavHelper NGo targetPosition = {targetPosition}");
#endif

            RunInMainThread(() => {
                _navMeshAgent.SetDestination(targetPosition);
            });

            Vector3? oldPosition = null;

            var current_checking_distance_iteration = 0;

            while (true)
            {
                var position = RunInMainThread(() => { 
                    return _transform.position;
                });

#if DEBUG
                logger.Info("6F9F507F-E56F-4EE4-BF97-02B8F2C428A0", $"NavHelper NGo position = {position}");
                logger.Info("CD95EF47-AD7D-4A7D-A183-7FDCF8E468BD", $"NavHelper NGo oldPosition = {oldPosition}");
#endif

                if (targetPosition.x == position.x && targetPosition.z == position.z)
                {
#if DEBUG
                    logger.Info("AC70B852-FB6D-4AB6-A43D-6E1049562A7F", "NavHelper NGo ");
#endif

                    return new GoResult() { GoStatus = GoStatus.Success };
                }

                current_checking_distance_iteration++;

#if DEBUG
                logger.Info("4E3CB6EA-3819-4F31-AD44-E7CEA0D4C125", $"NavHelper NGo current_checking_distance_iteration = {current_checking_distance_iteration}");
#endif

                if (current_checking_distance_iteration > CHECKING_DISTANCE_ITERATINONS_COUNT)
                {
#if DEBUG
                    logger.Info("244CA323-2C93-48E4-B19C-D1B9C4DE2596", "NavHelper NGo current_checking_distance_iteration > CHECKING_DISTANCE_ITERATINONS_COUNT");
#endif

                    current_checking_distance_iteration = 0;

                    if (oldPosition.HasValue)
                    {
                        var delta = Vector3.Distance(position, oldPosition.Value);

#if DEBUG
                        logger.Info("A27F0494-5F44-4A95-B19A-53F8118CD3A7", $"NavHelper NGo delta = {delta}");
#endif

                        if (delta < DISTANCE_DELTA_THRESHOLD)
                        {
#if DEBUG
                            logger.Info("5704397F-F891-4E99-B144-9FF0DB081BF8", "NavHelper NGo delta < 0.1");
#endif

                            var distanceBetweenTarget = Vector3.Distance(position, targetPosition);

#if DEBUG
                            logger.Info("B5155A03-0336-44A0-9308-D9AF80DD514C", $"NavHelper NGo distanceBetweenTarget = {distanceBetweenTarget}");
#endif

                            if (distanceBetweenTarget > DISTANCE_BETWEEN_TARGET_THRESHOLD)
                            {
#if DEBUG
                                logger.Info("D4A9C912-EE31-47C5-8027-BE94B4CA62CB", "NavHelper NGo distanceBetweenTarget > DISTANCE_BETWEEN_TARGET_THRESHOLD");
#endif

                                return new GoResult() { GoStatus = GoStatus.BrokenByObsticle };
                            }

#if DEBUG
                            logger.Info("71ED2072-CEA6-4F6C-880F-BBC66637E0C0", "NavHelper NGo return new GoResult() { GoStatus = GoStatus.Success }");
#endif

                            return new GoResult() { GoStatus = GoStatus.Success };
                        }
                    }

                    oldPosition = position;
                }

                if (cancellationToken.IsCancellationRequested)
                {
#if DEBUG
                    logger.Info("78F06369-649F-4850-BA8D-504F3DE23637", "NavHelper NGo cancellationToken.IsCancellationRequested");
#endif

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
