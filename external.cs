using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequiredIf
{
    class External
    {
        public static bool IsSurnameEmpty(object instace)
        {
            if(instace is Test test)
                return string.IsNullOrWhiteSpace(test.Surname);
            return false;
        }
    }
}
