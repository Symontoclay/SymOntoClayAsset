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

        bool SetToHandsOfHumanoid(IBipedHumanoidCustomBehavior humanoid);

        void LookAt();
        void LookAt(GameObject target);
        void LookAt(Transform target);
        void LookAt(Vector3? target);
        bool ThrowOut();
        void StartFire(CancellationToken cancellationToken);
        void StopFire();
    }
}
