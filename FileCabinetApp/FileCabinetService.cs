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
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// Creates a new record after validating user input and returns it id.
        /// </summary>
        /// <param name="unvalidatedRecord">Record data that needs to be validated.</param>
        /// <returns> Record's id. </returns>
        public int CreateRecord(UnvalidatedRecordData unvalidatedRecord)
        {
            ValidateInput(unvalidatedRecord);

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
            ValidateInput(unvalidatedRecord);
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
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
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
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (!this.firstNameDictionary.ContainsKey(firstName))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.firstNameDictionary[firstName].ToArray();
        }

        /// <summary>
        /// Returns all records with their last name equal to <c>lastName</c>.
        /// </summary>
        /// <param name="lastName">Name to search by.</param>
        /// <returns> Array of all records with corresponding last name. </returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (!this.lastNameDictionary.ContainsKey(lastName))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.lastNameDictionary[lastName].ToArray();
        }

        /// <summary>
        /// Returns all records with their date of birth equal to <c>dateOfBirth</c>.
        /// </summary>
        /// <param name="dateOfBirth">Date to search by.</param>
        /// <returns> Array of all records with corresponding date of birth. </returns>
        public FileCabinetRecord[] FindByDateOfBirth(DateTime dateOfBirth)
        {
            if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.dateOfBirthDictionary[dateOfBirth].ToArray();
        }

        private static void ValidateNameString(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.Length < 2 || name.Length > 60 || name.Trim() == string.Empty)
            {
                throw new ArgumentException("String must be 2 to 60 characters long and consist not only of whitespaces", nameof(name));
            }
        }

        private static void ValidateDateTime(DateTime dateTime)
        {
            var marginDate = new DateTime(year: 1950, month: 1, day: 1);
            if (dateTime.CompareTo(DateTime.Now) > 0 || dateTime.CompareTo(marginDate) < 0)
            {
                throw new ArgumentException("Incorrect Date of birth");
            }
        }

        private static void ValidateSex(char sex)
        {
            if (!char.IsLetter(sex))
            {
                throw new ArgumentException("Sex should be a letter!");
            }
        }

        private static void ValidateWeight(short weight)
        {
            if (weight < 0 || weight > 300)
            {
                throw new ArgumentException("Weight must be from 0 to 300");
            }
        }

        private static void ValidateHeight(decimal height)
        {
            if (height < 0 || height > 300)
            {
                throw new ArgumentException("Height must be from 0 to 300");
            }
        }

        private static void ValidateInput(UnvalidatedRecordData unvalidatedRecord)
        {
            ValidateNameString(unvalidatedRecord.FirstName);
            ValidateNameString(unvalidatedRecord.LastName);
            ValidateDateTime(unvalidatedRecord.DateOfBirth);
            ValidateSex(unvalidatedRecord.Sex);
            ValidateWeight(unvalidatedRecord.Weight);
            ValidateHeight(unvalidatedRecord.Height);
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