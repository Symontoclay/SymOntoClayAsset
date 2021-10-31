using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExamplesOfSymOntoClay
{
    public class HumanoidHealthController: MonoBehaviour, IHumanoidHealth, ITargetOfDamage
    {
        public void SetHit(RaycastHit shootHit, int damagePerShot)
        {
#if DEBUG
            Debug.Log($"ProcessShoot damagePerShot = {damagePerShot}");
#endif
        }
    }
}
