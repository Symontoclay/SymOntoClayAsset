﻿using SymOntoClay.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay
{
    [AddComponentMenu("SymOntoClay/HandThing")]
    public class HandThing : BaseThing
    {
        protected override bool CanBeTakenBy(IEntity subject)
        {
            throw new NotImplementedException();
        }
    }
}
