using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SymOntoClay.Interfaces
{
    public interface IСategorized
    {
        List<string> DefaultCategories { get; }
        List<string> Categories { get; set; }
        bool EnableCategories { get; set; }
    }
}
