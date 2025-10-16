using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RequiredIf
{
    public partial class Student : Person
    {
        [ObservableProperty]
        //[RequiredIf<ValidationRules>(nameof(ValidationRules.IsSurnameEmptyEXT), parameters: [1])]
        //[RequiredIf<ValidationRules>("IsSurnameEmptyEXT", parameters: [1])]
        //[RequiredIf<ValidationRules>("TestOut")]
        [RequiredIf<ValidationRules>(nameof(ValidationRules.TestOutGeneric))]
        //[RequiredIf("IsSurnameEmpty", parameters: [1], AllowEmptyStrings = true)]
        //[RequiredIf(nameof(IsSurnameEmpty2))]
        string? _studentId;
    }
}
