using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides properties for manipulating person's personal information.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or person's ID.
        /// </summary>
        /// <value>Person's ID.</value>
        public int Id { get; init; }

        /// <summary>
        /// Gets or sets person's first name.
        /// </summary>
        /// <value>Person's first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets person's last name.
        /// </summary>
        /// <value>Person's last name.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets person's date of birth.
        /// </summary>
        /// <value>Person's date of birth.</value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets person's sex.
        /// </summary>
        /// <value>Person's sex.</value>
        public Sex Sex { get; set; }

        /// <summary>
        /// Gets or sets person's kids count.
        /// </summary>
        /// <value>The number of kids the person has.</value>>
        public short KidsCount { get; set; }

        /// <summary>
        /// Gets or sets person's budget.
        /// </summary>
        /// <value>Amount of money the person has.</value>>
        public decimal Budget { get; set; }

        /// <summary>
        /// Gets or sets currency.
        /// </summary>
        /// <value>The currency symbol of person's savings.</value>>
        public char Currency { get; set; }
    }
}
