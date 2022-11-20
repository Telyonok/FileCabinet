using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using FileCabinetApp;
class FileCabinetGenerator
{
    static void Main(string[] arguments)
    {
        IRecordValidator validator = new DefaultValidator();
        arguments = arguments.Select(s => s.ToLowerInvariant()).ToArray();
        List<string> args = new ();
        foreach (var arg in arguments)
        {
            foreach (var sarg in arg.Split('='))
            {
                args.Add(sarg);
            }
        }

        CommandArgsManager.ValidateCommandArguments("FileCabinetGenerator", args.ToArray());

        int recordCount = FindValue(args, "--records-amount", "-a", 1);

        int startId = FindValue(args, "--start-id", "-i", 0);

        string path = "records";
        int index = Math.Max(args.IndexOf("--output"), args.IndexOf("-o"));
        if (index != -1)
        {
            path = args[index + 1];
        }
        
        List<FileCabinetRecord> records = new List<FileCabinetRecord>();

        int id = startId;
        for (int i = 0; i < recordCount; i++)
        {
            records.Add(GenerateRecord(id, validator));
            id++;
        }

        FileCabinetServiceSnapshot snapshot = new (records);

        try 
        {
            if (args.Contains("xml"))
            {
                snapshot.SaveToXml(new StreamWriter(path));
            }
            else
            {
                snapshot.SaveToCsv(new StreamWriter(path));
            }
        }
        catch
        {
            throw new ArgumentException($"Couldn't open/create file {path}.");
        }

        Console.WriteLine($"{recordCount} records were written to {path}");
    }

    private static int FindValue(List<string> args, string firstCommand, string secondCommand, int defaultValue)
    {
        int value = defaultValue;
        int index = Math.Max(args.IndexOf(firstCommand), args.IndexOf(secondCommand));
        if (index != -1)
        {
            if (!int.TryParse(args[index + 1], out value))
            {
                throw new ArgumentException($"{args[index]} must be followed by an int");
            }
        }

        return value;
    }

    private static FileCabinetRecord GenerateRecord(int id, IRecordValidator validator)
    {
        Random random = new Random();
        string firstname = GenerateString(random.Next(2, 60));
        string lastname = GenerateString(random.Next(2, 60));
        char sex = GenerateString(1)[0];
        short weight = (short)random.Next(0, 300);
        decimal height = random.Next(0, 300);

        DateTime start = new DateTime(1950, 1, 1);
        int range = (DateTime.Today - start).Days;
        DateTime dateOfBirth = start.AddDays(random.Next(range));

        return new (id, firstname, lastname, sex, weight, height, dateOfBirth);
    }

    private static string GenerateString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        var stringChars = new char[length];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new String(stringChars);
    }
}