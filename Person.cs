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
        [RequiredIf(nameof(ValidationRules.IsSurnameEmpty), typeof(ValidationRules), parameters: [], AllowEmptyStrings = false)]
        private string? _name;

        [ObservableProperty]
        [Required]
        private string? _surname;

        private bool IsSurnameEmpty(object instance)
        {
            return string.IsNullOrWhiteSpace(Surname);
        }
        private bool flag() => true;

        public void ValidateSelf() => ValidateAllProperties();
    }
}
