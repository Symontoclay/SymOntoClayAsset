using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay
{
    /// <summary>
    /// Provides interaction with user defined scripts/components.
    /// </summary>
    public interface IUHandThingHost
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
