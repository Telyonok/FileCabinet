using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

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
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
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
            new string[] { "export", "exports all current records to a file", "The 'export <parameter>' command outputs all current records to a file of <parameter> format. Possible values are: csv, xml" },
            new string[] { "import", "imports all records from a file", "The 'import <parameter> <path>' command imports all records from the <path> file of <parameter> format. Possible <parameter> values are: csv, xml" },
        };

        private static bool isRunning = true;

        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());

        /// <summary>
        /// Accepts input and calls corresponding methods.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            ApplyLaunchArguments(args.Select(arg => arg.ToLower(CultureInfo.InvariantCulture)).ToArray());
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine($"Using {fileCabinetService.GetServiceName()} service.");
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
                    Commands[index].Item2(parameters.ToLower(CultureInfo.InvariantCulture).Trim());
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
            fileCabinetService.Finish();
        }

        private static void Stat(string parameters)
        {
            var splittedParameters = parameters.Split(' ', '=');
            if (!CommandArgsManager.ValidateCommandArguments("stat", splittedParameters))
            {
                return;
            }

            int recordsCount;

            recordsCount = fileCabinetService.GetStat();

            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            var splittedParameters = parameters.Split(' ', '=');
            if (!CommandArgsManager.ValidateCommandArguments("create", splittedParameters))
            {
                return;
            }

            fileCabinetService.CreateRecord(GetUserInputData());
        }

        private static void List(string parameters)
        {
            var splittedParameters = parameters.Split(' ', '=');
            if (!CommandArgsManager.ValidateCommandArguments("list", splittedParameters))
            {
                return;
            }

            ListRecordArray(fileCabinetService.GetRecords());
        }

        private static void Edit(string parameters)
        {
            var splittedParameters = parameters.Split(' ', '=');
            if (string.IsNullOrEmpty(parameters) || splittedParameters[^1][0] == '-')
            {
                Console.WriteLine("'Edit' command must have 1 value argument");
                return;
            }

            if (splittedParameters.Length > 1)
            {
                if (!CommandArgsManager.ValidateCommandArguments("edit", splittedParameters[..^1]))
                {
                    return;
                }
            }

            if (!int.TryParse(splittedParameters[^1], out int value))
            {
                Console.WriteLine("Parameter should be a number");
                return;
            }

            if (fileCabinetService.GetStat() < value || value < 1)
            {
                Console.WriteLine($"#{value} record is not found.");
                return;
            }

            fileCabinetService.EditRecord(value, GetUserInputData());
        }

        private static void Find(string parameters)
        {
            string[] splittedParameters = parameters.Split(' ', '=');
            if (splittedParameters.Length < 2)
            {
                Console.WriteLine("'Find' command must have 2 value arguments");
                return;
            }

            if (splittedParameters[^1][0] != '\"' || splittedParameters[^1][^1] != '\"')
            {
                Console.WriteLine("Quotation marks should not be omitted");
                return;
            }

            if (splittedParameters.Length > 1)
            {
                if (!CommandArgsManager.ValidateCommandArguments("find", splittedParameters[..^2]))
                {
                    return;
                }
            }

            splittedParameters[^1] = splittedParameters[^1][1..^1];

            switch (splittedParameters[^2])
            {
                case "firstname":
                    ListRecordArray(fileCabinetService.FindByFirstName(splittedParameters[^1]));
                    break;
                case "lastname":
                    ListRecordArray(fileCabinetService.FindByLastName(splittedParameters[^1]));
                    break;
                case "dateofbirth":
                    if (DateTime.TryParse(splittedParameters[^1], out DateTime date))
                    {
                        ListRecordArray(fileCabinetService.FindByDateOfBirth(date));
                    }
                    else
                    {
                        Console.WriteLine("Incorrect parameter");
                    }

                    break;
                default:
                    Console.WriteLine("Incorrect parameter");
                    return;
            }
        }

        private static void Export(string parameters)
        {
            parameters = parameters.ToLower(CultureInfo.InvariantCulture);
            string[] splittedString = parameters.Split(' ', '=');
            if (splittedString.Length != 2)
            {
                Console.WriteLine("'Export' command should have 2 parameters");
                return;
            }

            StreamWriter streamWriter;

            if (File.Exists(splittedString[1]))
            {
                char input;
                bool correct;
                do
                {
                    Console.Write($"File already exists - rewrite {splittedString[1]}? [Y/n] ");
                    correct = char.TryParse(Console.ReadLine(), out input);
                }
                while (!correct || (input != 'Y' && input != 'n'));

                if (input == 'n')
                {
                    return;
                }
            }

            try
            {
                streamWriter = new (splittedString[1]);
            }
            catch
            {
                Console.WriteLine($"Export failed: can\'t open file {splittedString[1]}.");
                return;
            }

            FileCabinetServiceSnapshot snapshot = fileCabinetService.MakeSnapshot();
            switch (splittedString[0])
            {
                case "csv":
                    snapshot.SaveToCsv(streamWriter);
                    break;
                case "xml":
                    snapshot.SaveToXml(streamWriter);
                    break;
                default:
                    Console.WriteLine("Incorrect parameter");
                    return;
            }

            Console.WriteLine($"All records are exported to file {splittedString[1]}.");
            streamWriter.Close();
        }

        private static void Import(string parameters)
        {
            string[] splittedParameters = parameters.Split(' ');
            if (splittedParameters.Length != 2)
            {
                Console.WriteLine("'Import' command should have 2 parameters.");
                return;
            }

            string path = splittedParameters[1];
            FileStream fileStream;
            try
            {
                fileStream = new (path, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                Console.WriteLine($"Couldn't open '{path}' file.");
                return;
            }

            FileCabinetServiceSnapshot snapshot = fileCabinetService.MakeSnapshot();

            if (splittedParameters[0] == "csv")
            {
                snapshot.LoadFromCsv(new StreamReader(fileStream));
            }
            else if (splittedParameters[0] == "xml")
            {
                snapshot.LoadFromXml(new StreamReader(fileStream));
            }
            else
            {
                Console.WriteLine("First value must be \"csv\" or \"xml\".");
                return;
            }

            fileCabinetService.RestoreSnapshot(snapshot);
        }

        private static void ListRecordArray(ReadOnlyCollection<FileCabinetRecord> records)
        {
            int i = 0;
            foreach (var item in records)
            {
                Console.WriteLine("#{0}, {1}, {2}, sex: {3}, weight: {4}, height: {5}, {6}", i + 1, item.FirstName, item.LastName, item.Sex, item.Weight, item.Height, item.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture));
                i++;
            }
        }

        private static void ApplyLaunchArguments(string[] args)
        {
            if (!CommandArgsManager.ValidateCommandArguments("FileCabinetApp", args))
            {
                throw new ArgumentException("Invalid argument input", nameof(args));
            }

            IRecordValidator validator;
            if (args.Contains<string>("custom") || args.Contains<string>("--validation-rules=custom"))
            {
                validator = new CustomValidator();
            }
            else
            {
                validator = new DefaultValidator();
            }

            if (args.Contains<string>("file"))
            {
                fileCabinetService = new FileCabinetFilesystemService(new FileStream("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite), validator);
            }
            else
            {
                fileCabinetService = new FileCabinetMemoryService(validator);
            }
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                if (input == null)
                {
                    Console.WriteLine($"Conversion failed: input was empty. Please, correct your input.");
                    continue;
                }

                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static Tuple<bool, string, string> InputToNameConverter(string input)
        {
            return Tuple.Create<bool, string, string>(!string.IsNullOrEmpty(input), "name was null or empty", input);
        }

        private static Tuple<bool, string, DateTime> InputToDateConverter(string input)
        {
            bool successful = true;
            DateTime time = DateTime.MinValue;
            try
            {
                time = DateTime.Parse(input, CultureInfo.CurrentCulture);
            }
            catch
            {
                successful = false;
            }

            return Tuple.Create<bool, string, DateTime>(successful, "incorrect date format", time);
        }

        private static Tuple<bool, string, char> InputToSexConverter(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return Tuple.Create<bool, string, char>(true, string.Empty, input[0]);
            }
            else
            {
                return Tuple.Create<bool, string, char>(false, "sex was null or empty", '?');
            }
        }

        private static Tuple<bool, string, short> InputToWeightConverter(string input)
        {
            bool succesful = short.TryParse(input, out short weight);
            return Tuple.Create<bool, string, short>(succesful, "weight was null or empty", weight);
        }

        private static Tuple<bool, string, decimal> InputToHeightConverter(string input)
        {
            bool succesful = decimal.TryParse(input, out decimal height);
            return Tuple.Create<bool, string, decimal>(succesful, "weight was null or empty", height);
        }

        private static InputDataSet GetUserInputData()
        {
            Console.Write("First name: ");
            string firstName = ReadInput<string>(InputToNameConverter, fileCabinetService.Validator.ValidateNameString);
            Console.Write("Last name: ");
            string lastName = ReadInput<string>(InputToNameConverter, fileCabinetService.Validator.ValidateNameString);
            Console.Write("Sex: ");
            char sex = ReadInput<char>(InputToSexConverter, fileCabinetService.Validator.ValidateSex);
            Console.Write("Weight: ");
            short weight = ReadInput<short>(InputToWeightConverter, fileCabinetService.Validator.ValidateWeight);
            Console.Write("Height: ");
            decimal height = ReadInput<decimal>(InputToHeightConverter, fileCabinetService.Validator.ValidateHeight);
            Console.Write("Date of birth: ");
            DateTime dateOfBirth = ReadInput<DateTime>(InputToDateConverter, fileCabinetService.Validator.ValidateDateTime);

            return new InputDataSet(firstName, lastName, sex, weight, height, dateOfBirth);
        }
    }
}