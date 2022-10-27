using System.Collections.ObjectModel;
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

        private static FileCabinetMemoryService fileCabinetMemoryService = new (new DefaultValidator());
        private static FileCabinetFilesystemService fileCabinetFilesystemService = new (new FileStream("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite));

        /// <summary>
        /// Accepts input and calls corresponding methods.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            ApplyLaunchArguments(args.Select(arg => arg.ToLower(CultureInfo.InvariantCulture)).ToArray());
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine($"Using {fileCabinetMemoryService.Validator.GetValidatorName()} validation rules.");
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

        private static ReadOnlyCollection<string> GetAllowedParametersByCommand(string command)
        {
            return command switch
            {
                "FileCabinetApp" => new ReadOnlyCollection<string>(new List<string> { "--validation-rules=custom", "--validation-rules=default", "-v" }),
                "create" => new ReadOnlyCollection<string>(new List<string> { "--storage", "-s" }),
                "edit" => new ReadOnlyCollection<string>(new List<string> { "--storage", "-s" }),
                "find" => new ReadOnlyCollection<string>(new List<string> { "--storage", "-s" }),
                "stat" => new ReadOnlyCollection<string>(new List<string> { "--storage", "-s" }),
                "list" => new ReadOnlyCollection<string>(new List<string> { "--storage", "-s" }),
                _ => new ReadOnlyCollection<string>(Enumerable.Empty<string>().ToList()),
            };
        }

        private static int GetParameterValueCount(string parameter)
        {
            return parameter switch
            {
                "--storage" => 1,
                "-s" => 1,
                "-v" => 1,
                "--validation-rules=custom" => 0,
                "--validation-rules=default" => 0,
                _ => 0,
            };
        }

        private static ReadOnlyCollection<string> GetAllowedValuesByParameter(string parameter)
        {
            return parameter switch
            {
                "--storage" => new ReadOnlyCollection<string>(new List<string> { "memory", "file" }),
                "-s" => new ReadOnlyCollection<string>(new List<string> { "memory", "file" }),
                "--validation-rule" => new ReadOnlyCollection<string>(new List<string> { "custom", "default" }),
                "-v" => new ReadOnlyCollection<string>(new List<string> { "custom", "default" }),
                _ => new ReadOnlyCollection<string>(Enumerable.Empty<string>().ToList()),
            };
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintWrongCommandParameterInfo(string command, string parameter)
        {
            Console.WriteLine($"'{parameter}' parameter doesn't exist for '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintWrongValueCountInfo(string parameter)
        {
            Console.WriteLine($"'{parameter}' parameter should have {GetParameterValueCount(parameter)} values.");
            Console.WriteLine();
        }

        private static void PrintWrongValueInfo(string parameter, string value)
        {
            Console.WriteLine($"'{parameter}' parameter has no '{value}' value.");
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
            var splittedParameters = parameters.Split(" ");
            if (!ValidateCommandArguments("stat", splittedParameters))
            {
                return;
            }

            int recordsCount;

            if (splittedParameters.Contains<string>("file"))
            {
                recordsCount = fileCabinetFilesystemService.GetStat();
            }
            else
            {
                recordsCount = fileCabinetMemoryService.GetStat();
            }

            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            var splittedParameters = parameters.Split(" ");
            if (!ValidateCommandArguments("create", splittedParameters))
            {
                return;
            }

            if (splittedParameters.Contains<string>("file"))
            {
                fileCabinetFilesystemService.CreateRecord();
            }
            else
            {
                fileCabinetMemoryService.CreateRecord();
            }
        }

        private static void List(string parameters)
        {
            var splittedParameters = parameters.Split(" ");
            if (!ValidateCommandArguments("list", splittedParameters))
            {
                return;
            }

            if (splittedParameters.Contains<string>("file"))
            {
                ListRecordArray(fileCabinetFilesystemService.GetRecords());
            }
            else
            {
                ListRecordArray(fileCabinetMemoryService.GetRecords());
            }
        }

        private static void Edit(string parameters)
        {
            var splittedParameters = parameters.Split(" ");
            if (string.IsNullOrEmpty(parameters) || splittedParameters[^1][0] == '-')
            {
                Console.WriteLine("'Edit' command must have 1 value argument");
                return;
            }

            if (splittedParameters.Length > 1)
            {
                if (!ValidateCommandArguments("edit", splittedParameters[..^1]))
                {
                    return;
                }
            }

            if (!int.TryParse(splittedParameters[^1], out int value))
            {
                Console.WriteLine("Parameter should be a number");
                return;
            }

            if (fileCabinetMemoryService.GetStat() < value || value < 1)
            {
                Console.WriteLine($"#{value} record is not found.");
                return;
            }

            if (splittedParameters.Contains<string>("file"))
            {
                fileCabinetFilesystemService.EditRecord(value);
            }
            else
            {
                fileCabinetMemoryService.EditRecord(value);
            }
        }

        private static void Find(string parameters)
        {
            string[] splittedParameters = parameters.Split(' ');
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
                if (!ValidateCommandArguments("find", splittedParameters[..^2]))
                {
                    return;
                }
            }

            splittedParameters[^1] = splittedParameters[^1][1..^1];

            switch (splittedParameters[^2])
            {
                case "firstname":
                    ListRecordArray(fileCabinetMemoryService.FindByFirstName(splittedParameters[^1]));
                    break;
                case "lastname":
                    ListRecordArray(fileCabinetMemoryService.FindByLastName(splittedParameters[^1]));
                    break;
                case "dateofbirth":
                    if (!DateTime.TryParse(splittedParameters[^1], out DateTime date))
                    {
                        Console.WriteLine("Incorrect date value");
                    }

                    ListRecordArray(fileCabinetMemoryService.FindByDateOfBirth(date));
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

            FileCabinetServiceSnapshot snapshot = fileCabinetMemoryService.MakeSnapshot();
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
            if (!ValidateCommandArguments("FileCabinetApp", args))
            {
                throw new ArgumentException("Invalid argument input", nameof(args));
            }

            if (args.Contains<string>("custom") || args.Contains<string>("--validation-rules=custom"))
            {
                fileCabinetMemoryService = new FileCabinetMemoryService(new CustomValidator());
            }
            else
            {
                fileCabinetMemoryService = new FileCabinetMemoryService(new DefaultValidator());
            }
        }

        private static bool ValidateCommandArguments(string command, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                return true;
            }

            if (!string.IsNullOrEmpty(arguments[0]))
            {
                int requiredValueCount = 0;
                int valueCount = 0;
                string currentParameter = string.Empty;
                foreach (var arg in arguments)
                {
                    if (arg[0] == '-')
                    {
                        if (requiredValueCount != valueCount)
                        {
                            PrintWrongValueCountInfo(currentParameter);
                            return false;
                        }

                        if (!GetAllowedParametersByCommand(command).Contains(arg))
                        {
                            PrintWrongCommandParameterInfo(command, arg);
                            return false;
                        }

                        currentParameter = arg;
                        requiredValueCount = GetParameterValueCount(arg);
                        valueCount = 0;
                    }
                    else
                    {
                        if (!GetAllowedValuesByParameter(currentParameter).Contains(arg))
                        {
                            if (currentParameter == string.Empty)
                            {
                                Console.WriteLine("Parameter should start with '-'");
                            }
                            else
                            {
                                PrintWrongValueInfo(currentParameter, arg);
                            }

                            return false;
                        }

                        valueCount++;
                    }
                }

                if (requiredValueCount != valueCount)
                {
                    PrintWrongValueCountInfo(currentParameter);
                    return false;
                }
            }

            return true;
        }
    }
}