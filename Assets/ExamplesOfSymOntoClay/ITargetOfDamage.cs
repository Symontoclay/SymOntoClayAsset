using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExamplesOfSymOntoClay
{
    public interface ITargetOfDamage
    {
        void SetHit(RaycastHit shootHit, int damagePerShot);
    }
}
