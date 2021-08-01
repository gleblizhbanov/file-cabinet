using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Used to validate parameters using custom rules.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <summary>
        /// Validates name parameter.
        /// </summary>
        /// <param name="name">Name parameter to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when the name is null, empty or consists only of whitespace characters.</exception>
        /// <exception cref="ArgumentException">Thrown when the length of person's first or last name is less than 2 or greater than 60.</exception>
        public void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), string.Format(CultureInfo.InvariantCulture, Resources.StringIsNullOrWhiteSpaceMessage, "name"));
            }

            if (name.Length is < 2 or > 60)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "length of the name"), nameof(name));
            }

            if (!name.All(char.IsLetter))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "name"));
            }
        }

        /// <summary>
        /// Validates date of birth parameter.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to validate.</param>
        public void ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth < new DateTime(DateTime.Now.Year - 120, DateTime.Now.Month, DateTime.Now.Day) || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "date of birth"), nameof(dateOfBirth));
            }
        }

        /// <summary>
        /// Validates sex parameter.
        /// </summary>
        /// <param name="sex">Sex to validate.</param>
        public void ValidateSex(Sex sex)
        {
            if (sex is not Sex.Male and not Sex.Female)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "sex"), nameof(sex));
            }
        }

        /// <summary>
        /// Validates count parameter.
        /// </summary>
        /// <param name="count">Count to validate.</param>
        public void ValidateCount(short count)
        {
            if (count < 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsNegativeMessage, "count"), nameof(count));
            }
        }

        /// <summary>
        /// Validates count parameter.
        /// </summary>
        /// <param name="count">Count to validate.</param>
        public void ValidateCount(decimal count)
        {
            if (count < 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsNegativeMessage, "count"), nameof(count));
            }
        }

        /// <summary>
        /// Validates currency symbol parameter.
        /// </summary>
        /// <param name="currency">Currency symbol to validate.</param>
        public void ValidateCurrency(char currency)
        {
            if (currency is not '$' and not '€' and not '¥' and not '£' and not '₩' and not '₿' and not '₽')
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "currency symbol"), nameof(currency));
            }
        }
    }
}
