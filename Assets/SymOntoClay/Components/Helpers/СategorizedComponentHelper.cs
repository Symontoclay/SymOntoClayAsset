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
            if (target.Categories == null)
            {
                target.Categories = new List<string>(target.DefaultCategories);
            }

            if (!target.Categories.Any())
            {
                target.Categories.AddRange(target.DefaultCategories);
            }
        }
    }
}
