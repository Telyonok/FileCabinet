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
        /// Gets validator value.
        /// </summary>
        /// <value>
        /// Set of validation rules.
        /// </value>
        public IRecordValidator Validator { get; }

        /// <summary>
        /// Gets name of the service.
        /// </summary>
        /// <returns>Service name.</returns>
        public string GetServiceName();

        /// <summary>
        /// Creates a new record after validating user input and returns it id.
        /// </summary>
        /// <param name="dataSet">Input user data.</param>
        /// <returns> Record's id. </returns>
        public int CreateRecord(InputDataSet dataSet);

        /// <summary>
        /// Edits a record after validating new input.
        /// </summary>
        /// <param name="value">Record's order number.</param>
        /// <param name="dataSet">Input user data.</param>
        public void EditRecord(int value, InputDataSet dataSet);

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

        /// <summary>
        /// Creates a snapshot of current recordList.
        /// </summary>
        /// <returns>Created recordList snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Service cleans after itself and finishes it's work.
        /// </summary>
        public void Finish();

        /// <summary>
        /// Loads records from snapshot.
        /// </summary>
        /// <param name="snapshot">Snapshot with new records.</param>
        public void RestoreSnapshot(FileCabinetServiceSnapshot snapshot);
    }
}
