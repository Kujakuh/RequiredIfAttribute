using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequiredIf
{
    class ValidationRules
    {
        public static bool IsSurnameEmpty(object instace)
        {
            if (instace is Person test)
            {
                return string.IsNullOrWhiteSpace(test.Surname);
            }

            return false;
        }
    }
}
