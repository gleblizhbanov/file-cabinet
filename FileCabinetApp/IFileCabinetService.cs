using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// An interface for FileCabinetService classes.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Adds a new record to the list.
        /// </summary>
        /// <param name="record">A record to add to the list.</param>
        /// <returns>The ID of the record.</returns>
        public int CreateRecord(FileCabinetRecord record);

        /// <summary>
        /// Allows user to edit the data of the record with <paramref name="id"/> ID.
        /// </summary>
        /// <param name="id">The ID of the record to edit.</param>
        /// <param name="newRecord">A record to replace old one with.</param>
        public void EditRecord(int id, FileCabinetRecord newRecord);

        /// <summary>
        /// Searches for the records with the first name value equal to <paramref name="firstName"/>.
        /// </summary>
        /// <param name="firstName">The first name of the person.</param>
        /// <returns>The array of records if they're found or an empty array if not.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Searches for the records with the last name value equal to <paramref name="lastName"/>.
        /// </summary>
        /// <param name="lastName">The last name of the person.</param>
        /// <returns>The array of records if they're found or an empty array if not.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Searches for the records with the date of birth value equal to <paramref name="dateOfBirth"/>.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth of the person.</param>
        /// <returns>The array of records if they're found or an empty array if not.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// Searches for the records with the sex value equal to <paramref name="sex"/>.
        /// </summary>
        /// <param name="sex">The sex of the person.</param>
        /// <returns>The array of records if they're found or an empty array if not.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindBySex(Sex sex);

        /// <summary>
        /// Searches for the records with the number of kids equal to <paramref name="kidsCount"/>.
        /// </summary>
        /// <param name="kidsCount">The number of kids the person has.</param>
        /// <returns>The array of records if they're found or an empty array if not.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByKidsCount(short kidsCount);

        /// <summary>
        /// Searches for the records with the budget equal to <paramref name="amount"/> of <paramref name="currency"/>.
        /// </summary>
        /// <param name="amount">The amount of money the person has.</param>
        /// <param name="currency">The currency symbol of person's savings.</param>
        /// <returns>The array of records if they're found or an empty array if not.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByBudget(decimal amount, char currency);

        /// <summary>
        /// Obtains the list of all records.
        /// </summary>
        /// <returns>The array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Counts the number of records.
        /// </summary>
        /// <returns>The number of records.</returns>
        public int GetStat();
    }
}
