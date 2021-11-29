using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay
{
    /// <summary>
    /// Represents SymOntoClay's humanoid component.
    /// It can be both NPC and Player.
    /// </summary>
    public interface IUSocHumanoid : IUSocGameObject
    {
        /// <summary>
        /// Performs death of humanoid.
        /// All active processes will have been stopped.
        /// Another NPCs will percept the humanoid as died.
        /// </summary>
        void Die();
    }
}
