using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequiredIf
{
    class ValidationRules
    {
        public static bool IsSurnameEmptyEXT(Person per, int mode)
        {
            if (mode == 1) return true;
            return false;
        }
    }
}
