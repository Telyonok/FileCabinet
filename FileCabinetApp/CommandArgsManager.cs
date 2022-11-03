using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides correct parameter values for command line arguments.
    /// </summary>
    public static class CommandArgsManager
    {
        /// <summary>
        /// Gets a list of allowed parameters for a console command.
        /// </summary>
        /// <param name="command">Entered command.</param>
        /// <returns>Collection of allowed parameters.</returns>
        public static ReadOnlyCollection<string> GetAllowedParametersByCommand(string command)
        {
            return command switch
            {
                "FileCabinetApp" => new ReadOnlyCollection<string>(new List<string> { "--validation-rules", "-v", "--storage", "-s" }),
                "FileCabinetGenerator" => new ReadOnlyCollection<string>(new List<string> { "--output-type", "--output-type", "-t", "--output", "-o", "--records-amount", "-a", "--start-id", "-i" }),
                _ => new ReadOnlyCollection<string>(new List<string> { }),
            };
        }

        /// <summary>
        /// Gets count of values required after a parameter.
        /// </summary>
        /// <param name="parameter">Entered parameter.</param>
        /// <returns>Count of values.</returns>
        public static int GetParameterValueCount(string parameter)
        {
            return parameter switch
            {
                "--storage" => 1,
                "-s" => 1,
                "-v" => 1,
                "--validation-rules" => 1,
                "--output-type" => 1,
                "-t" => 1,
                "--output" => 1,
                "-o" => 1,
                "--records-amount" => 1,
                "-a" => 1,
                "--start-id" => 1,
                "-i" => 1,
                _ => 0,
            };
        }

        /// <summary>
        /// Gets a list of allowed values for a parameter.
        /// </summary>
        /// <param name="parameter">Entered parameter.</param>
        /// <returns>Collection of allowed values.</returns>
        public static ReadOnlyCollection<string> GetAllowedValuesByParameter(string parameter)
        {
            return parameter switch
            {
                "--storage" => new ReadOnlyCollection<string>(new List<string> { "memory", "file" }),
                "-s" => new ReadOnlyCollection<string>(new List<string> { "memory", "file" }),
                "--validation-rules" => new ReadOnlyCollection<string>(new List<string> { "custom", "default" }),
                "-v" => new ReadOnlyCollection<string>(new List<string> { "custom", "default" }),
                "--output-type" => new ReadOnlyCollection<string>(new List<string> { "csv", "xml" }),
                "-t" => new ReadOnlyCollection<string>(new List<string> { "csv", "xml" }),
                _ => new ReadOnlyCollection<string>(new List<string> { }),
            };
        }

        /// <summary>
        /// Validates entered arguments.
        /// </summary>
        /// <param name="command">First entered argument.</param>
        /// <param name="args">Other entered arguments.</param>
        /// <returns>True if validation was succesful.</returns>
        public static bool ValidateCommandArguments(string command, string[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }

            if (!string.IsNullOrEmpty(args.First()))
            {
                int requiredValueCount = 0;
                int valueCount = 0;
                string currentParameter = string.Empty;
                foreach (var arg in args)
                {
                    if (arg[0] == '-')
                    {
                        if (requiredValueCount != valueCount)
                        {
                            PrintWrongValueCountInfo(currentParameter);
                            return false;
                        }

                        if (CommandArgsManager.GetAllowedParametersByCommand(command).Count == 0 && !CommandArgsManager.GetAllowedParametersByCommand(command).Contains(arg))
                        {
                            PrintWrongCommandParameterInfo(command, arg);
                            return false;
                        }

                        currentParameter = arg;
                        requiredValueCount = CommandArgsManager.GetParameterValueCount(arg);
                        valueCount = 0;
                    }
                    else
                    {
                        if (CommandArgsManager.GetAllowedValuesByParameter(currentParameter).Count != 0 && !CommandArgsManager.GetAllowedValuesByParameter(currentParameter).Contains(arg))
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

        private static void PrintWrongCommandParameterInfo(string command, string parameter)
        {
            Console.WriteLine($"'{parameter}' parameter doesn't exist for '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintWrongValueCountInfo(string parameter)
        {
            Console.WriteLine($"'{parameter}' parameter should have {CommandArgsManager.GetParameterValueCount(parameter)} values.");
            Console.WriteLine();
        }

        private static void PrintWrongValueInfo(string parameter, string value)
        {
            Console.WriteLine($"'{parameter}' parameter has no '{value}' value.");
            Console.WriteLine();
        }
    }
}
