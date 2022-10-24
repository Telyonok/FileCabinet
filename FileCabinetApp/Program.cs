using System.Globalization;
using System.Net.Sockets;

[assembly: CLSCompliant(true)]

namespace FileCabinetApp
{
    /// <summary>
    /// Class <c>Program</c> provides interface and is controlled via commands by user.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Ilya Chvilyov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints records amount", "The 'stat' command prints amount of records" },
            new string[] { "create", "creates a new record", "The 'create' command creates a new note" },
            new string[] { "list", "lists all records", "The 'list' command lists all records" },
            new string[] { "edit", "edits a chosen record", "The 'edit <id>' command edits a record with corresponding Id" },
            new string[] { "find", "finds all records with given parameter value", "The 'find <parameter> \"<value>\"' command finds all records with corresponding value" },
        };

        private static bool isRunning = true;

        private static FileCabinetService fileCabinetService = new (new DefaultValidator());

        /// <summary>
        /// Accepts input and calls corresponding methods.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            ApplyArguments(args);
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine($"Using {fileCabinetService.Validator.GetValidatorName()} validation rules.");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                var inputs = line != null ? line.Split(' ', 2) : new string[] { string.Empty, string.Empty };
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(command, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    Commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            GetUserInputAndRecord(InputMode.Create);
        }

        private static void List(string parameters)
        {
            ListRecordArray(fileCabinetService.GetRecords());
        }

        private static void Edit(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("'Edit' command had no parameters");
                return;
            }

            if (!int.TryParse(parameters, out int value))
            {
                Console.WriteLine("Parameter should be a number");
                return;
            }

            if (fileCabinetService.GetStat() < value || value < 1)
            {
                Console.WriteLine($"#{value} record is not found.");
                return;
            }

            GetUserInputAndRecord(InputMode.Edit, value);
        }

        private static void Find(string parameters)
        {
            parameters = parameters.ToLower(CultureInfo.InvariantCulture);
            string[] splittedString = parameters.Split(' ');
            if (splittedString.Length != 2)
            {
                Console.WriteLine("'Find' command should have 2 parameters");
                return;
            }

            if (splittedString[1][0] != '\"' || splittedString[1][^1] != '\"')
            {
                Console.WriteLine("Quotation marks should not be omitted");
                return;
            }

            splittedString[1] = splittedString[1][1..^1].ToLower(CultureInfo.InvariantCulture);

            switch (splittedString[0])
            {
                case "firstname":
                    ListRecordArray(fileCabinetService.FindByFirstName(splittedString[1]));
                    break;
                case "lastname":
                    ListRecordArray(fileCabinetService.FindByLastName(splittedString[1]));
                    break;
                case "dateofbirth":
                    if (!DateTime.TryParse(splittedString[1], out DateTime date))
                    {
                        Console.WriteLine("Incorrect date value");
                    }

                    ListRecordArray(fileCabinetService.FindByDateOfBirth(date));
                    break;
                default:
                    Console.WriteLine("Incorrect parameter");
                    return;
            }
        }

        private static void ListRecordArray(FileCabinetRecord[] records)
        {
            for (int i = 0; i < records.Length; i++)
            {
                var item = records[i];
                Console.WriteLine("#{0}, {1}, {2}, sex: {3}, weight: {4}, height: {5}, {6}", i + 1, item.FirstName, item.LastName, item.Sex, item.Weight, item.Height, item.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture));
            }
        }

        private static void GetUserInputAndRecord(InputMode mode, int value = 0)
        {
            Console.WriteLine();
            Console.Write("First name: ");
            string? firstName = Console.ReadLine();
            if (string.IsNullOrEmpty(firstName))
            {
                Console.WriteLine("First Name can not be empty");
                GetUserInputAndRecord(mode);
                return;
            }

            Console.Write("Last name: ");
            string? lastName = Console.ReadLine();
            if (string.IsNullOrEmpty(lastName))
            {
                Console.WriteLine("Last Name can not be empty");
                GetUserInputAndRecord(mode);
                return;
            }

            Console.Write("Sex: ");
            string? sexString = Console.ReadLine();
            if (string.IsNullOrEmpty(sexString))
            {
                Console.WriteLine("Sex can not be empty");
                GetUserInputAndRecord(mode);
                return;
            }

            Console.Write("Weight: ");
            string? weightString = Console.ReadLine();
            if (string.IsNullOrEmpty(weightString))
            {
                Console.WriteLine("Weight can not be empty.");
                GetUserInputAndRecord(mode);
                return;
            }

            if (!short.TryParse(weightString, out short weight))
            {
                Console.WriteLine("Weight value was invalid.");
                GetUserInputAndRecord(mode);
                return;
            }

            Console.Write("Height: ");
            string? heightString = Console.ReadLine();
            if (string.IsNullOrEmpty(heightString))
            {
                Console.WriteLine("Height can not be empty");
                GetUserInputAndRecord(mode);
                return;
            }

            if (!decimal.TryParse(heightString, out decimal height))
            {
                Console.WriteLine("Height value was invalid.");
                GetUserInputAndRecord(mode);
                return;
            }

            Console.Write("Date of birth: ");
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Date can not be empty");
                GetUserInputAndRecord(mode);
                return;
            }

            string[] splitedInput = input.Split('/');
            if (splitedInput.Length != 3)
            {
                Console.WriteLine("Date is incorrect");
                GetUserInputAndRecord(mode);
                return;
            }

            int month = int.Parse(splitedInput[0], CultureInfo.InvariantCulture);
            int day = int.Parse(splitedInput[1], CultureInfo.InvariantCulture);
            int year = int.Parse(splitedInput[2], CultureInfo.InvariantCulture);
            DateTime date = new (year, month, day);
            UnvalidatedRecordData unvalidatedRecord = new (firstName, lastName, sexString[0], weight, height, date);

            try
            {
                if (mode == InputMode.Create)
                {
                    fileCabinetService.CreateRecord(unvalidatedRecord);
                }
                else
                {
                    fileCabinetService.EditRecord(value, unvalidatedRecord);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Your input was not recorded. Try again.");
                GetUserInputAndRecord(mode, value);
                return;
            }

            if (mode == InputMode.Create)
            {
                Console.WriteLine($"Record #{fileCabinetService.GetStat()} is created");
            }
            else
            {
                Console.WriteLine($"Record #{value} is updated");
            }
        }

        private static void ApplyArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i][0] == '-')
                {
                    switch (args[i][1])
                    {
                        case '-':
                            ProcessArgument(args[i]);
                            break;
                        default:
                            if (args.Length > i + 1)
                            {
                                ProcessArgument(args[i], args[i + 1]);
                                i++;
                            }

                            break;
                    }
                }
                else
                {
                    throw new ArgumentException($"Unknown command \"{args[i]}\"");
                }
            }
        }

        private static void ProcessArgument(string argument)
        {
            string[] splittedArgument = argument.Split('=');
            if (splittedArgument.Length != 2)
            {
                throw new ArgumentException($"Invalid argument \"{splittedArgument[0]}\"");
            }

            switch (splittedArgument[0])
            {
                case "--validation-rules":
                    switch (splittedArgument[1].ToLower(CultureInfo.InvariantCulture))
                    {
                        case "custom":
                            fileCabinetService = new FileCabinetService(new CustomValidator());
                            return;
                        case "default":
                            return;
                    }

                    throw new ArgumentException($"Unknown argument \"{splittedArgument[1]}\"");
            }

            throw new ArgumentException($"Unknown argument \"{splittedArgument[0]}\"");
        }

        private static void ProcessArgument(string argument1, string argument2)
        {
            switch (argument1)
            {
                case "-v":
                    switch (argument2.ToLower(CultureInfo.InvariantCulture))
                    {
                        case "custom":
                            fileCabinetService = new FileCabinetService(new CustomValidator());
                            return;
                        case "default":
                            return;
                    }

                    throw new ArgumentException($"Unknown argument \"{argument2}\"");
            }

            throw new ArgumentException($"Unknown command \"{argument1}\"");
        }
    }
}