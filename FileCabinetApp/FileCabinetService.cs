using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// This class provides methods to manipulate records.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();

        private readonly IRecordValidator validator;

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();
        private readonly Dictionary<Sex, List<FileCabinetRecord>> sexDictionary = new ();
        private readonly Dictionary<short, List<FileCabinetRecord>> kidsCountDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> budgetDictionary = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="validator">Validator object implementing <see cref="IRecordValidator"/> interface.</param>
        public FileCabinetService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Creates new oldRecord and return its ID.
        /// </summary>
        /// <param name="record">A oldRecord to add to the list.</param>
        /// <exception cref="ArgumentNullException">Thrown when person's first or last name is null, empty or consists only of white-space characters.</exception>
        /// <returns>The ID of the oldRecord.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsNullMessage, "record"));
            }

            this.validator.ValidateParameters(record);
            this.list.Add(record);

            AddRecordToTheDictionary(this.firstNameDictionary, key: record.FirstName.ToUpperInvariant(), record);
            AddRecordToTheDictionary(this.lastNameDictionary, key: record.LastName.ToUpperInvariant(), record);
            AddRecordToTheDictionary(this.dateOfBirthDictionary, key: record.DateOfBirth, record);
            AddRecordToTheDictionary(this.sexDictionary, key: record.Sex, record);
            AddRecordToTheDictionary(this.kidsCountDictionary, key: record.KidsCount, record);
            AddRecordToTheDictionary(this.budgetDictionary, key: record.Currency.ToString() + record.Budget, record);

            return record.Id;
        }

        /// <summary>
        /// Allows user to edit the data of the oldRecord with <paramref name="id"/> ID.
        /// </summary>
        /// <param name="id">The ID of the oldRecord to edit.</param>
        /// <param name="newRecord">A oldRecord to replace old one with.</param>
        /// <exception cref="ArgumentNullException">Thrown when new record is null.</exception>
        public void EditRecord(int id, FileCabinetRecord newRecord)
        {
            if (newRecord is null)
            {
                throw new ArgumentNullException(nameof(newRecord), string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsNullMessage, "record"));
            }

            this.validator.ValidateParameters(newRecord);

            var record = this.list[id - 1];

            this.EditFirstName(record, newRecord);
            this.EditLastName(record, newRecord);
            this.EditDateOfBirth(record, newRecord);
            this.EditSex(record, newRecord);
            this.EditKidsCount(record, newRecord);
            this.EditBudget(record, newRecord);
        }

        /// <summary>
        /// Searches for the records with the first name value equal to <paramref name="firstName"/>.
        /// </summary>
        /// <param name="firstName">The first name of the person.</param>
        /// <returns>The array of records if they're found or an empty array if not.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            var upperCaseFirstName = firstName.ToUpperInvariant();
            return this.firstNameDictionary.ContainsKey(upperCaseFirstName) ? this.firstNameDictionary[upperCaseFirstName].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Searches for the records with the last name value equal to <paramref name="lastName"/>.
        /// </summary>
        /// <param name="lastName">The last name of the person.</param>
        /// <returns>The array of records if they're found or an empty array if not.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            var upperCaseLastName = lastName.ToUpperInvariant();
            return this.lastNameDictionary.ContainsKey(upperCaseLastName) ? this.lastNameDictionary[upperCaseLastName].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Searches for the records with the date of birth value equal to <paramref name="dateOfBirth"/>.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth of the person.</param>
        /// <returns>The array of records if they're found or an empty array if not.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(DateTime dateOfBirth)
        {
            return this.dateOfBirthDictionary.ContainsKey(dateOfBirth) ? this.dateOfBirthDictionary[dateOfBirth].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Searches for the records with the sex value equal to <paramref name="sex"/>.
        /// </summary>
        /// <param name="sex">The sex of the person.</param>
        /// <returns>The array of records if they're found or an empty array if not.</returns>
        public FileCabinetRecord[] FindBySex(Sex sex)
        {
            return this.sexDictionary.ContainsKey(sex) ? this.sexDictionary[sex].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Searches for the records with the number of kids equal to <paramref name="kidsCount"/>.
        /// </summary>
        /// <param name="kidsCount">The number of kids the person has.</param>
        /// <returns>The array of records if they're found or an empty array if not.</returns>
        public FileCabinetRecord[] FindByKidsCount(short kidsCount)
        {
            return this.kidsCountDictionary.ContainsKey(kidsCount) ? this.kidsCountDictionary[kidsCount].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Searches for the records with the budget equal to <paramref name="amount"/> of <paramref name="currency"/>.
        /// </summary>
        /// <param name="amount">The amount of money the person has.</param>
        /// <param name="currency">The currency symbol of person's savings.</param>
        /// <returns>The array of records if they're found or an empty array if not.</returns>
        public FileCabinetRecord[] FindByBudget(decimal amount, char currency)
        {
            var budget = currency.ToString() + amount;
            return this.budgetDictionary.ContainsKey(budget) ? this.budgetDictionary[budget].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Obtains the list of all records.
        /// </summary>
        /// <returns>The array of records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        /// <summary>
        /// Counts the number of records.
        /// </summary>
        /// <returns>The number of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        private static void AddRecordToTheDictionary<T>(Dictionary<T, List<FileCabinetRecord>> dictionary, T key, FileCabinetRecord record)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<FileCabinetRecord>());
            }

            dictionary[key].Add(record);
        }

        private void EditFirstName(FileCabinetRecord record, FileCabinetRecord newRecord)
        {
            if (newRecord.FirstName != record.FirstName)
            {
                if (!newRecord.FirstName.Equals(record.FirstName, StringComparison.InvariantCultureIgnoreCase))
                {
                    var upperCaseOldFirstName = record.FirstName.ToUpperInvariant();
                    this.firstNameDictionary[upperCaseOldFirstName].Remove(record);
                    AddRecordToTheDictionary(this.firstNameDictionary, key: newRecord.FirstName.ToUpperInvariant(), record);
                }

                record.FirstName = newRecord.FirstName;
            }
        }

        private void EditLastName(FileCabinetRecord record, FileCabinetRecord newRecord)
        {
            if (newRecord.LastName != record.FirstName)
            {
                if (!newRecord.LastName.Equals(record.LastName, StringComparison.InvariantCultureIgnoreCase))
                {
                    var upperCaseOldLastName = record.LastName.ToUpperInvariant();
                    this.lastNameDictionary[upperCaseOldLastName].Remove(record);
                    AddRecordToTheDictionary(this.lastNameDictionary, key: newRecord.LastName.ToUpperInvariant(), record);
                }

                record.LastName = newRecord.LastName;
            }
        }

        private void EditDateOfBirth(FileCabinetRecord record, FileCabinetRecord newRecord)
        {
            if (newRecord.DateOfBirth != record.DateOfBirth)
            {
                this.dateOfBirthDictionary[newRecord.DateOfBirth].Remove(record);
                record.DateOfBirth = newRecord.DateOfBirth;
                AddRecordToTheDictionary(this.dateOfBirthDictionary, key: newRecord.DateOfBirth, record);
            }
        }

        private void EditSex(FileCabinetRecord record, FileCabinetRecord newRecord)
        {
            if (newRecord.Sex != record.Sex)
            {
                this.sexDictionary[newRecord.Sex].Remove(record);
                record.Sex = newRecord.Sex;
                AddRecordToTheDictionary(this.sexDictionary, key: newRecord.Sex, record);
            }
        }

        private void EditKidsCount(FileCabinetRecord record, FileCabinetRecord newRecord)
        {
            if (newRecord.KidsCount != record.KidsCount)
            {
                this.kidsCountDictionary[newRecord.KidsCount].Remove(record);
                record.KidsCount = newRecord.KidsCount;
                AddRecordToTheDictionary(this.kidsCountDictionary, key: newRecord.KidsCount, record);
            }
        }

        private void EditBudget(FileCabinetRecord record, FileCabinetRecord newRecord)
        {
            if (newRecord.Currency != record.Currency || newRecord.Budget != record.Budget)
            {
                var oldBudget = record.Currency.ToString() + record.Budget;
                var budget = newRecord.Currency.ToString() + newRecord.Budget;
                this.budgetDictionary[oldBudget].Remove(record);
                record.Budget = newRecord.Budget;
                record.Currency = newRecord.Currency;
                AddRecordToTheDictionary(this.budgetDictionary, key: budget, record);
            }
        }
    }
}
