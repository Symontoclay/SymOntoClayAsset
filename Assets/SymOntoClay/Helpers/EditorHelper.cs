using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Helpers
{
    public static class EditorHelper
    {
        public static bool IsValidId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            if (!id.StartsWith("#"))
            {
                return false;
            }

            if (UniqueIdRegistry.ContainsId(id))
            {
                return false;
            }

            return true;
        }
    }
}
