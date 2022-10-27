using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods for record creation, editing and storage.
    /// </summary>
    internal interface IFileCabinetService
    {
        /// <summary>
        /// Creates a new record after validating user input and returns it id.
        /// </summary>
        /// <returns> Record's id. </returns>
        public int CreateRecord();

        /// <summary>
        /// Edits a record after validating new input.
        /// </summary>
        /// <param name="value">Record's number.</param>
        public void EditRecord(int value);

        /// <summary>
        /// Gets an array with all records from the <c>list</c>.
        /// </summary>
        /// <returns> Array with all records. </returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Returns the amount of created records.
        /// </summary>
        /// <returns> Count of elements in <c>list</c>. </returns>
        public int GetStat();

        /// <summary>
        /// Returns all records with their first name equal to <c>firstName</c>.
        /// </summary>
        /// <param name="firstName">Name to search by.</param>
        /// <returns> Array of all records with corresponding first name. </returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Returns all records with their last name equal to <c>lastName</c>.
        /// </summary>
        /// <param name="lastName">Name to search by.</param>
        /// <returns> Array of all records with corresponding last name. </returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Returns all records with their date of birth equal to <c>dateOfBirth</c>.
        /// </summary>
        /// <param name="dateOfBirth">Date to search by.</param>
        /// <returns> Array of all records with corresponding date of birth. </returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth);
    }
}
