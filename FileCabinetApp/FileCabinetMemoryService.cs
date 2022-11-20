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
        private Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();
        private List<FileCabinetRecord> list = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">Set of validation methods.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.Validator = validator;
        }

        /// <inheritdoc/>
        public IRecordValidator Validator { get; }

        /// <inheritdoc/>
        public string GetServiceName()
        {
            return "memory";
        }

        /// <inheritdoc/>
        public int CreateRecord(InputDataSet dataSet)
        {
            var record = new FileCabinetRecord(this.list.Count, dataSet);

            this.list.Add(record);
            this.UpdateDictionaries(record);
            Console.Write($"Record {record.Id + 1} created successfuly: \n");

            return record.Id;
        }

        /// <inheritdoc/>
        public void EditRecord(int value, InputDataSet dataSet)
        {
            int id = value - 1;
            string oldFirstName = this.list[id].FirstName;
            string oldLastName = this.list[id].LastName;
            DateTime oldDateOfBirth = this.list[id].DateOfBirth;

            this.firstNameDictionary[oldFirstName.ToLower(CultureInfo.InvariantCulture)].Remove(this.list[id]);
            this.lastNameDictionary[oldLastName.ToLower(CultureInfo.InvariantCulture)].Remove(this.list[id]);
            this.dateOfBirthDictionary[oldDateOfBirth].Remove(this.list[id]);

            this.list[id] = new FileCabinetRecord(id, dataSet);

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

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list);
        }

        /// <inheritdoc/>
        public void Finish()
        {
            return;
        }

        /// <inheritdoc/>
        public void RestoreSnapshot(FileCabinetServiceSnapshot snapshot)
        {
            List<FileCabinetRecord> newList = snapshot.GetRecords().ToList<FileCabinetRecord>();
            this.list = new (newList);
            this.firstNameDictionary.Clear();
            this.lastNameDictionary.Clear();
            this.dateOfBirthDictionary.Clear();

            int realId = 0;
            foreach (var record in newList)
            {
                Tuple<bool, string> tuple = this.Validator.ValidateRecord(record);
                if (!tuple.Item1)
                {
                    Console.WriteLine($"Error: \"{tuple.Item2}\" caused by record with Id: {record.Id}. Skipping.");
                    this.list.Remove(record);
                    continue;
                }

                record.Id = realId;
                this.UpdateDictionaries(record);
                realId++;
            }
        }

        private void UpdateDictionaries(FileCabinetRecord record)
        {
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