using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamplesOfSymOntoClay
{
    public interface IHandThing
    {
        KindOfHandThing Kind { get; }

        string IdForFacts { get; }
        IEntityLogger Logger { get; }
    }
}
