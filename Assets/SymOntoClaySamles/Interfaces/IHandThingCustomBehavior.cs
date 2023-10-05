using SymOntoClay;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
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
    public interface IHandThingCustomBehavior: IHandThingHostBehavior
    {
        KindOfHandThing Kind { get; }

        string IdForFacts { get; }
        IMonitorLogger Logger { get; }
        IGameObjectBehavior USocGameObject { get; }
    }
}
