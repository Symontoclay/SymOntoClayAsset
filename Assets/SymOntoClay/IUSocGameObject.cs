using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay
{
    public interface IUSocGameObject
    {
        string IdForFacts { get; }
        IEntityLogger Logger { get; }
        void RunInMainThread(Action function);
        TResult RunInMainThread<TResult>(Func<TResult> function);
        string InsertPublicFact(string text);
        void RemovePublicFact(string id);
        void PushSoundFact(float power, string text);
    }
}
