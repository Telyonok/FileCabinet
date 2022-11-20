using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Reads records from a xml file.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly XmlSerializer recordSerializer = new (typeof(List<FileCabinetRecord>));
        private readonly XmlReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">Stream to read from.</param>
        public FileCabinetRecordXmlReader(StreamReader reader)
        {
            this.reader = XmlReader.Create(reader);
        }

        /// <summary>
        /// Reads records from a xml file.
        /// </summary>
        /// <returns>List of read records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord>? records;
            try
            {
                records = this.recordSerializer.Deserialize(this.reader) as List<FileCabinetRecord>;

                if (records == null)
                {
                    return new List<FileCabinetRecord>();
                }
            }
            catch
            {
                Console.WriteLine("Error reading from file");
                return new List<FileCabinetRecord>();
            }

            return records;
        }
    }
}
