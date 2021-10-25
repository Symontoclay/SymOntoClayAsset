using SymOntoClay.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay
{
    [AddComponentMenu("SymOntoClay/HandThing")]
    public class HandThing : BaseThing, IUHandThing
    {
        protected override bool CanBeTakenBy(IEntity subject)
        {
#if DEBUG
            UnityEngine.Debug.Log($"HandThing CanBeTakenBy.");
#endif

            //Please! Write here code for controlling too far interaction!

            return _hostListener.CanBeTakenBy(subject);
        }
    }
}
