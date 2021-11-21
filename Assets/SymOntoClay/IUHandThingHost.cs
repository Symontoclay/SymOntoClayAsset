using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay
{
    public interface IUHandThingHost
    {
        void HideForBackpackInMainThread();
        void HideForBackpackInUsualThread();
    }
}
