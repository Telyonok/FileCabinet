using System.Collections.ObjectModel;
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
            new Tuple<string, Action<string>>("export", Export),
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
            fileCabinetService.CreateRecord();

            // GetUserInputAndRecord(InputMode.Create);
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

            fileCabinetService.EditRecord(value);
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

            splittedString[1] = splittedString[1][1..^1];

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

        private static void Export(string parameters)
        {
            parameters = parameters.ToLower(CultureInfo.InvariantCulture);
            string[] splittedString = parameters.Split(' ');
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
                    Console.Write($"File already exists - rewrite {splittedString[1]}? [Y/n]");
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
                    break;
                default:
                    Console.WriteLine("Incorrect parameter");
                    return;
            }

            Console.WriteLine($"All records are exported to file {splittedString[1]}.");
            streamWriter.Close();
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