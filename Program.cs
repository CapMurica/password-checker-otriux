using System;
using System.Linq;
using System.Net.Http.Headers;


//Created by Otriux


namespace PasswordCheckerNEA
{
    class Program
    {
        //Initialisation of static variables
        private static readonly char[] Symbols = { '!', '$', '%', '^', '&', '(', ')', '-', '_', '=', '+' };

        // As per the PDF requirement, each row should be checked independently
        private static readonly string KeyboardRow1 = "QWERTYUIOP";
        private static readonly string KeyboardRow2 = "ASDFGHJKL";
        private static readonly string KeyboardRow3 = "ZXCVBNM";
        private static readonly string UpperCaseCharacters = KeyboardRow1 + KeyboardRow2 + KeyboardRow3;
        private static readonly string LowerCaseCharacters = UpperCaseCharacters.ToLower();

        static void Main(string[] args)
        {
            //Call the Menu method
            Menu();
        }

        static void Menu()
        {

            // Multiline strings are supported by using @
            Console.WriteLine(@"Welcome to the Password Checker!" +
                            "************************************************************************************");

            //Declare a bool reenterChoice with the value of false, so that the while loop run  while reenterChoice is false
            bool reenterChoice = true;
            while (reenterChoice)
            {
                Console.WriteLine("Select an operation to perform:");
                Console.WriteLine("[1] Check Password");
                Console.WriteLine("[2] Generate Password");
                Console.WriteLine("[3] Quit");
                // A try and catch helps to capture the exception, instead of crashing the program
                try
                {
                    // Cap Murica: Use TryParse. It will reject any input apart from an integer automatically
                    // Cap Murica: This will allow you to return specific message for integer input or other input.
                    if (Int32.TryParse(Console.ReadLine(), out int option))
                    {
                        // Cap Murica: Using a switch instead of multiple if statements, switch is better for choices.
                        switch (option)
                        {
                            case 1:
                                {
                                    // Call the CheckPassword method
                                    reenterChoice = CheckPassword();
                                    break;
                                }
                            case 2:
                                {
                                    // Call the GeneratePassword method
                                    GeneratePassword();
                                    reenterChoice = false;
                                    break;
                                }
                            case 3:
                                {
                                    // Quit the program
                                    Console.WriteLine("You have decided to exit the Program.");
                                    Environment.Exit(0);
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine("The option entered is not valid. Please enter a valid option!");
                                    reenterChoice = true;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Input is not a valid number. Please provide a valid option!");
                        reenterChoice = true;
                    }

                }
                catch (Exception e)
                {
                    //Catches any exceptions thrown and displays an error message
                    //You can also append e.Message to see the main error
                    Console.WriteLine($"Something went wrong. Error Message: { e.Message }");
                    reenterChoice = false;
                }
            }
        }

        static bool CheckPassword()
        {
            string symbolString = new string(Symbols);
            Console.WriteLine(@"Enter a Password. A valid password contains 8-24 characters, atleast one UPPERCASE character, atleast one LOWERCASE character, atleast one number and atleast one of the special characters in '" + symbolString + "'");
            string passCheck = Console.ReadLine();
            //if the user did not enter a password
            // Cap Murica: Use string.IsNullOrEmpty() to check a null or empty string
            if (string.IsNullOrEmpty(passCheck))
            {
                Console.WriteLine("You did not enter a Password to check");
                return true;
            }
            if (passCheck.Length < 8 || passCheck.Length > 24)
            {
                //checks for the length of the Password
                Console.WriteLine("Your password must be between 8-24 characters long");
                return true;
            }

            bool symbolPresent = (passCheck.IndexOfAny(Symbols) >= 0);
            bool upperCharPresent = passCheck.Any(ch1 => char.IsUpper(ch1));
            bool lowerCharPresent = passCheck.Any(ch1 => char.IsLower(ch1));
            bool digitPresent = passCheck.Any(ch1 => char.IsDigit(ch1));

            if (!symbolPresent && !upperCharPresent && !digitPresent)
            {
                Console.WriteLine("The password doesn't meet the required specifications.");
                return true;
            }
            //Here we pass in the password into the CalculatePoints as a parameter so we can check how many points it gets
            int strength = CalculateStrength(passCheck, symbolPresent, upperCharPresent, lowerCharPresent, digitPresent, out string passwordStrength);

            Console.WriteLine($"Password Strength: { passwordStrength }");
            Console.WriteLine($"Strength Score: { strength }");
            return false;
        }

        //Here, we return an int via the method instead of void
        static int CalculateStrength(string password, bool hasSymbol, bool hasUpperChar, bool hasLowerChar, bool hasDigit, out string passwordStrength)
        {
            //set strength to the length of the password
            int strength = password.Length;

            //checks if there are symbols in the password
            if (hasSymbol)
                strength += 5;

            //checks if any character in the password is Upper, Lower or a Digit
            if (hasUpperChar)
                strength += 5;

            if (hasLowerChar)
                strength += 5;

            if (hasDigit)
                strength += 5;

            //Checks if the password contains all 4
            if (hasSymbol && hasUpperChar && hasLowerChar && hasDigit)
                strength += 10;

            //checks if the password is all digits, upper, lower or symbols
            if (password.All(char.IsDigit))
                strength -= 5;

            if (password.All(char.IsUpper))
                strength -= 5;

            if (password.All(char.IsLower))
                strength -= 5;

            if (password.All(char.IsSymbol) || password.All(char.IsPunctuation))
                strength -= 5;

            for (int i = 0; i < password.Length - 2; i++)
            {
                string upperPassword = password.ToUpper();
                if (KeyboardRow1.Contains(upperPassword.Substring(i, 3)) ||
                    KeyboardRow2.Contains(upperPassword.Substring(i, 3)) ||
                    KeyboardRow3.Contains(upperPassword.Substring(i, 3)))
                {
                    strength -= 5;
                }
            }

            // Set out parameter to strength string
            if (strength > 20)
                passwordStrength = "STRONG";
            else if (strength <= 0)
                passwordStrength = "WEAK";
            else
                passwordStrength = "MEDIUM";

            // Return the final password strength
            return strength;
        }

        static void GeneratePassword()
        {
            // Instatiate a new object from the Random class
            Random rnd = new Random();

            string allCharacters = UpperCaseCharacters + LowerCaseCharacters;

            // Generates the length of the password between 8 and 12 inclusive
            int requiredLength = rnd.Next(8, 24);

            // Our strong password will contain 2 special characters, 2 numbers. Rest will be a mix of upper and lower case letters
            int numberOfCharacters = requiredLength - 4;
            char[] passwordCharacters = new char[requiredLength];

            // Select the required characters
            for (int i = 0; i < numberOfCharacters; i++)
            {
                //loop through each char, and set each char to the one that is generated randomly from the Characters available
                passwordCharacters[i] = allCharacters[rnd.Next(allCharacters.Length - 1)];
            }

            // Select the required numbers
            for (int i = 0; i < 2; i++)
            {
                passwordCharacters[numberOfCharacters + i] = Convert.ToChar(rnd.Next(0, 9).ToString());
            }

            // Select the required symbols
            for (int i = 2; i <= 3; i++)
            {
                passwordCharacters[numberOfCharacters + i] = Symbols[rnd.Next(Symbols.Length - 1)];
            }

            // One final level of randomization and creation of the password
            string generatedPassword = new string(passwordCharacters.OrderBy(s => (rnd.Next(2) % 2) == 0).ToArray());

            // Pass it as a parameter, we know all the specifications are met. Just need to calculate the strength points
            int strength = CalculateStrength(generatedPassword, true, true, true, true, out string passwordStrength);

            // Print the password strength and strength points
            Console.WriteLine($"Generated Password: { generatedPassword }");
            Console.WriteLine($"Password Strength: { passwordStrength }");
            Console.WriteLine($"Strength Score: { strength }");
        }
    }
}
