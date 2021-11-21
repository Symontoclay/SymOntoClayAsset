using SymOntoClay;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamplesOfSymOntoClay
{
    public interface IHandThing: IUHandThingHost
    {
        KindOfHandThing Kind { get; }

        string IdForFacts { get; }
        IEntityLogger Logger { get; }
        IUSocGameObject USocGameObject { get; }
    }
}
