using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarvarldDB
{
    public class authorclass
    {
        public string familyname = "";
        public string firstname = "";
        public List<affclass> aff = new List<affclass>();
        public List<string> pubs = new List<string>(); 

    }
}
