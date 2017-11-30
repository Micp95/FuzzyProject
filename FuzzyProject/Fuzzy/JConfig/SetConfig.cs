using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyProject.Fuzzy.JConfig
{
    public class SetConfig
    {
        public SetType Type { get; set; }
        public String Name { get; set; }
        public List<float> Values { get; set; }
    }
}
