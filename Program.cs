using System.ComponentModel.DataAnnotations;

namespace RequiredIf
{
    class Program
    {
        static void Main(string[] args)
        {
            // Scenario 0: Condition met, Surname is null, Name is null (validation should fail)
            Test test0 = new Test();
            //ValidateModel(test0);

            // Scenario 1: Condition met, Surname is empty, Name is null (validation should fail)
            Test test1 = new Test { Surname = "" };
            Test t1 = new Test { Name = "         ", Surname = "        "};
            //ValidateModel(test1);

            //// Scenario 2: Condition met, Surname is empty, Name is set (validation should pass)
            //Test test2 = new Test
            //{
            //    Surname = "",
            //    Name = "John"
            //};
            //ValidateModel(test2);

            //// Scenario 3: Condition met, Surname is not empty, Name is null (validation should pass)
            //Test test3 = new Test
            //{
            //    Surname = "Doe"
            //};
            //ValidateModel(test3);

            //// Scenario 4: Condition met, Surname is not empty, Name is set (validation should pass)
            //Test test4 = new Test
            //{
            //    Surname = "Doe",
            //    Name = "John"
            //};
            //ValidateModel(test4);

            //// Scenario 5: Condition not met, Surname is empty, Name is null (validation should pass)
            //TestWithoutCondition test5 = new TestWithoutCondition { Surname = "" };
            //ValidateModel(test5);

            //// Scenario 6: Condition not met, Surname is empty, Name is set (validation should pass)
            //TestWithoutCondition test6 = new TestWithoutCondition
            //{
            //    Surname = "",
            //    Name = "John"
            //};
            //ValidateModel(test6);

            //// Scenario 7: Condition not met, Surname is not empty, Name is null (validation should pass)
            //TestWithoutCondition test7 = new TestWithoutCondition
            //{
            //    Surname = "Doe"
            //};
            //ValidateModel(test7);

            //// Scenario 8: Condition not met, Surname is not empty, Name is set (validation should pass)
            //TestWithoutCondition test8 = new TestWithoutCondition
            //{
            //    Surname = "Doe",
            //    Name = "John"
            //};
            //ValidateModel(test8);

            t1.ValidateSelf();
            List<ValidationResult> enumerable = t1.GetErrors().ToList();
            foreach (var value in enumerable) Console.WriteLine(value.ErrorMessage);

            //Test test = new Test
            //{
            //    Surname = "", // Inválido
            //    Name = null    // Inválido porque Surname está vacío
            //};

            //// Validar el modelo
            //test.ValidateSelf();

            //// Verificar errores
            //if (test.HasErrors)
            //{
            //    foreach (var error in test.GetErrors(null))
            //    {
            //        Console.WriteLine($"Error: {error}");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("All validations passed.");
            //}

            //List<ValidationResult> validationResults = test.ValidateSelf2().ToList();

            //foreach (var validationResult in validationResults) Console.WriteLine($": {validationResult.ErrorMessage}");


            //Console.WriteLine("Validation completed.");
        }


        private static void ValidateModel(object model)
        {
            Console.WriteLine($"Validating model: {model.GetType().Name}");

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(model, context, results, true);

            if (isValid)
            {
                Console.WriteLine("Model is valid.");
            }
            else
            {
                Console.WriteLine("Model validation failed.");
                foreach (var validationResult in results)
                {
                    Console.WriteLine($" - {validationResult.ErrorMessage}");
                }
            }

            Console.WriteLine();
        }
    }

    public class TestWithoutCondition : Test
    {
        private bool isSurnameEmpty() => false;
    }

}
