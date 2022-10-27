using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods for csv outputing.
    /// </summary>
    internal class FileCabinetRecordCsvWriter
    {
        private readonly StreamWriter textWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="textWriter">Stream to write to.</param>
        public FileCabinetRecordCsvWriter(StreamWriter textWriter)
        {
            this.textWriter = textWriter;
        }

        /// <summary>
        /// Writes record's information to file.
        /// </summary>
        /// <param name="record">Record to write.</param>
        public void Write(FileCabinetRecord record)
        {
            this.textWriter.WriteLine($"{record.Id + 1}, {record.FirstName}, {record.LastName}, {record.Sex}, {record.Weight}, {record.Height}, {record.DateOfBirth.ToString("dd/mm/yyyy", CultureInfo.InvariantCulture)}");
        }

        /// <summary>
        /// Prints names of parameters.
        /// </summary>
        public void PrintParamNames()
        {
            this.textWriter.WriteLine("Id,First Name,Last Name,Sex,Weight,Height,Date of Birth");
        }
    }
}
