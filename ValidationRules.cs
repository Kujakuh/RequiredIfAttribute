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

        public static bool TestOut(Student st)
        {
            Console.WriteLine($"{st.StudentId} id, with name: {st.Name}"); return true;
        }

        public static bool TestOutGeneric(object st)
        {
            if (st is Student std)
            {
                Console.WriteLine($"{std.StudentId} id, with name: {std.Name}");
                return true;
            }
            else if (st is Person per)
            {
                Console.WriteLine($"Given person with name: {per.Name} is not a Student");
            }
            return false;
        }
    }
}
