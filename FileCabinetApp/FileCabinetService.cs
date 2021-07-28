using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();

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

            if (currency is not '$' or '€' or '¥' or '£' or '₩' or '₿' or '₽')
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

            return record.Id;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, Sex sex, short kidsCount, decimal budget, char currency)
        {
            this.list[id - 1].FirstName = firstName;
            this.list[id - 1].LastName = lastName;
            this.list[id - 1].DateOfBirth = dateOfBirth;
            this.list[id - 1].Sex = sex;
            this.list[id - 1].KidsCount = kidsCount;
            this.list[id - 1].Budget = budget;
            this.list[id - 1].Currency = currency;
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
