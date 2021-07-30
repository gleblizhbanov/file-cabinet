using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet service with default validation.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Validates input parameters.
        /// </summary>
        /// <param name="record">Record parameter to validate.</param>
        protected override void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsNullMessage, "record"));
            }

            if (string.IsNullOrWhiteSpace(record.FirstName))
            {
                throw new ArgumentNullException(nameof(record), string.Format(CultureInfo.InvariantCulture, Resources.StringIsNullOrWhiteSpaceMessage, "first name"));
            }

            if (string.IsNullOrWhiteSpace(record.LastName))
            {
                throw new ArgumentNullException(nameof(record), string.Format(CultureInfo.InvariantCulture, Resources.StringIsNullOrWhiteSpaceMessage, "last name"));
            }

            if (record.FirstName.Length is < 2 or > 60)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "length of the first name"), nameof(record));
            }

            if (record.LastName.Length is < 2 or > 60)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "length of the last name"), nameof(record));
            }

            if (record.DateOfBirth < new DateTime(1950, 1, 1) || record.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "date of birth"), nameof(record));
            }

            if (record.Sex is not Sex.Male and not Sex.Female)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "sex"), nameof(record));
            }

            if (record.Budget < 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsNegativeMessage, "amount of money"), nameof(record));
            }

            if (record.KidsCount < 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsNegativeMessage, "number of kids"), nameof(record));
            }

            if (record.Currency is not '$' and not '€' and not '¥' and not '£' and not '₩' and not '₿' and not '₽')
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "currency symbol"), nameof(record));
            }
        }
    }
}
