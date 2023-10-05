using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Samles.Interfaces
{
    public interface IRifleCustomBehavior: IHandThingCustomBehavior
    {
        GameObject MainWP { get; }
        GameObject AddWP { get; }

        bool SetToHandsOfHumanoid(IMonitorLogger logger, IBipedHumanoidCustomBehavior humanoid);

        void LookAt(IMonitorLogger logger);
        void LookAt(IMonitorLogger logger, GameObject target);
        void LookAt(IMonitorLogger logger, Transform target);
        void LookAt(IMonitorLogger logger, Vector3? target);
        bool ThrowOut(IMonitorLogger logger);
        void StartFire(CancellationToken cancellationToken, IMonitorLogger logger);
        void StopFire(IMonitorLogger logger);
    }
}
