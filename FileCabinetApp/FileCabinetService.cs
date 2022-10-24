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
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="validator">Set of validation methods.</param>
        public FileCabinetService(IRecordValidator validator)
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

        /// <summary>
        /// Creates a new record after validating user input and returns it id.
        /// </summary>
        /// <param name="unvalidatedRecord">Record data that needs to be validated.</param>
        /// <returns> Record's id. </returns>
        public int CreateRecord(UnvalidatedRecordData unvalidatedRecord)
        {
            this.Validator.ValidateParameters(unvalidatedRecord);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = unvalidatedRecord.FirstName,
                LastName = unvalidatedRecord.LastName,
                Sex = unvalidatedRecord.Sex,
                Weight = unvalidatedRecord.Weight,
                Height = unvalidatedRecord.Height,
                DateOfBirth = unvalidatedRecord.DateOfBirth,
            };

            this.list.Add(record);
            this.UpdateDictionaries(unvalidatedRecord, record.Id);

            return record.Id;
        }

        /// <summary>
        /// Edits a record after validating new input.
        /// </summary>
        /// <param name="id">Record's id.</param>
        /// <param name="unvalidatedRecord">Record data that needs to be validated.</param>
        public void EditRecord(int id, UnvalidatedRecordData unvalidatedRecord)
        {
            this.Validator.ValidateParameters(unvalidatedRecord);
            string? oldFirstName = this.list[id - 1].FirstName;
            string? oldLastName = this.list[id - 1].LastName;
            DateTime oldDateOfBirth = this.list[id - 1].DateOfBirth;

            if (oldFirstName is null)
            {
                throw new MissingMemberException(nameof(oldFirstName));
            }

            if (oldLastName is null)
            {
                throw new MissingMemberException(nameof(oldLastName));
            }

            this.firstNameDictionary[oldFirstName].Remove(this.list[id - 1]);
            this.lastNameDictionary[oldLastName].Remove(this.list[id - 1]);
            this.dateOfBirthDictionary[oldDateOfBirth].Remove(this.list[id - 1]);

            this.list[id - 1].FirstName = unvalidatedRecord.FirstName;
            this.list[id - 1].LastName = unvalidatedRecord.LastName;
            this.list[id - 1].Sex = unvalidatedRecord.Sex;
            this.list[id - 1].Weight = unvalidatedRecord.Weight;
            this.list[id - 1].Height = unvalidatedRecord.Height;
            this.list[id - 1].DateOfBirth = unvalidatedRecord.DateOfBirth;

            this.UpdateDictionaries(unvalidatedRecord, id);
        }

        /// <summary>
        /// Gets an array with all records from the <c>list</c>.
        /// </summary>
        /// <returns> Array with all records. </returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.list.AsReadOnly();
        }

        /// <summary>
        /// Returns the amount of created records.
        /// </summary>
        /// <returns> Count of elements in <c>list</c>. </returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Returns all records with their first name equal to <c>firstName</c>.
        /// </summary>
        /// <param name="firstName">Name to search by.</param>
        /// <returns> Array of all records with corresponding first name. </returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (!this.firstNameDictionary.ContainsKey(firstName))
            {
                return new List<FileCabinetRecord>().AsReadOnly();
            }

            return this.firstNameDictionary[firstName].AsReadOnly();
        }

        /// <summary>
        /// Returns all records with their last name equal to <c>lastName</c>.
        /// </summary>
        /// <param name="lastName">Name to search by.</param>
        /// <returns> Array of all records with corresponding last name. </returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (!this.lastNameDictionary.ContainsKey(lastName))
            {
                return new List<FileCabinetRecord>().AsReadOnly();
            }

            return this.lastNameDictionary[lastName].AsReadOnly();
        }

        /// <summary>
        /// Returns all records with their date of birth equal to <c>dateOfBirth</c>.
        /// </summary>
        /// <param name="dateOfBirth">Date to search by.</param>
        /// <returns> Array of all records with corresponding date of birth. </returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                return new List<FileCabinetRecord>().AsReadOnly();
            }

            return this.dateOfBirthDictionary[dateOfBirth].AsReadOnly();
        }

        private void UpdateDictionaries(UnvalidatedRecordData unvalidatedRecord, int id)
        {
            unvalidatedRecord.FirstName = unvalidatedRecord.FirstName.ToLower(CultureInfo.InvariantCulture);
            unvalidatedRecord.LastName = unvalidatedRecord.LastName.ToLower(CultureInfo.InvariantCulture);

            if (!this.firstNameDictionary.ContainsKey(unvalidatedRecord.FirstName))
            {
                this.firstNameDictionary.Add(unvalidatedRecord.FirstName, new List<FileCabinetRecord>());
            }

            if (!this.lastNameDictionary.ContainsKey(unvalidatedRecord.LastName))
            {
                this.lastNameDictionary.Add(unvalidatedRecord.LastName, new List<FileCabinetRecord>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(unvalidatedRecord.DateOfBirth))
            {
                this.dateOfBirthDictionary.Add(unvalidatedRecord.DateOfBirth, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[unvalidatedRecord.FirstName].Add(this.list[id - 1]);
            this.lastNameDictionary[unvalidatedRecord.LastName].Add(this.list[id - 1]);
            this.dateOfBirthDictionary[unvalidatedRecord.DateOfBirth].Add(this.list[id - 1]);
        }
    }
}