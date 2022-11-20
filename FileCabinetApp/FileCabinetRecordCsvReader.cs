using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Reads records from a csv file.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="streamReader">Stream to read from.</param>
        public FileCabinetRecordCsvReader(StreamReader streamReader)
        {
            this.reader = streamReader;
        }

        /// <summary>
        /// Reads records from a csv file.
        /// </summary>
        /// <returns>List of read records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> records = new ();
            this.reader.ReadLine();
            while (!this.reader.EndOfStream)
            {
                var line = this.reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    Console.WriteLine("Line was empty. Skipping.");
                    continue;
                }
                else
                {
                    var splittedLine = line.Split(", ");
                    if (splittedLine.Length != 7)
                    {
                        Console.WriteLine("Line had incorrect amount of parameters. Skipping.");
                        continue;
                    }

                    int id;
                    if (!int.TryParse(splittedLine[0], out id))
                    {
                        Console.WriteLine("Id parameter wasn't a number. Skipping.");
                        continue;
                    }

                    string firstname = splittedLine[1];
                    string lastname = splittedLine[2];
                    char sex = splittedLine[3][0];
                    short weight;
                    if (!short.TryParse(splittedLine[4], out weight))
                    {
                        Console.WriteLine("Weight parameter wasn't a number. Skipping.");
                        continue;
                    }

                    decimal height;
                    if (!decimal.TryParse(splittedLine[5], out height))
                    {
                        Console.WriteLine("Height parameter wasn't a number. Skipping.");
                        continue;
                    }

                    DateTime dateOfBirth;
                    if (!DateTime.TryParse(splittedLine[6], out dateOfBirth))
                    {
                        Console.WriteLine("Date of birth was in incorrect format. Skipping.");
                        continue;
                    }

                    records.Add(new (id, firstname, lastname, sex, weight, height, dateOfBirth));
                }
            }

            return records;
        }
    }
}
