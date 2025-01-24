using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequiredIf
{
    class ValidationRules
    {
        public static bool IsSurnameEmpty(object instance, int mode)
        {
            if (instance is Person test && mode == 1)
            {
                return string.IsNullOrWhiteSpace(test.Surname);
            }

            // Same as doing:

            Person? per = instance as Person;
            // Same as Person per = (Person) instance; but without the InvalidCastException

            if (per != null) return string.IsNullOrWhiteSpace(per.Surname);

            return false;
        }
    }
}
