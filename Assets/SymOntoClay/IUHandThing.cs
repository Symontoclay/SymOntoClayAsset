using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay
{
    /// <summary>
    /// Represents SymOntoClay's thing component which can be taken by NPC's hands.
    /// </summary>
    public interface IUHandThing: IUSocGameObject
    {
        /// <summary>
        /// Hides the thing as part of placing in backpack.
        /// This method should be called only in main thread.
        /// </summary>
        void HideForBackpackInMainThread();

        /// <summary>
        /// Hides the thing as part of placing in backpack.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        void HideForBackpackInUsualThread();
    }
}
