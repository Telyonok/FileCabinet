using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Class <c>FileCabinetService</c> provides methods for creating, editting, listing,
    /// finding and validating records.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">Set of validation methods.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.Validator = validator;
        }

        /// <summary>
        /// Gets validator used by the service.
        /// </summary>
        /// <value>
        /// validator.
        /// </value>
        public IRecordValidator Validator { get; }

        /// <inheritdoc/>
        public int CreateRecord()
        {
            var record = new FileCabinetRecord();
            Console.Write("First name: ");
            record.FirstName = ReadInput<string>(InputToNameConverter, this.Validator.ValidateNameString);
            Console.Write("Last name: ");
            record.LastName = ReadInput<string>(InputToNameConverter, this.Validator.ValidateNameString);
            Console.Write("Sex: ");
            record.Sex = ReadInput<char>(InputToSexConverter, this.Validator.ValidateSex);
            Console.Write("Weight: ");
            record.Weight = ReadInput<short>(InputToWeightConverter, this.Validator.ValidateWeight);
            Console.Write("Height: ");
            record.Height = ReadInput<decimal>(InputToHeightConverter, this.Validator.ValidateHeight);
            Console.Write("Date of birth: ");
            record.DateOfBirth = ReadInput<DateTime>(InputToDateConverter, this.Validator.ValidateDateTime);

            record.Id = this.list.Count;

            this.list.Add(record);
            this.UpdateDictionaries(record);
            Console.Write($"Record {record.Id + 1} created successfuly: \n");

            return record.Id;
        }

        /// <inheritdoc/>
        public void EditRecord(int value)
        {
            int id = value - 1;
            string? oldFirstName = this.list[id].FirstName;
            string? oldLastName = this.list[id].LastName;
            DateTime oldDateOfBirth = this.list[id].DateOfBirth;

            if (oldFirstName is null)
            {
                throw new MissingMemberException(nameof(oldFirstName));
            }

            if (oldLastName is null)
            {
                throw new MissingMemberException(nameof(oldLastName));
            }

            this.firstNameDictionary[oldFirstName.ToLower(CultureInfo.InvariantCulture)].Remove(this.list[id]);
            this.lastNameDictionary[oldLastName.ToLower(CultureInfo.InvariantCulture)].Remove(this.list[id]);
            this.dateOfBirthDictionary[oldDateOfBirth].Remove(this.list[id]);

            Console.Write("First name: ");
            this.list[id].FirstName = ReadInput<string>(InputToNameConverter, this.Validator.ValidateNameString);
            Console.Write("Last name: ");
            this.list[id].LastName = ReadInput<string>(InputToNameConverter, this.Validator.ValidateNameString);
            Console.Write("Sex: ");
            this.list[id].Sex = ReadInput<char>(InputToSexConverter, this.Validator.ValidateSex);
            Console.Write("Weight: ");
            this.list[id].Weight = ReadInput<short>(InputToWeightConverter, this.Validator.ValidateWeight);
            Console.Write("Height: ");
            this.list[id].Height = ReadInput<decimal>(InputToHeightConverter, this.Validator.ValidateHeight);
            Console.Write("Date of birth: ");
            this.list[id].DateOfBirth = ReadInput<DateTime>(InputToDateConverter, this.Validator.ValidateDateTime);

            Console.Write($"Record {value} updated successfuly: \n");

            this.UpdateDictionaries(this.list[id]);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.list.AsReadOnly();
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (!this.firstNameDictionary.ContainsKey(firstName))
            {
                return new List<FileCabinetRecord>().AsReadOnly();
            }

            return this.firstNameDictionary[firstName].AsReadOnly();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (!this.lastNameDictionary.ContainsKey(lastName))
            {
                return new List<FileCabinetRecord>().AsReadOnly();
            }

            return this.lastNameDictionary[lastName].AsReadOnly();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                return new List<FileCabinetRecord>().AsReadOnly();
            }

            return this.dateOfBirthDictionary[dateOfBirth].AsReadOnly();
        }

        /// <summary>
        /// Creates a snapshot of current recordList.
        /// </summary>
        /// <returns>Created recordList snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list);
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
                time = DateTime.Parse(input, CultureInfo.InvariantCulture);
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

        private void UpdateDictionaries(FileCabinetRecord record)
        {
            if (record.FirstName is null)
            {
                throw new MissingMemberException(nameof(record.FirstName));
            }

            if (record.LastName is null)
            {
                throw new MissingMemberException(nameof(record.LastName));
            }

            string firstName = record.FirstName.ToLower(CultureInfo.InvariantCulture);
            string lastName = record.LastName.ToLower(CultureInfo.InvariantCulture);

            if (!this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary.Add(firstName, new List<FileCabinetRecord>());
            }

            if (!this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary.Add(lastName, new List<FileCabinetRecord>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateOfBirthDictionary.Add(record.DateOfBirth, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[firstName].Add(this.list[record.Id]);
            this.lastNameDictionary[lastName].Add(this.list[record.Id]);
            this.dateOfBirthDictionary[record.DateOfBirth].Add(this.list[record.Id]);
        }
    }
}