using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarvarldDB
{

    public class affclass
    {
        public int id = 0;
        public string name = "";
        public int orgtype = 0; 
        public int partof = -1;
        public int aliasof = -1;

        public const int sweuni = 1;
        public const int otheruni = 2;
        public const int other = 3;
        public const int unknown = 4;
        public const int reslab = 5;
    }
}
