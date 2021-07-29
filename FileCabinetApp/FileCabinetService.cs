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

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, Sex sex, short kidsCount, decimal budget, char currency)
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

            if (budget < 0)
            {
                throw new ArgumentException("The budget is negative.", nameof(budget));
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
                Budget = budget,
                Currency = currency,
            };

            this.list.Add(record);

            var upperCaseFirstName = firstName.ToUpperInvariant();

            if (!this.firstNameDictionary.ContainsKey(upperCaseFirstName))
            {
                this.firstNameDictionary.Add(upperCaseFirstName, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[upperCaseFirstName].Add(record);

            return record.Id;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, Sex sex, short kidsCount, decimal budget, char currency)
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

            if (budget < 0)
            {
                throw new ArgumentException("The budget is negative.", nameof(budget));
            }

            if (kidsCount < 0)
            {
                throw new ArgumentException("The count of kids if negative.", nameof(kidsCount));
            }

            if (currency is not '$' and not '€' and not '¥' and not '£' and not '₩' and not '₿' and not '₽')
            {
                throw new ArgumentException("The currency symbol is invalid.", nameof(currency));
            }

            this.firstNameDictionary[this.list[id - 1].FirstName].Remove(this.list[id - 1]);
            this.list[id - 1].FirstName = firstName;
            this.list[id - 1].LastName = lastName;
            this.list[id - 1].DateOfBirth = dateOfBirth;
            this.list[id - 1].Sex = sex;
            this.list[id - 1].KidsCount = kidsCount;
            this.list[id - 1].Budget = budget;
            this.list[id - 1].Currency = currency;

            var upperCaseFirstName = firstName.ToUpperInvariant();

            if (!this.firstNameDictionary.ContainsKey(upperCaseFirstName))
            {
                this.firstNameDictionary.Add(upperCaseFirstName, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[upperCaseFirstName].Add(this.list[id - 1]);
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

            var records = new List<FileCabinetRecord>();
            foreach (var record in this.list)
            {
                if (lastName.Equals(record.LastName, StringComparison.InvariantCultureIgnoreCase))
                {
                    records.Add(record);
                }
            }

            return records.ToArray();
        }

        public FileCabinetRecord[] FindByDateOfBirth(DateTime dateOfBirth)
        {
            var records = new List<FileCabinetRecord>();
            foreach (var record in this.list)
            {
                if (dateOfBirth == record.DateOfBirth)
                {
                    records.Add(record);
                }
            }

            return records.ToArray();
        }

        public FileCabinetRecord[] FindBySex(Sex sex)
        {
            var records = new List<FileCabinetRecord>();
            foreach (var record in this.list)
            {
                if (sex == record.Sex)
                {
                    records.Add(record);
                }
            }

            return records.ToArray();
        }

        public FileCabinetRecord[] FindByKidsCount(short kidsCount)
        {
            var records = new List<FileCabinetRecord>();
            foreach (var record in this.list)
            {
                if (kidsCount == record.KidsCount)
                {
                    records.Add(record);
                }
            }

            return records.ToArray();
        }

        public FileCabinetRecord[] FindByBudget(decimal amount, char currency)
        {
            var records = new List<FileCabinetRecord>();
            foreach (var record in this.list)
            {
                if (currency == record.Currency && amount == record.Budget)
                {
                    records.Add(record);
                }
            }

            return records.ToArray();
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
