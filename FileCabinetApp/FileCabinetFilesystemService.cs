using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Receives input and provides output of records to a file.
    /// </summary>
    internal class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordSize = 277;
        private readonly FileStream fileStream;
        private int recordCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">File to work with.</param>
        /// <param name="validator">Set of validation methods.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.Validator = validator;
            this.recordCount = 0;
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
            short status = 0;

            this.fileStream.Write(BitConverter.GetBytes(status), 0, 2);
            this.fileStream.Write(BitConverter.GetBytes(this.recordCount), 0, 4);
            char[] firstNameArray = new char[120];
            for (int i = 0; i < dataSet.FirstName.Length; i++)
            {
                firstNameArray[i] = (char)dataSet.FirstName[i];
            }

            char[] lastNameArray = new char[120];
            for (int i = 0; i < dataSet.LastName.Length; i++)
            {
                lastNameArray[i] = (char)dataSet.LastName[i];
            }

            this.fileStream.Write(Encoding.Default.GetBytes(firstNameArray), 0, 120);
            this.fileStream.Write(Encoding.Default.GetBytes(lastNameArray), 0, 120);
            this.fileStream.Write(BitConverter.GetBytes(dataSet.Sex), 0, 1);
            this.fileStream.Write(BitConverter.GetBytes(dataSet.Weight), 0, 2);

            int[] intHeight = decimal.GetBits(dataSet.Height);
            for (int i = 0; i < 4; i++)
            {
                this.fileStream.Write(BitConverter.GetBytes(intHeight[i]), 0, 4);
            }

            this.fileStream.Write(BitConverter.GetBytes(dataSet.DateOfBirth.Year), 0, 4);
            this.fileStream.Write(BitConverter.GetBytes(dataSet.DateOfBirth.Month), 0, 4);
            this.fileStream.Write(BitConverter.GetBytes(dataSet.DateOfBirth.Day), 0, 4);

            this.recordCount++;
            return this.recordCount - 1;
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
            List<FileCabinetRecord> records = new ();
            this.fileStream.Position = 0;
            for (int i = 0; i < this.recordCount; i++)
            {
                byte[] buffer = new byte[277];
                this.fileStream.Read(buffer, 0, buffer.Length);
                short status = BitConverter.ToInt16(buffer.AsSpan()[..2]);
                int id = BitConverter.ToInt32(buffer.AsSpan()[2..6]);
                string firstName = Encoding.UTF8.GetString(buffer[6..126]).Replace("\0", string.Empty, StringComparison.InvariantCulture);
                string lastName = Encoding.UTF8.GetString(buffer[126..246]).Replace("\0", string.Empty, StringComparison.InvariantCulture);
                char sex = Encoding.UTF8.GetString(buffer[246..247])[0];
                short weight = BitConverter.ToInt16(buffer.AsSpan()[247..249]);
                decimal height = BitConverter.ToInt32(buffer.AsSpan()[249..265]);
                int year = BitConverter.ToInt32(buffer.AsSpan()[265..269]);
                int month = BitConverter.ToInt32(buffer.AsSpan()[269..273]);
                int day = BitConverter.ToInt32(buffer.AsSpan()[273..277]);
                DateTime date = new (year, month, day);
                FileCabinetRecord record = new (id, firstName, lastName, sex, weight, height, date);
                records.Add(record);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return this.recordCount;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Finish()
        {
            this.fileStream.Flush();
            this.fileStream.Close();
            this.fileStream.Dispose();
        }
    }
}
