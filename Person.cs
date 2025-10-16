using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RequiredIf
{
    public partial class Person : ObservableValidator
    {
        [ObservableProperty]
        //[RequiredIf("IsSurnameEmpty", parameters: [1], AllowEmptyStrings = true)]
        [RequiredIf("IsSurnameEmpty2", AllowEmptyStrings = false)]
        private string? _name;

        [ObservableProperty]
        [Required]
        private string? _surname;

        public bool IsSurnameEmpty(Person per, int? mode)
        {
            if (mode == 1) return true;
            return false;
        }

        public bool IsSurnameEmpty2()
        {
            return true;
}

        public void ValidateSelf() => ValidateAllProperties();
    }
}
