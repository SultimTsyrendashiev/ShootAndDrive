using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD
{
    [Serializable]
    class RefInt
    {
        public int Value;
        public RefInt(int a = 0) { Value = a; }
    }
}
