using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Captures current record list.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="recordList">List of records.</param>
        public FileCabinetServiceSnapshot(List<FileCabinetRecord> recordList)
        {
            this.records = new FileCabinetRecord[recordList.Count];
            this.records = recordList.ToArray();
        }

        /// <summary>
        /// Creates and utilizes FileCabinetRecordCsvWriter instance.
        /// </summary>
        /// <param name="streamWriter">StreamWriter to use.</param>
        public void SaveToCsv(StreamWriter streamWriter)
        {
            FileCabinetRecordCsvWriter csvWriter = new (streamWriter);
            csvWriter.PrintParamNames();
            foreach (var record in this.records)
            {
                csvWriter.Write(record);
            }
        }

        /// <summary>
        /// Creates and utilizes FileCabinetRecordXmlWriter instance.
        /// </summary>
        /// <param name="streamWriter">StreamWriter to use.</param>
        public void SaveToXml(StreamWriter streamWriter)
        {
            FileCabinetRecordXmlWriter xmlWriter = new (streamWriter);
            xmlWriter.PrintOpeningText();
            foreach (var record in this.records)
            {
                xmlWriter.Write(record);
            }

            xmlWriter.PrintClosingText();
        }
    }
}
