using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet service with custom validation.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Validates input parameters.
        /// </summary>
        /// <param name="record">Record parameter to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when the record is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the first or the last.</exception>
        /// <exception cref="ArgumentException">Thrown when the length of person's first or last name is less than 2 or greater than 20.</exception>
        /// <exception cref="ArgumentException">Thrown when person's date of birth is 120+ years before current date or after the current date.</exception>
        /// <exception cref="ArgumentException">Thrown when person's sex value is not <see cref="Sex.Male"/> or <see cref="Sex.Female"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when person's kids count or person's amount of money is negative.</exception>
        /// <exception cref="ArgumentException">Thrown when the currency symbol is not valid.</exception>
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

            if (record.FirstName.Length is < 2 or > 20)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "length of the first name"), nameof(record));
            }

            if (record.LastName.Length is < 2 or > 20)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "length of the last name"), nameof(record));
            }

            if (!record.FirstName.All(char.IsLetter))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "first name"));
            }

            if (!record.LastName.All(char.IsLetter))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "last name"));
            }

            if (record.DateOfBirth < new DateTime(DateTime.Now.Year - 120, DateTime.Now.Month, DateTime.Now.Day) || record.DateOfBirth > DateTime.Now)
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
