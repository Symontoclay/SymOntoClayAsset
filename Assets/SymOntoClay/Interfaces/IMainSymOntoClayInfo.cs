using SymOntoClay.UnityAsset.Scriptables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SymOntoClay.Interfaces
{
    public interface IMainSymOntoClayInfo
    {
        SobjFile SobjFile { get; set; }
        string Id { get; set; }
        string IdForFacts { get; set; }
        string Name { get; }
        string OldName { get; set; }
    }
}
