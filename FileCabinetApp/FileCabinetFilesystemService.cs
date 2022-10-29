using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Receives input and provides output of records to a file.
    /// </summary>
    internal class FileCabinetFilesystemService : IFileCabinetService
    {
        private readonly FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">File to work with.</param>
        /// <param name="validator">Set of validation methods.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.Validator = validator;
        }

        /// <inheritdoc/>
        public IRecordValidator Validator { get; }

        /// <inheritdoc/>
        public string GetServiceName()
        {
            return "filesystem";
        }

        /// <inheritdoc/>
        public int CreateRecord(InputDataSet dataSet)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void EditRecord(int value, InputDataSet dataSet)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}
