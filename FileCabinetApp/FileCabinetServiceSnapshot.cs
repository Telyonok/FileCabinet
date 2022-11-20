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
        private FileCabinetRecord[] records;

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
        /// Returns snapshot's records.
        /// </summary>
        /// <returns>Record array.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            return this.records;
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

            streamWriter.Flush();
        }

        /// <summary>
        /// Creates and utilizes FileCabinetRecordXmlWriter instance.
        /// </summary>
        /// <param name="streamWriter">StreamWriter to use.</param>
        public void SaveToXml(StreamWriter streamWriter)
        {
            FileCabinetRecordXmlWriter xmlWriter = new (streamWriter);
            xmlWriter.Write(this.records);
        }

        /// <summary>
        /// Loads records from csv file.
        /// </summary>
        /// <param name="streamReader">Stream to read from.</param>
        public void LoadFromCsv(StreamReader streamReader)
        {
            FileCabinetRecordCsvReader csvReader = new (streamReader);
            List<FileCabinetRecord> readRecords = (List<FileCabinetRecord>)csvReader.ReadAll();
            this.LoadReadRecords(readRecords);
        }

        /// <summary>
        /// Loads records from xml file.
        /// </summary>
        /// <param name="streamReader">Stream to read from.</param>
        public void LoadFromXml(StreamReader streamReader)
        {
            FileCabinetRecordXmlReader xmlReader = new (streamReader);
            List<FileCabinetRecord> readRecords = (List<FileCabinetRecord>)xmlReader.ReadAll();
            this.LoadReadRecords(readRecords);
        }

        private void LoadReadRecords(List<FileCabinetRecord> readRecords)
        {
            if (readRecords.Count == 0)
            {
                return;
            }

            int lastId = -1;
            if (this.records.Length != 0)
            {
                lastId = this.records[^1].Id;
            }

            if (lastId < readRecords.First().Id - 1)
            {
                Console.WriteLine($"Couldn't load records from file. First record's id from file should be lower or equal to {lastId + 1}.");
                return;
            }

            FileCabinetRecord[] oldRecords = this.records;
            this.records = new FileCabinetRecord[oldRecords.Length + readRecords.Count - (lastId - readRecords.First().Id + 1)];
            oldRecords.CopyTo(this.records, 0);
            foreach (var rec in readRecords)
            {
                this.records[rec.Id] = rec;
            }
        }
    }
}
