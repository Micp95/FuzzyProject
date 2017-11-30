using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyProject.Fuzzy.JConfig
{
    public class VariableConfig
    {
        public VariableType Type { get; set; }
        public String Name { get; set; }
        public Range Range { get; set; }
        public List<SetConfig> sets { get; set; }
    }
}
