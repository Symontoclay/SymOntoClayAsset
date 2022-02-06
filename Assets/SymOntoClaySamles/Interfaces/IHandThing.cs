﻿using SymOntoClay;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Interfaces;
using SymOntoClay.UnityAsset.Samles.Behavior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Samles.Interfaces
{
    public interface IHandThing: IUHandThingHost
    {
        KindOfHandThing Kind { get; }

        string IdForFacts { get; }
        IEntityLogger Logger { get; }
        IUSocGameObject USocGameObject { get; }
    }
}
