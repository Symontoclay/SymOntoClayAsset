using SymOntoClay.UnityAsset.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SymOntoClay.Helpers
{
    public class GoResult: IGoResult
    {
        public GoStatus GoStatus { get; set; }
    }
}
