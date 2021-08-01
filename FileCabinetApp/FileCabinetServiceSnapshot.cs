using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Used for creating snapshots and exporting them to files.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">An array of records.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Exports snapshot data to a CSV file.
        /// </summary>
        /// <param name="writer">A stream writer with opened file.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            var csvWriter = new FileCabinetCsvWriter(writer);
            foreach (var record in this.records)
            {
                csvWriter.Write(record);
            }
        }

        /// <summary>
        /// Exports snapshot data to an XML file.
        /// </summary>
        /// <param name="writer">An XML writer with opened file.</param>
        public void SaveToXml(XmlWriter writer)
        {
            var xmlWriter = new FileCabinetRecordXmlWriter(writer);
            foreach (var record in this.records)
            {
                xmlWriter.Write(record);
            }
        }
    }
}
