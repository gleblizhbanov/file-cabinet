using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface for validator classes.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates incoming parameters.
        /// </summary>
        /// <param name="record">Record parameter to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="record"/>> is null.</exception>>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.ValidateName(record.FirstName);
            this.ValidateName(record.LastName);
            this.ValidateDateOfBirth(record.DateOfBirth);
            this.ValidateSex(record.Sex);
            this.ValidateCount(record.KidsCount);
            this.ValidateCount(record.Budget);
            this.ValidateCurrency(record.Currency);
        }

        /// <summary>
        /// Validates name parameter.
        /// </summary>
        /// <param name="name">Name parameter to validate.</param>
        public void ValidateName(string name);

        /// <summary>
        /// Validates date of birth parameter.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to validate.</param>
        public void ValidateDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// Validates sex parameter.
        /// </summary>
        /// <param name="sex">Sex to validate.</param>
        public void ValidateSex(Sex sex);

        /// <summary>
        /// Validates count parameter.
        /// </summary>
        /// <param name="count">Count to validate.</param>
        public void ValidateCount(short count);

        /// <summary>
        /// Validates count parameter.
        /// </summary>
        /// <param name="count">Count to validate.</param>
        public void ValidateCount(decimal count);

        /// <summary>
        /// Validates currency symbol parameter.
        /// </summary>
        /// <param name="currency">Currency symbol to validate.</param>
        public void ValidateCurrency(char currency);
    }
}
