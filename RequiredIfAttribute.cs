// Created By: Álvaro Vos Graciá
// Contact: avos202@gmail.com

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RequiredIf
{
    /// <summary>
    /// Attribute used to validate that a property is required if the provided method returns true.
    /// The validation method can have different signatures and be located either in the same class or in an external validator class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <strong>Method Signature Requirements:</strong>
    /// <list type="bullet">
    /// <item>Must return a <see cref="bool"/> indicating whether the property is required.</item>
    /// <item>Can accept: no parameters, typed parameter, or object parameter.</item>
    /// <item>If in external class, must be static.</item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Location Rules:</strong>
    /// <list type="bullet">
    /// <item><strong>Same class:</strong> <c>[RequiredIf("MethodName")]</c> - Can be instance or static</item>
    /// <item><strong>External class:</strong> <c>[RequiredIf&lt;ValidatorClass&gt;("MethodName")]</c> - MUST be static</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <collapsible>
    /// <summary>Usage Examples</summary>
    /// <para>
    /// <strong>CASE 1: Method in the same class, parameterless</strong>
    /// <code>
    /// class Product
    /// {
    ///     [RequiredIf("HasStock")]
    ///     public string? Details { get; set; }
    ///     
    ///     public int Stock { get; set; }
    ///     
    ///     public bool HasStock() => Stock > 0;
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// <strong>CASE 2: Method in the same class, with typed parameter (static)</strong>
    /// <code>
    /// class Product
    /// {
    ///     [RequiredIf("IsNameProvided")]
    ///     public string? Description { get; set; }
    ///     
    ///     public string? Name { get; set; }
    ///     
    ///     public static bool IsNameProvided(Product product) 
    ///         => !string.IsNullOrEmpty(product.Name);
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// <strong>CASE 3: Method in external class (MUST use generic syntax)</strong>
    /// <code>
    /// class Person
    /// {
    ///     [RequiredIf&lt;ValidationMethods&gt;("IsSurnameEmpty")]
    ///     public string? FirstName { get; set; }
    ///     
    ///     public string? Surname { get; set; }
    /// }
    /// 
    /// static class ValidationMethods
    /// {
    ///     public static bool IsSurnameEmpty(Person person) 
    ///         => string.IsNullOrWhiteSpace(person.Surname);
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// <strong>CASE 4: Using inherited type with parent's validation method (same class)</strong>
    /// <code>
    /// class Person
    /// {
    ///     [RequiredIf&lt;Person&gt;("HasValidName")]
    ///     public string? Email { get; set; }
    ///     
    ///     public string? Name { get; set; }
    ///     
    ///     public static bool HasValidName(Person person) 
    ///         => !string.IsNullOrEmpty(person.Name);
    /// }
    /// 
    /// class Student : Person
    /// {
    ///     public string? StudentId { get; set; }
    ///     // Inherits the HasValidName validation method from Person
    /// }
    /// 
    /// // Student instance will use Person's HasValidName method automatically
    /// var student = new Student { Name = "John", Email = null };
    /// // Email validation works because Student is assignable from Person
    /// </code>
    /// </para>
    /// <para>
    /// <strong>CASE 5: Method with additional parameters</strong>
    /// <code>
    /// class Person
    /// {
    ///     [ObservableProperty]
    ///     [RequiredIf("IsSurnameEmpty", parameters: [1])]
    ///     private string? _name;
    ///     
    ///     [ObservableProperty]
    ///     private string? _surname;
    ///     
    ///     // Method accepts additional parameters beyond the instance
    ///     public bool IsSurnameEmpty(Person per, int mode)
    ///     {
    ///         if (mode == 1) return string.IsNullOrEmpty(Surname);
    ///         return false;
    ///     }
    /// }
    /// 
    /// class Student : Person
    /// {
    ///     [ObservableProperty]
    ///     [RequiredIf&lt;Person&gt;(nameof(IsSurnameEmpty))]
    ///     string? _studentId;
    ///     
    ///     // Inherits IsSurnameEmpty from Person
    ///     // The parameters passed to Person's attribute are inherited too
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// <strong>CASE 6: Using inherited type with external validator and parameters</strong>
    /// <code>
    /// class Person
    /// {
    ///     [RequiredIf&lt;ValidationRules&gt;("IsNameValid", parameters: [true])]
    ///     public string? Email { get; set; }
    ///     
    ///     public string? Name { get; set; }
    /// }
    /// 
    /// class Student : Person
    /// {
    ///     public string? StudentId { get; set; }
    /// }
    /// 
    /// static class ValidationRules
    /// {
    ///     public static bool IsNameValid(Student student, bool checkEmpty) 
    ///         => checkEmpty ? !string.IsNullOrEmpty(student.Name) : true;
    /// }
    /// </code>
    /// </para>
    /// <code>
    /// class Person
    /// {
    ///     [RequiredIf&lt;ValidationRules&gt;("IsNameValid")]
    ///     public string? Email { get; set; }
    ///     
    ///     public string? Name { get; set; }
    /// }
    /// 
    /// class Student : Person
    /// {
    ///     public string? StudentId { get; set; }
    /// }
    /// 
    /// static class ValidationRules
    /// {
    ///     // Method accepts Student even though [RequiredIf&lt;Person&gt;] was declared
    ///     // Works because Student : Person (Student is assignable from Person)
    ///     public static bool IsNameValid(Student student) 
    ///         => !string.IsNullOrEmpty(student.Name);
    /// }
    /// </code>
    /// </para>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        /// <summary>
        /// The name of the method used for validation.
        /// </summary>
        protected readonly string MethodName;

        /// <summary>
        /// The type that declares the validation method. If null, the type of the validating instance is used.
        /// </summary>
        protected readonly Type? ValidatorType;

        /// <summary>
        /// Additional arguments to pass to the validation method, if needed.
        /// </summary>
        protected readonly object[] Parameters;

        /// <summary>
        /// Indicates whether empty strings are considered valid.
        /// </summary>
        /// <value>Defaults to <c>false</c>, meaning empty strings are invalid if the property is required.</value>
        public bool AllowEmptyStrings { get; set; } = false;

        /// <param name="methodName">The name of the validation method to be invoked.</param>
        /// <param name="parameters">Optional parameters to pass to the validation method.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="methodName"/> is null or empty.</exception>
        /// <remarks>
        /// When no validator type is specified, the validation method is looked up in the class of the instance being validated.
        /// The method can be instance-based (parameterless) or static (with typed or object parameter).
        /// </remarks>
        public RequiredIfAttribute(string methodName, params object[] parameters)
        {
            MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            ValidatorType = null;
            Parameters = ExtractParameters(parameters);
        }

        /// <param name="methodName">The name of the validation method to be invoked.</param>
        /// <param name="validatorType">The external type that declares the validation method. This method MUST be static.</param>
        /// <param name="parameters">Optional parameters to pass to the validation method.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="methodName"/> is null or empty.</exception>
        /// <remarks>
        /// Use this constructor when the validation method is in an external class.
        /// The method in the external class MUST be static and accept an appropriate parameter (the instance being validated or object).
        /// </remarks>
        public RequiredIfAttribute(string methodName, Type validatorType, params object[] parameters)
        {
            MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            ValidatorType = validatorType;
            Parameters = ExtractParameters(parameters);
        }

        /// <summary>
        /// Extracts the actual parameters, handling the case where params array contains only the array itself.
        /// </summary>
        private static object[] ExtractParameters(object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return [];

            // Handle case where user passes object[] directly via parameters named argument
            if (parameters.Length == 1 && parameters[0] is object[] arrayParam)
                return arrayParam;

            return parameters;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            object? validationInstance = validationContext.ObjectInstance;

            // Use the provided validator type or default to the instance type
            Type validatorType = ValidatorType ?? validationInstance?.GetType()
                ?? throw new InvalidOperationException("The validator type could not be determined.");

            // Find the validation method with the most compatible signature
            MethodInfo? method = FindValidationMethod(validatorType, validationInstance);

            if (method == null)
            {
                throw new InvalidOperationException(
                    $"Method '{MethodName}' not found in type '{validatorType.FullName}' with a compatible signature.");
            }

            if (method.ReturnType != typeof(bool))
            {
                throw new InvalidOperationException($"Method '{MethodName}' must return a boolean value.");
            }

            // Build method arguments based on method signature
            object[] methodArguments = BuildMethodArguments(method, validationInstance, validatorType);

            bool isConditionMet = (bool)method.Invoke(validationInstance, methodArguments)!;

            if (isConditionMet)
            {
                if (value is null)
                {
                    return new ValidationResult(
                        ErrorMessage ?? $"The {validationContext.DisplayName} field is required.",
                        [validationContext.DisplayName]);
                }

                if (!AllowEmptyStrings && value is string stringValue && string.IsNullOrWhiteSpace(stringValue))
                {
                    return new ValidationResult(
                        ErrorMessage ?? $"The {validationContext.DisplayName} field cannot be empty or whitespace.",
                        [validationContext.DisplayName]);
                }
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Finds the validation method with the most compatible signature.
        /// Supports inheritance: if method is not found in the validator type, searches in base classes.
        /// Priority: Parameterless > Typed parameter matching validator type > Typed parameter in inheritance hierarchy > Object parameter > Custom typed parameter
        /// </summary>
        protected MethodInfo? FindValidationMethod(Type validatorType, object? validationInstance)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var methods = validatorType.GetMethods(bindingFlags)
                .Where(m => m.Name == MethodName)
                .ToList();

            if (methods.Count == 0)
                return null;

            // Priority 1: Parameterless method
            var parameterless = methods.FirstOrDefault(m => m.GetParameters().Length == 0);
            if (parameterless != null)
                return parameterless;

            // Priority 2: Method with parameter matching the validator type exactly
            var typedForValidator = methods.FirstOrDefault(m =>
                m.GetParameters().Length == 1 + Parameters.Length &&
                m.GetParameters()[0].ParameterType == validatorType);
            if (typedForValidator != null)
                return typedForValidator;

            // Priority 3: Method with parameter in inheritance hierarchy
            var typedInHierarchy = methods.FirstOrDefault(m =>
                m.GetParameters().Length == 1 + Parameters.Length &&
                m.GetParameters()[0].ParameterType.IsAssignableFrom(validatorType));
            if (typedInHierarchy != null)
                return typedInHierarchy;

            // Priority 4: Method with parameter matching the validation instance type
            var typedForInstance = methods.FirstOrDefault(m =>
                m.GetParameters().Length == 1 + Parameters.Length &&
                m.GetParameters()[0].ParameterType.IsInstanceOfType(validationInstance));
            if (typedForInstance != null)
                return typedForInstance;

            // Priority 5: Method with object parameter
            var objectParam = methods.FirstOrDefault(m =>
                m.GetParameters().Length == 1 + Parameters.Length &&
                m.GetParameters()[0].ParameterType == typeof(object));
            if (objectParam != null)
                return objectParam;

            // Priority 6: Any method with compatible parameter count
            return methods.FirstOrDefault(m => m.GetParameters().Length == 1 + Parameters.Length);
        }

        /// <summary>
        /// Builds the method arguments based on the method's signature.
        /// Handles the instance parameter plus any additional parameters.
        /// </summary>
        protected object[] BuildMethodArguments(MethodInfo method, object? validationInstance, Type validatorType)
        {
            var methodParams = method.GetParameters();

            // Parameterless method
            if (methodParams.Length == 0)
                return [];

            // Build arguments: first is the instance, rest are additional parameters
            object[] args = new object[methodParams.Length];

            // First parameter: the validation instance (converted if necessary)
            Type firstParamType = methodParams[0].ParameterType;

            if (firstParamType == typeof(object) || firstParamType.IsInstanceOfType(validationInstance))
            {
                args[0] = validationInstance!;
            }
            else if (firstParamType == validatorType)
            {
                args[0] = validationInstance!;
            }
            else if (firstParamType.IsAssignableFrom(validatorType))
            {
                args[0] = validationInstance!;
            }
            else
            {
                // Try to convert if possible
                try
                {
                    args[0] = Convert.ChangeType(validationInstance, firstParamType);
                }
                catch
                {
                    args[0] = validationInstance!;
                }
            }

            // Copy additional parameters starting from index 1
            for (int i = 1; i < methodParams.Length && i - 1 < Parameters.Length; i++)
            {
                args[i] = Parameters[i - 1];
            }

            return args;
        }
    }

    /// <summary>
    /// Generic version of RequiredIfAttribute that allows specifying the validator type using generic syntax.
    /// Use this when the validation method is in an external class (which must be static).
    /// <para>
    /// <strong>Usage:</strong>
    /// <code>
    /// [RequiredIf&lt;ExternalValidatorClass&gt;("MethodName")]
    /// public string? PropertyName { get; set; }
    /// </code>
    /// </para>
    /// <para>
    /// The validator class method must be static and accept the appropriate type parameter.
    /// </para>
    /// </summary>
    /// <typeparam name="TValidator">The external validator type that declares the validation method. The method must be static.</typeparam>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class RequiredIfAttribute<TValidator> : RequiredIfAttribute
    {
        /// <param name="methodName">The name of the validation method to be invoked.</param>
        /// <param name="parameters">Optional parameters to pass to the validation method.</param>
        public RequiredIfAttribute(string methodName, params object[] parameters)
            : base(methodName, typeof(TValidator), parameters)
        {
        }
    }
}