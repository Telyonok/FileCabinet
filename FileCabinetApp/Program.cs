using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Ilya Chvilyov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;

        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints records amount", "The 'stat' command prints amount of records" },
            new string[] { "create", "creates a new record", "The 'create' command creates a new note" },
            new string[] { "list", "lists all records", "The 'list' command lists all records" },
        };

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
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

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
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
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void List(string parameters)
        {
            var array = fileCabinetService.GetRecords();
            for (int i = 0; i < fileCabinetService.GetStat(); i++)
            {
                var item = array[i];
                Console.WriteLine("#{0}, {1}, {2}, sex: {3}, weight: {4}, height: {5}, {6}", i + 1, item.FirstName, item.LastName, item.Sex, item.Weight, item.Height, item.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture));
            }
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            string? firstName = Console.ReadLine();
            if (string.IsNullOrEmpty(firstName))
            {
                Console.WriteLine("First Name can not be empty");
                return;
            }

            Console.Write("Last name: ");
            string? lastName = Console.ReadLine();
            if (string.IsNullOrEmpty(lastName))
            {
                Console.WriteLine("Last Name can not be empty");
                return;
            }

            Console.Write("Sex: ");
            string? sexString = Console.ReadLine();
            if (string.IsNullOrEmpty(sexString))
            {
                Console.WriteLine("Sex can not be empty");
                return;
            }

            Console.Write("Weight: ");
            string? weightString = Console.ReadLine();
            if (string.IsNullOrEmpty(weightString))
            {
                Console.WriteLine("Weight can not be empty");
                return;
            }

            short weight = short.Parse(weightString, CultureInfo.InvariantCulture);

            Console.Write("Height: ");
            string? heightString = Console.ReadLine();
            if (string.IsNullOrEmpty(heightString))
            {
                Console.WriteLine("Height can not be empty");
                return;
            }

            decimal height = decimal.Parse(heightString, CultureInfo.InvariantCulture);

            Console.Write("Date of birth: ");
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Date can not be empty");
                return;
            }

            string[] splitedInput = input.Split('/');
            if (splitedInput.Length != 3)
            {
                Console.WriteLine("Date is incorrect");
                return;
            }

            int month = int.Parse(splitedInput[0], CultureInfo.InvariantCulture);
            int day = int.Parse(splitedInput[1], CultureInfo.InvariantCulture);
            int year = int.Parse(splitedInput[2], CultureInfo.InvariantCulture);

            Console.WriteLine($"Record #{fileCabinetService.GetStat() + 1} is created");
            DateTime date = new DateTime(year, month, day);
            fileCabinetService.CreateRecord(firstName, lastName, sexString[0], weight, height, date);
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }
    }
}