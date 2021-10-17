using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExamplesOfSymOntoClay
{
    public interface IUTwoHandGun
    {
        GameObject MainWP { get; }
        GameObject AddWP { get; }

        bool SetToHandsOfHumanoid(IUBipedHumanoid humanoid);
        void LookAt(Transform target);
    }
}
