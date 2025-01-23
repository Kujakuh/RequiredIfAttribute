// Created By: Álvaro Vos Graciá
// Contact: avos202@gmail.com

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RequiredIf
{
    /// <summary>
    /// An attribute used to validate that a property is required if the provided method returns true.
    /// Keep in mind that any paramter used on the method provided must be a static member.
    /// <remarks>
    /// <para>
    /// The validation method specified must adhere to the following requirements:
    /// <list type="bullet">
    /// <item>
    ///     It must return a <see cref="bool"/> indicating whether the property is required.
    /// </item>
    /// <item>
    ///     It must accept an <see cref="object"/> as its first parameter, representing the validation context object.
    /// </item>
    /// <item>
    ///     If declared in an external class, the method must be static.
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <para>
    /// The following demonstrates typical usage of the <see cref="RequiredIfAttribute"/>:
    /// <code>
    /// 
    /// // Conditional validation within the same class
    /// 
    /// [ObservableProperty]
    /// [RequiredIf("IsInStock")]
    /// public string? _linkedInventory;
    /// 
    /// // Conditional validation using an external method
    /// 
    /// [ObservableProperty]
    /// [RequiredIf(nameof(ValidationMethods.IsSurnameEmpty), typeof(ValidationMethods))]
    /// public string? _name;
    /// 
    /// </code>
    /// </para>
    /// <para>
    /// In the first example, the `LinkedInventory` property is required if the `IsInStock` method in the same class returns true.
    /// </para>
    /// <para>
    /// In the second example, the `Name` property is required if the external static method `IsSurnameEmpty` returns true.
    /// </para>
    /// <para>
    /// Example methods used in the above code:
    /// <code>
    /// 
    /// class ValidationMethods
    /// {
    ///     public static bool IsSurnameEmpty(object instance)
    ///     {
    ///         if (instance is Person person)
    ///             return string.IsNullOrWhiteSpace(person.Surname);
    ///         return false;
    ///     }
    /// }
    /// 
    /// 
    /// class Person
    /// {
    ///     private bool IsSurnameEmpty(object instance)
    ///     {
    ///         return string.IsNullOrWhiteSpace(Surname);
    ///     }
    /// }
    /// 
    /// </code>
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class RequiredIfAttribute : ValidationAttribute
    {
        /// <summary>
        /// The name of the method used for validation.
        /// </summary>
        private readonly string _methodName;

        /// <summary>
        /// The type that declares the validation method. If null, the method is expected to exist in the same class.
        /// </summary>
        private readonly Type? _declaringType;

        /// <summary>
        /// Additional arguments to pass to the validation method, if needed.
        /// </summary>
        private readonly object[] _parameters;

        /// <summary>
        /// Indicates whether empty strings are considered valid.
        /// </summary>
        /// <value>Defaults to <c>false</c>, meaning empty strings are invalid if the property is required.</value>
        public bool AllowEmptyStrings { get; set; } = false;


        /// <param name="methodName">The name of the validation method to be invoked.</param>
        /// <param name="declaringType">
        /// The type declaring the method. This parameter is optional and can be omitted if the method is in the same class.
        /// </param>
        /// <param name="parameters">Optional parameters to pass to the validation method.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="methodName"/> is null or empty.</exception>
        public RequiredIfAttribute(string methodName, Type? declaringType = null, params object[] parameters)
        {
            _methodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            _declaringType = declaringType;
            _parameters = parameters ?? [];
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            Type declaringType = _declaringType ?? validationContext.ObjectInstance?.GetType()
                ?? throw new InvalidOperationException("The declaring type could not be determined.");

            MethodInfo method = declaringType.GetMethod(_methodName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException($"Method '{_methodName}' not found in type '{declaringType.FullName}'.");

            if (method.ReturnType != typeof(bool))
            {
                throw new InvalidOperationException($"Method '{_methodName}' must return a boolean value.");
            }

            object[] methodArguments = new object[_parameters.Length + 1];
            methodArguments[0] = validationContext.ObjectInstance;
            Array.Copy(_parameters, 0, methodArguments, 1, _parameters.Length);

            bool isConditionMet = (bool) method.Invoke(validationContext.ObjectInstance, methodArguments);

            if (isConditionMet)
            {
                if (value is null)
                {
                    return new ValidationResult(ErrorMessage ?? $"The {validationContext.DisplayName} field is required.");
                }

                if (!AllowEmptyStrings && value is string stringValue && string.IsNullOrWhiteSpace(stringValue))
                {
                    return new ValidationResult(ErrorMessage ?? $"The {validationContext.DisplayName} field cannot be empty or whitespace.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
