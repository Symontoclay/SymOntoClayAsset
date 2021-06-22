using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay
{
    public interface IUHumanoidNPC
    {
        IHumanoidNPC NPC { get; }
        string IdForFacts { get; }
        void Die();
    }
}
