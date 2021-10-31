using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.Helpers
{
    public abstract class CoroutineObjectBase
    {
        public MonoBehaviour Owner { get; protected set; }
        public Coroutine Coroutine { get; protected set; }

        public bool IsProcessing => Coroutine != null;

        public abstract event Action Finished;
    }
}
