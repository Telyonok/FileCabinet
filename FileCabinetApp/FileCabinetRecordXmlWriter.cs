using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods for xml outputing.
    /// </summary>
    internal class FileCabinetRecordXmlWriter
    {
        private readonly StreamWriter textWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="textWriter">Stream to write to.</param>
        public FileCabinetRecordXmlWriter(StreamWriter textWriter)
        {
            this.textWriter = textWriter;
        }

        /// <summary>
        /// Writes record's information to file.
        /// </summary>
        /// <param name="record">Record to write.</param>
        public void Write(FileCabinetRecord record)
        {
            this.textWriter.WriteLine($"\t<record id=\"{record.Id + 1}\">");
            this.textWriter.WriteLine($"\t\t<name first=\"{record.FirstName}\" last=\"{record.LastName}\" />");
            this.textWriter.WriteLine($"\t\t<sex>{record.Sex}</sex>");
            this.textWriter.WriteLine($"\t\t<weight>{record.Weight}</weight>");
            this.textWriter.WriteLine($"\t\t<height>{record.Height}</height>");
            this.textWriter.WriteLine($"\t\t<dateOfBirth>{record.DateOfBirth.ToString("dd/mm/yyyy", CultureInfo.InvariantCulture)}</dateOfBirth>");
            this.textWriter.WriteLine($"\t</record>");
        }

        /// <summary>
        /// Prints xml file's processing instruction and 'record' opening tag.
        /// </summary>
        public void PrintOpeningText()
        {
            this.textWriter.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<records>");
        }

        /// <summary>
        /// Prints xml file's processing instruction and 'record' opening tag.
        /// </summary>
        public void PrintClosingText()
        {
            this.textWriter.WriteLine("</records>");
        }
    }
}
