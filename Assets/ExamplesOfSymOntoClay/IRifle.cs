﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ExamplesOfSymOntoClay
{
    public interface IRifle: IHandThing
    {
        GameObject MainWP { get; }
        GameObject AddWP { get; }

        bool SetToHandsOfHumanoid(IUBipedHumanoid humanoid);

        void LookAt();
        void LookAt(GameObject target);
        void LookAt(Transform target);
        void LookAt(Vector3? target);
        bool ThrowOut();
        void StartFire(CancellationToken cancellationToken);
        void StopFire();
    }
}
