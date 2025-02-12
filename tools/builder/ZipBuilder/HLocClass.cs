using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipBuilder
{
    public class HLocClass
    {
        public string name { get; set; }
        public byte? org_tbyte { get; set; }
        public string value { get; set; }
        public List<HLocClass> children { get; set; }
        public int numChildren { get; set; }
        public string type { get; set; }

    }
}
