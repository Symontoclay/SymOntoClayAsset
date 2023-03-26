using Assets.SymOntoClay.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SymOntoClay.Components.Helpers
{
    public static class СategorizedComponentHelper
    {
        public static void Validate(IСategorized target)
        {
            if (Categories == null)
            {
                Categories = new List<string>(DefaultCategories);
            }

            if (!Categories.Any())
            {
                Categories.AddRange(DefaultCategories);
            }
        }
    }
}
