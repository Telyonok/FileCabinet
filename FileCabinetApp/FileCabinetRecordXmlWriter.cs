using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods for xml outputing.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly XmlSerializer recordSerializer = new (typeof(FileCabinetRecord[]));
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
        /// <param name="records">Records to write.</param>
        public void Write(FileCabinetRecord[] records)
        {
            this.recordSerializer.Serialize(this.textWriter, records);
            this.textWriter.Flush();
        }
    }
}
