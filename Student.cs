using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RequiredIf
{
    partial class Student : Person
    {
        [ObservableProperty]
        [RequiredIf(nameof(IsStudentIdEmpty), AllowEmptyStrings = false)]
        string? _studentId;

        public bool IsStudentIdEmpty(object instance)
        {
            return string.IsNullOrWhiteSpace(StudentId) && string.IsNullOrWhiteSpace(Name);
        }
    }
}
