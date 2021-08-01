using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Used to export data to csv file.
    /// </summary>
    public class FileCabinetCsvWriter
    {
        private readonly StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">A stream with opened file to export.</param>
        public FileCabinetCsvWriter(StreamWriter writer)
        {
            this.writer = writer;
            this.writer.WriteLine(Resources.CSVFormat, "ID", "First Name", "Last Name", "Date Of Birth", "Sex", "Number of kids", "Currency", "Budget");
        }

        /// <summary>
        /// Writes record data to a file.
        /// </summary>
        /// <param name="record">A record to write to a file.</param>
        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsNullMessage, "record"));
            }

            this.writer.WriteLine(Resources.CSVFormat, record.Id, record.FirstName, record.LastName, record.DateOfBirth.ToShortDateString(), record.Sex, record.KidsCount, record.Currency, record.Budget);
        }
    }
}
