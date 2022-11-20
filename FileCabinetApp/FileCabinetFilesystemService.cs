using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
            this.fileStream.Position = this.recordCount * RecordSize;
            short status = 0;

            this.fileStream.Write(BitConverter.GetBytes(status), 0, 2);
            this.fileStream.Write(BitConverter.GetBytes(this.recordCount), 0, 4);
            this.WriteToStream(dataSet);

            this.recordCount++;
            return this.recordCount - 1;
        }

        /// <inheritdoc/>
        public void EditRecord(int value, InputDataSet dataSet)
        {
            byte[] buffer = new byte[4];
            int currentValue;
            int j = 0;
            do
            {
                this.fileStream.Position = (j * RecordSize) + 2;
                this.fileStream.Read(buffer, 0, buffer.Length);
                currentValue = BitConverter.ToInt32(buffer);
                j++;
            }
            while (currentValue != value - 1);

            this.WriteToStream(dataSet);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            byte[] yearBuffer = BitConverter.GetBytes(dateOfBirth.Year);
            byte[] monthBuffer = BitConverter.GetBytes(dateOfBirth.Month);
            byte[] dayBuffer = BitConverter.GetBytes(dateOfBirth.Day);
            byte[] buffer = new byte[yearBuffer.Length + monthBuffer.Length + dayBuffer.Length];

            yearBuffer.CopyTo(buffer, 0);
            monthBuffer.CopyTo(buffer, 4);
            dayBuffer.CopyTo(buffer, 8);

            return this.GetSpecifiedRecords(buffer, 265);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            char[] firstNameArray = new char[120];
            firstName.CopyTo(firstNameArray);
            return this.GetSpecifiedRecords(Encoding.Default.GetBytes(firstNameArray), 6);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            char[] lastNameArray = new char[120];
            lastName.CopyTo(lastNameArray);
            return this.GetSpecifiedRecords(Encoding.Default.GetBytes(lastNameArray), 126);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = new ();
            this.fileStream.Position = 0;
            for (int i = 0; i < this.recordCount; i++)
            {
                records.Add(this.ReadRecord());
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
            return new FileCabinetServiceSnapshot(this.GetRecords().ToList());
        }

        /// <inheritdoc/>
        public void Finish()
        {
            this.fileStream.Flush();
            this.fileStream.Close();
            this.fileStream.Dispose();
        }

        /// <inheritdoc/>
        public void RestoreSnapshot(FileCabinetServiceSnapshot snapshot)
        {
            this.fileStream.Position = 0;
            this.recordCount = 0;
            foreach (var record in snapshot.GetRecords())
            {
                this.CreateRecord(record);
            }
        }

        private void WriteToStream(InputDataSet dataSet)
        {
            char[] firstNameArray = new char[120];
            dataSet.FirstName.CopyTo(firstNameArray);

            char[] lastNameArray = new char[120];
            dataSet.LastName.CopyTo(lastNameArray);

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
        }

        private FileCabinetRecord ReadRecord()
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
            return new FileCabinetRecord(id, firstName, lastName, sex, weight, height, date);
        }

        private ReadOnlyCollection<FileCabinetRecord> GetSpecifiedRecords(byte[] sample, int offset)
        {
            List<FileCabinetRecord> records = new ();

            byte[] buffer = new byte[sample.Length];

            for (int j = 0; j < this.recordCount; j++)
            {
                this.fileStream.Position = (j * RecordSize) + offset;
                this.fileStream.Read(buffer, 0, buffer.Length);
                if (string.Equals(Encoding.UTF8.GetString(sample), Encoding.UTF8.GetString(buffer), StringComparison.OrdinalIgnoreCase))
                {
                    this.fileStream.Position = j * RecordSize;
                    records.Add(this.ReadRecord());
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        private int CreateRecord(FileCabinetRecord record)
        {
            return this.CreateRecord(new InputDataSet(record.FirstName, record.LastName, record.Sex, record.Weight, record.Height, record.DateOfBirth));
        }
    }
}
