using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Samles.Interfaces
{
    public interface ITargetOfDamageCustomBehavior
    {
        void SetHit(IMonitorLogger logger, RaycastHit shootHit, int damagePerShot);
    }
}
