using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Used to export data to xml file.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly XmlWriter xmlWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="xmlWriter">Writer with opened xml file to export data.</param>
        public FileCabinetRecordXmlWriter(XmlWriter xmlWriter)
        {
            this.xmlWriter = xmlWriter;
            this.xmlWriter.WriteStartDocument();
            this.xmlWriter.WriteStartElement("records");
        }

        /// <summary>
        /// Writes record's data to an XML file.
        /// </summary>
        /// <param name="record">A record to write data.</param>
        /// <exception cref="ArgumentNullException">Thrown when the record is null.</exception>>
        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.xmlWriter.WriteStartElement("record");

            this.xmlWriter.WriteStartElement("name");
            this.xmlWriter.WriteAttributeString("first", record.FirstName);
            this.xmlWriter.WriteAttributeString("last", record.LastName);
            this.xmlWriter.WriteEndElement();

            this.xmlWriter.WriteElementString("dateOfBirth", record.DateOfBirth.ToShortDateString());
            this.xmlWriter.WriteElementString("sex", record.Sex.ToString());
            this.xmlWriter.WriteElementString("kidsCount", record.KidsCount.ToString(CultureInfo.InvariantCulture));
            this.xmlWriter.WriteElementString("budget", record.Currency.ToString() + record.Budget);

            this.xmlWriter.WriteEndElement();
        }
    }
}
