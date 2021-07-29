using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();
        private readonly Dictionary<Sex, List<FileCabinetRecord>> sexDictionary = new ();
        private readonly Dictionary<short, List<FileCabinetRecord>> kidsCountDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> budgetDictionary = new ();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, Sex sex, short kidsCount, decimal amountOfMoney, char currency)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (firstName.Length is < 2 or > 60)
            {
                throw new ArgumentException("The length of the first name is invalid.", nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (lastName.Length is < 2 or > 60)
            {
                throw new ArgumentException("The length of the last name is invalid.", nameof(lastName));
            }

            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("The date of birth is invalid.", nameof(dateOfBirth));
            }

            if (sex is not Sex.Male and not Sex.Female)
            {
                throw new ArgumentException("The sex is invalid.", nameof(sex));
            }

            if (amountOfMoney < 0)
            {
                throw new ArgumentException("The amount of money is negative.", nameof(amountOfMoney));
            }

            if (kidsCount < 0)
            {
                throw new ArgumentException("The count of kids if negative.", nameof(kidsCount));
            }

            if (currency is not '$' and not '€' and not '¥' and not '£' and not '₩' and not '₿' and not '₽')
            {
                throw new ArgumentException("The currency symbol is invalid.", nameof(currency));
            }

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Sex = sex,
                KidsCount = kidsCount,
                Budget = amountOfMoney,
                Currency = currency,
            };

            this.list.Add(record);

            var upperCaseFirstName = firstName.ToUpperInvariant();
            if (!this.firstNameDictionary.ContainsKey(upperCaseFirstName))
            {
                this.firstNameDictionary.Add(upperCaseFirstName, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[upperCaseFirstName].Add(record);

            var upperCaseLastName = lastName.ToUpperInvariant();
            if (!this.lastNameDictionary.ContainsKey(upperCaseLastName))
            {
                this.lastNameDictionary.Add(upperCaseLastName, new List<FileCabinetRecord>());
            }

            this.lastNameDictionary[upperCaseLastName].Add(record);

            if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                this.dateOfBirthDictionary.Add(dateOfBirth, new List<FileCabinetRecord>());
            }

            this.dateOfBirthDictionary[dateOfBirth].Add(record);

            if (!this.sexDictionary.ContainsKey(sex))
            {
                this.sexDictionary.Add(sex, new List<FileCabinetRecord>());
            }

            this.sexDictionary[sex].Add(record);

            if (!this.kidsCountDictionary.ContainsKey(kidsCount))
            {
                this.kidsCountDictionary.Add(kidsCount, new List<FileCabinetRecord>());
            }

            this.kidsCountDictionary[kidsCount].Add(record);

            string budget = currency.ToString() + amountOfMoney;
            if (!this.budgetDictionary.ContainsKey(budget))
            {
                this.budgetDictionary.Add(budget, new List<FileCabinetRecord>());
            }

            this.budgetDictionary[budget].Add(record);

            return record.Id;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, Sex sex, short kidsCount, decimal amountOfMoney, char currency)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (firstName.Length is < 2 or > 60)
            {
                throw new ArgumentException("The length of the first name is invalid.", nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (lastName.Length is < 2 or > 60)
            {
                throw new ArgumentException("The length of the last name is invalid.", nameof(lastName));
            }

            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("The date of birth is invalid.", nameof(dateOfBirth));
            }

            if (sex is not Sex.Male or Sex.Female)
            {
                throw new ArgumentException("The sex is invalid.", nameof(sex));
            }

            if (amountOfMoney < 0)
            {
                throw new ArgumentException("The budget is negative.", nameof(amountOfMoney));
            }

            if (kidsCount < 0)
            {
                throw new ArgumentException("The count of kids if negative.", nameof(kidsCount));
            }

            if (currency is not '$' and not '€' and not '¥' and not '£' and not '₩' and not '₿' and not '₽')
            {
                throw new ArgumentException("The currency symbol is invalid.", nameof(currency));
            }

            var upperCaseOldFirstName = this.list[id - 1].FirstName.ToUpperInvariant();
            this.firstNameDictionary[upperCaseOldFirstName].Remove(this.list[id - 1]);
            this.list[id - 1].FirstName = firstName;

            var upperCaseOldLastName = this.list[id - 1].LastName.ToUpperInvariant();
            this.lastNameDictionary[upperCaseOldLastName].Remove(this.list[id - 1]);
            this.list[id - 1].LastName = lastName;

            this.dateOfBirthDictionary[dateOfBirth].Remove(this.list[id - 1]);
            this.list[id - 1].DateOfBirth = dateOfBirth;

            this.sexDictionary[sex].Remove(this.list[id - 1]);
            this.list[id - 1].Sex = sex;

            this.kidsCountDictionary[kidsCount].Remove(this.list[id - 1]);
            this.list[id - 1].KidsCount = kidsCount;

            var oldBudget = this.list[id - 1].Currency.ToString() + this.list[id - 1].Budget;
            this.budgetDictionary[oldBudget].Remove(this.list[id - 1]);
            this.list[id - 1].Budget = amountOfMoney;
            this.list[id - 1].Currency = currency;

            var upperCaseFirstName = firstName.ToUpperInvariant();

            if (!this.firstNameDictionary.ContainsKey(upperCaseFirstName))
            {
                this.firstNameDictionary.Add(upperCaseFirstName, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[upperCaseFirstName].Add(this.list[id - 1]);

            var upperCaseLastName = lastName.ToUpperInvariant();

            if (!this.lastNameDictionary.ContainsKey(upperCaseLastName))
            {
                this.lastNameDictionary.Add(upperCaseLastName, new List<FileCabinetRecord>());
            }

            this.lastNameDictionary[upperCaseLastName].Add(this.list[id - 1]);

            if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                this.dateOfBirthDictionary.Add(dateOfBirth, new List<FileCabinetRecord>());
            }

            this.dateOfBirthDictionary[dateOfBirth].Add(this.list[id - 1]);

            if (!this.sexDictionary.ContainsKey(sex))
            {
                this.sexDictionary.Add(sex, new List<FileCabinetRecord>());
            }

            this.sexDictionary[sex].Add(this.list[id - 1]);

            if (!this.kidsCountDictionary.ContainsKey(kidsCount))
            {
                this.kidsCountDictionary.Add(kidsCount, new List<FileCabinetRecord>());
            }

            this.kidsCountDictionary[kidsCount].Add(this.list[id - 1]);

            var budget = currency.ToString() + amountOfMoney;
            if (!this.budgetDictionary.ContainsKey(budget))
            {
                this.budgetDictionary[budget].Add(this.list[id - 1]);
            }
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            var upperCaseFirstName = firstName.ToUpperInvariant();

            return this.firstNameDictionary.ContainsKey(upperCaseFirstName) ? this.firstNameDictionary[upperCaseFirstName].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            var upperCaseLastName = lastName.ToUpperInvariant();

            return this.lastNameDictionary.ContainsKey(upperCaseLastName) ? this.lastNameDictionary[upperCaseLastName].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] FindByDateOfBirth(DateTime dateOfBirth)
        {
            return this.dateOfBirthDictionary.ContainsKey(dateOfBirth) ? this.dateOfBirthDictionary[dateOfBirth].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] FindBySex(Sex sex)
        {
            return this.sexDictionary.ContainsKey(sex) ? this.sexDictionary[sex].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] FindByKidsCount(short kidsCount)
        {
            return this.kidsCountDictionary.ContainsKey(kidsCount) ? this.kidsCountDictionary[kidsCount].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] FindByBudget(decimal amount, char currency)
        {
            var budget = currency.ToString() + amount;
            return this.budgetDictionary.ContainsKey(budget) ? this.budgetDictionary[budget].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }
    }
}
